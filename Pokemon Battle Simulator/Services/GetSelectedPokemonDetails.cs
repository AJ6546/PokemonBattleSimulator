using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class GetSelectedPokemonDetails: IGetSelectedPokemonDetails
    {
        private readonly ICachePokemon cachePokemon;

        public GetSelectedPokemonDetails(ICachePokemon cachePokemon)
        {
            this.cachePokemon = cachePokemon;
        }

        public async Task ExecuteAsync(PokemonModel pokemon)
        {
            var allPokemon = await cachePokemon.ReadCacheAsync();
            var selectedPokemon = allPokemon.FirstOrDefault(p => p.Pokemon.Equals((Pokemon)pokemon.Id));
            if (selectedPokemon != null)
            {
                pokemon.PrimaryType = selectedPokemon.PrimaryType;
                pokemon.SecondaryType = selectedPokemon.SecondaryType;
                pokemon.Stats = selectedPokemon.Stats;
            }
        }
    }
}
