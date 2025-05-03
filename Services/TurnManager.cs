using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class TurnManager: ITurnManager
    {
        private readonly Random random;
        private readonly IApplyStatModifiers applyStatModifiers;
        public TurnManager(IApplyStatModifiers applyStatModifiers)
        {
            random = new Random();
            this.applyStatModifiers = applyStatModifiers;
        }

        public async Task<List<PokemonModel>> ExecuteAsync(List<PokemonModel> allPokemon, EnvironmentSetter environment)
        {
            var getPokemonSpeedTasks = allPokemon
                .Select(async p => new
                {
                    Pokemon = p,
                    Speed = await applyStatModifiers.GetEffectiveStat(
                        Models.Enum.Stat.Speed, p, environment)
                });


            var results = await Task.WhenAll(getPokemonSpeedTasks);

            return results
                .OrderByDescending(x => x.Speed)
                .ThenBy(_ => random.Next())
                .Select(x => x.Pokemon)
                .ToList();
        }
    }
}
