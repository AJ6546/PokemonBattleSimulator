using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ICacheMoves
    {
        public Task PopulateCacheAsync();
        public Task<List<MoveModel>> ReadCacheAsync();
        public Task ClearCacheAsync();
    }
}
