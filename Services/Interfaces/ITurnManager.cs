using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ITurnManager
    {
        public Task<List<PokemonModel>> ExecuteAsync(List<PokemonModel> allPokemon, EnvironmentSetter environment);
    }
}
