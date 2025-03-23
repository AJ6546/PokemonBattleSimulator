using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class AttackExecutor : IAttackExecutor
    {
        private readonly ICalculateDamage calculateDamage;

        public AttackExecutor(ICalculateDamage calculateDamage)
        {
            this.calculateDamage = calculateDamage;
        }

        public async Task ExecuteAsync(PokemonModel attacker, 
            PokemonModel target, MoveModel selectedMove, StringBuilder logBuilder)
        {
            if (selectedMove == null) return;

            logBuilder.AppendLine($"{attacker.Pokemon} uses {selectedMove.Move}");

            if (!attacker.MovesPP.ContainsKey(selectedMove.Move))
            {
                attacker.MovesPP[selectedMove.Move] = selectedMove.PP;
            }

            int damage = Math.Max(0, await calculateDamage.ExecuteAsync(attacker, target, selectedMove, 
                logBuilder));

            logBuilder.AppendLine($"{target.Pokemon} takes {damage} damage");
            target.Stats.HP = Math.Max(0, target.Stats.HP - damage);
            logBuilder.AppendLine($"{target.Pokemon} health is {target.Stats.HP}");

            if (target.Stats.HP <= 0)
            {
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
