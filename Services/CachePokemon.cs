using Microsoft.Extensions.Caching.Memory;
using PokemonBattleSimulator.Constants;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class CachePokemon: ICachePokemon
    {
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<CachePokemon> logger;
        private readonly IGetPokemon getPokemon;

        public CachePokemon(IMemoryCache memoryCache, ILogger<CachePokemon> logger, 
            IGetPokemon getPokemon)
        {
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.getPokemon = getPokemon;
        }

        public async Task PopulateCacheAsync()
        {
            var pokemon = await getPokemon.ExecuteAsync();

            if (pokemon == null || pokemon.Count < 1)
            {
                logger.LogError($"Pokemon list was not found.");
                return;
            }

            var entryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            memoryCache.Set(CacheKeys.Pokemon, pokemon, entryOptions);

            logger.LogInformation($"Cached Pokemon with key:{CacheKeys.Pokemon}");
        }

        public async Task<List<PokemonModel>> ReadCacheAsync()
        {
            var pokemon = memoryCache.Get<List<PokemonModel>>(CacheKeys.Pokemon);

            if(pokemon == null || pokemon.Count < 1)
            {
                await PopulateCacheAsync();
                pokemon = memoryCache.Get<List<PokemonModel>>(CacheKeys.Pokemon);
            }

            return pokemon;
        }

        public async Task ClearCacheAsync()
        {
           memoryCache.Remove(CacheKeys.Pokemon);
        }
    }
}
