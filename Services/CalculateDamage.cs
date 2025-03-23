using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class CalculateDamage: ICalculateDamage
    {
        private readonly ILookupTypeChart lookupTypeChart;

        public CalculateDamage(ILookupTypeChart lookupTypeChart)
        {
            this.lookupTypeChart = lookupTypeChart;
        }

        public async Task<int> ExecuteAsync(PokemonModel attacker, PokemonModel defender,
            MoveModel selectedMove, StringBuilder logBuilder)
        {
            float effectiveness = await lookupTypeChart.ExecuteAsync(selectedMove.Typing, defender.PrimaryType);

            if (!defender.SecondaryType.Equals(Typing.None))
            {
                effectiveness *= await lookupTypeChart.ExecuteAsync(selectedMove.Typing, defender.SecondaryType);
            }

            logBuilder.AppendLine($"Type Effectiveness is *{effectiveness}");

            var damageFactor = selectedMove.Category.Equals(Category.Special) ?
                (selectedMove.Power * attacker.Stats.SpecialAttack / defender.Stats.SpecialDefense) :
                (selectedMove.Power * attacker.Stats.Attack / defender.Stats.Defense);

            var hitChance = selectedMove.Accuracy * attacker.Stats.BattleStats.Accuracy / defender.Stats.BattleStats.Evasion;
            var randomMissChance = new Random().NextDouble() * 100;

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

            var damage = effectiveness * damageFactor * stabBoost;
            return (int) damage;
        }

        private double GetStab(PokemonModel attacker, MoveModel selectedMove)
        {
            return attacker.PrimaryType.Equals(selectedMove.Typing)
                || attacker.SecondaryType.Equals(selectedMove.Typing) ? 1.5 : 1;
        }
    }
}
