using PokemonBattleSimulator.Models;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IApplyStatEffects
    {
        public Task ExecuteAsync(PokemonModel attacker,
            PokemonModel target, List<StatEffect> statEffects, StringBuilder logBuilder);
    }
}
