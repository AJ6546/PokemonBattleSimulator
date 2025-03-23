using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class TurnManager: ITurnManager
    {
        private readonly Random random;

        public TurnManager()
        {
            random = new Random();
        }

        public List<PokemonModel> ExecuteAsync(List<PokemonModel> allPokemon)
        {
            return allPokemon
                .OrderByDescending(p => p.Stats.Speed)
                .ThenBy(_ => random.Next())
                .ToList();
        }
    }
}
