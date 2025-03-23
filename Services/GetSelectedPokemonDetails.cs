using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class GetSelectedPokemonDetails: IGetSelectedPokemonDetails
    {
        private const int MaxNumberOfMoves = 4;
        private static Random random;

        private readonly ICachePokemon cachePokemon;

        public GetSelectedPokemonDetails(ICachePokemon cachePokemon)
        {
            this.cachePokemon = cachePokemon;
            random = new Random();
        }

        public async Task<PokemonModel> ExecuteAsync(int id)
        {
            var allPokemon = await cachePokemon.ReadCacheAsync();
            var pokemon = allPokemon.FirstOrDefault(p => p.Pokemon.Equals((Pokemon)id));

            if (pokemon == null) return null;

            var selectedPokemon = new PokemonModel
            {
                Id = pokemon.Id,
                Pokemon = pokemon.Pokemon,
                PrimaryType = pokemon.PrimaryType,
                SecondaryType = pokemon.SecondaryType,
                Stats = new StatsModel
                {
                    HP = pokemon.Stats.HP,
                    Attack = pokemon.Stats.Attack,
                    Defense = pokemon.Stats.Defense,
                    Speed = pokemon.Stats.Speed,
                    SpecialAttack = pokemon.Stats.SpecialAttack,
                    SpecialDefense = pokemon.Stats.SpecialDefense,
                    BattleStats = pokemon.Stats.BattleStats,
                },
                Moves = pokemon.Moves.Any()
                    ? pokemon.Moves
                        .Take(Math.Min(MaxNumberOfMoves, pokemon.Moves.Count))
                        .OrderBy(m => random.Next())
                        .ToList() : new List<Move>(),
                MovesPP = new Dictionary<Move, int>(pokemon.MovesPP)
            };

            return selectedPokemon;
        }
    }
}
