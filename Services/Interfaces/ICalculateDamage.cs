using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ICalculateDamage
    {
        public Task<int> ExecuteAsync(PokemonModel attacker, PokemonModel defender, MoveModel selectedMove);
    }
}
