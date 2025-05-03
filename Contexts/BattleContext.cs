using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Contexts
{
    public class BattleContext
    {
        public EnvironmentSetter CurrentEnvironment { get; set; }
        public PokemonModel SourcePokemon { get; set; }
        public Typing Typing { get; set; }
    }
}
