using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ITurnManager
    {
        List<PokemonModel> ExecuteAsync(List<PokemonModel> allPokemon);
    }
}
