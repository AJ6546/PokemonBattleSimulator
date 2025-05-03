using PokemonBattleSimulator.Contexts;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class AttackExecutor : IAttackExecutor
    {
        private readonly ICalculateDamage calculateDamage;
        private readonly IGetSelectedPokemonDetails getSelectedPokemonDetails;
        private Random random;
        public AttackExecutor(ICalculateDamage calculateDamage, IGetSelectedPokemonDetails getSelectedPokemonDetails)
        {
            this.calculateDamage = calculateDamage;
            random = new Random();
            this.getSelectedPokemonDetails = getSelectedPokemonDetails;
        }

        public async Task ExecuteAsync(PokemonModel attacker, 
            PokemonModel target, MoveModel selectedMove, BattleContext context, StringBuilder logBuilder)
        {
            if (selectedMove == null) return;

            if (!attacker.MovesPP.ContainsKey(selectedMove.Move))
            {
                attacker.MovesPP[selectedMove.Move] = selectedMove.PP;
            }

            var timesHit = selectedMove.MaxTimesHit == 0 ? selectedMove.MinTimesHit:
                random.Next(selectedMove.MinTimesHit, selectedMove.MaxTimesHit + 1);

            for( int i = 0; i < timesHit; i++ )
            {
                int damage = Math.Max(0, await calculateDamage.ExecuteAsync(attacker, target, selectedMove,
               context, logBuilder));
                if (selectedMove.UserHealthModifier != 0)
                {
                    if (selectedMove.UserHealthModifier > 0)
                    {
                        var MaxHp = (await getSelectedPokemonDetails.ExecuteAsync(attacker.Id)).Stats.HP;
                        var healAmout = (int)(damage * selectedMove.UserHealthModifier);
                        attacker.Stats.HP = Math.Min(MaxHp, attacker.Stats.HP + healAmout);

                        logBuilder.AppendLine($"\n{attacker.Pokemon} healed by {healAmout} the effect of {selectedMove.Move}.\n");
                    }
                    else
                    {
                        var damageAmount = (int)(damage * selectedMove.UserHealthModifier);
                        attacker.Stats.HP = Math.Max(0, attacker.Stats.HP + damageAmount);
                        logBuilder.AppendLine($"\n{attacker.Pokemon} takes recoil damage of {damageAmount} from {selectedMove.Move}.\n");
                        if (attacker.Stats.HP <= 0)
                        {
                            logBuilder.AppendLine($"{attacker.Pokemon} fainted.\n");
                        }
                    }
                }
                

                attacker.CombatDetails.DamageDealt += damage;

                logBuilder.AppendLine($"{target.Pokemon} takes {damage} damage");
                target.Stats.HP = Math.Max(0, target.Stats.HP - damage);
                logBuilder.AppendLine($"{target.Pokemon} health is {target.Stats.HP}");

                if (target.Stats.HP <= 0)
                {
                    attacker.CombatDetails.KnockOutCount += 1;
                    attacker.CombatDetails.KnockedOutPokemon.Add(target.Pokemon);
                    logBuilder.AppendLine($"{target.Pokemon} is no longer able to battle.");
                }

                attacker.MovesPP[selectedMove.Move] -= 1;
                logBuilder.AppendLine($"{selectedMove.Move} - PP: {attacker.MovesPP[selectedMove.Move]}");

                if (attacker.MovesPP[selectedMove.Move] <= 0)
                {
                    attacker.Moves.Remove(selectedMove.Move);
                    attacker.MovesPP.Remove(selectedMove.Move);
                    logBuilder.AppendLine($"{attacker.Pokemon} can no longer use {selectedMove.Move}");
                }
            }
        }
    }
}
