using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ITurnManager
    {
        public Task<Dictionary<PokemonModel, double>> ExecuteAsync(List<PokemonModel> allPokemon, EnvironmentSetter environment);
    }
}
