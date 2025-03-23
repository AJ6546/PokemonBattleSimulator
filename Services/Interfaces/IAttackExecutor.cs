using PokemonBattleSimulator.Models;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IAttackExecutor
    {
        public Task ExecuteAsync(PokemonModel attacker,
            PokemonModel target, MoveModel selectedMove, StringBuilder logBuilder);
    }
}
