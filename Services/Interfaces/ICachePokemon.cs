using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ICachePokemon
    {
        public Task PopulateCacheAsync();
        public Task<List<PokemonModel>> ReadCacheAsync();
        public Task ClearCacheAsync();
    }
}
