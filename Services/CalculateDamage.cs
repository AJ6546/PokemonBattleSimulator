using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;
using PokemonBattleSimulator.Contexts;

namespace PokemonBattleSimulator.Services
{
    public class CalculateDamage: ICalculateDamage
    {
        private readonly ILookupTypeChart lookupTypeChart;
        private readonly IApplyStatModifiers applyStatModifiers;
        private Random random;
        public CalculateDamage(ILookupTypeChart lookupTypeChart, IApplyStatModifiers applyStatModifiers)
        {
            this.lookupTypeChart = lookupTypeChart;
            this.applyStatModifiers = applyStatModifiers;
            random = new Random();
        }

        public async Task<int> ExecuteAsync(PokemonModel attacker, PokemonModel defender,
            MoveModel selectedMove, BattleContext context, StringBuilder logBuilder)
        {
            float effectiveness = await lookupTypeChart.ExecuteAsync(selectedMove.Typing, defender.PrimaryType);

            if (!defender.SecondaryType.Equals(Typing.None))
            {
                effectiveness *= await lookupTypeChart.ExecuteAsync(selectedMove.Typing, defender.SecondaryType);
            }

            logBuilder.AppendLine($"Type Effectiveness is *{effectiveness}");

            var variablePower = selectedMove.VariablePower;
            var power = variablePower!= null && variablePower.Any() 
                ? variablePower[random.Next(variablePower.Count)] : selectedMove.Power;

            var damageFactor = selectedMove.Category.Equals(Category.Special) ?
                (power * 
                (await applyStatModifiers.GetEffectiveStat(Stat.SpecialAttack, 
                attacker, context.CurrentEnvironment)) 
                / (await applyStatModifiers.GetEffectiveStat(Stat.SpecialDefense, 
                defender, context.CurrentEnvironment))) :
                (power * 
                (await applyStatModifiers.GetEffectiveStat(Stat.Attack, 
                attacker, context.CurrentEnvironment))
                / (await applyStatModifiers.GetEffectiveStat(Stat.Defense, 
                defender, context.CurrentEnvironment)));

            var hitChance = selectedMove.Accuracy *
                (await applyStatModifiers.GetEffectiveStat(Stat.Accuracy,
                attacker, context.CurrentEnvironment)) /
                (await applyStatModifiers.GetEffectiveStat(Stat.Evasion,
                defender, context.CurrentEnvironment));

            var randomMissChance = random.NextDouble() * 100;

            if(randomMissChance > hitChance)
            {
                logBuilder.AppendLine("Move did not hit.");
                return 0;
            }

            var stabBoost = GetStab(attacker, selectedMove);
            if(stabBoost > 1)
            {
                logBuilder.AppendLine($"Attack gets a stab boost of *{stabBoost}");
            }

            var envMultiplier = 1.0;
            if (context?.CurrentEnvironment?.MoveTypeMultipliers != null &&
                context.CurrentEnvironment.MoveTypeMultipliers.TryGetValue(selectedMove.Typing, out var boost))
            {
                envMultiplier = boost;
                logBuilder.AppendLine($"Environmental boost applied: *{envMultiplier} to {selectedMove.Typing} moves.");
            }

            var damage = effectiveness * damageFactor * stabBoost * envMultiplier;
            return (int) damage;
        }

        private double GetStab(PokemonModel attacker, MoveModel selectedMove)
        {
            return attacker.PrimaryType.Equals(selectedMove.Typing)
                || attacker.SecondaryType.Equals(selectedMove.Typing) ? 1.5 : 1;
        }
    }
}
