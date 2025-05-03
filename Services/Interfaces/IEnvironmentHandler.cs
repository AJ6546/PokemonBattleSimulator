using PokemonBattleSimulator.Contexts;
using PokemonBattleSimulator.Models;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IEnvironmentHandler
    {
        public Task ApplyEnvironmentEffect(EnvironmentSetter environmentSetter, PokemonModel user, BattleContext context);
        public Task TickEnvironment(BattleContext context, StringBuilder logBuilder, List<PokemonModel> allPokemon);
    }
}
