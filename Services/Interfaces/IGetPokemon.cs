using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IGetPokemon
    {
        public Task<List<PokemonModel>> ExecuteAsync();
    }
}
