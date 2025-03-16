using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ICacheMoves
    {
        public Task PopulateCacheAsync();
        public Task<List<MoveModel>> ReadCacheAsync();
        public void ClearCacheAsync();
    }
}
