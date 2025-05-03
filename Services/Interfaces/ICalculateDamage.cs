using PokemonBattleSimulator.Contexts;
using PokemonBattleSimulator.Models;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ICalculateDamage
    {
        public Task<int> ExecuteAsync(PokemonModel attacker, PokemonModel defender, 
            MoveModel selectedMove, BattleContext context, StringBuilder logBuilder);
    }
}
