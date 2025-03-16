using Microsoft.Extensions.Caching.Memory;
using PokemonBattleSimulator.Constants;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class CacheMoves: ICacheMoves
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<CacheMoves> logger;
        private readonly IGetMoves getMoves;

        public CacheMoves(IMemoryCache memoryCache, ILogger<CacheMoves> logger,
            IGetMoves getMoves)
        {
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.getMoves = getMoves;
        }

        public async Task PopulateCacheAsync()
        {
            var moves = await getMoves.ExecuteAsync();

            if (moves == null || moves.Count < 1)
            {
                logger.LogError($"Moves list was not found.");
                return;
            }

            var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            memoryCache.Set(CacheKeys.Moves, moves, entryOptions);

            logger.LogInformation($"Cached Moves with key:{CacheKeys.Moves}");
        }

        public async Task<List<MoveModel>> ReadCacheAsync()
        {
            var moves = memoryCache.Get<List<MoveModel>>(CacheKeys.Moves);

            if (moves == null || moves.Count < 1)
            {
                await PopulateCacheAsync();
                moves = memoryCache.Get<List<MoveModel>>(CacheKeys.Moves);
            }

            return moves;
        }

        public void ClearCacheAsync()
        {
            memoryCache.Remove(CacheKeys.Moves);
        }
    }
}
