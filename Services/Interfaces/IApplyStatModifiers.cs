using PokemonBattleSimulator.Models;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IApplyStatModifiers
    {
        public Task ExecuteAsync(PokemonModel attacker,
            PokemonModel target, List<StatModifier> statModifiers, StringBuilder logBuilder);
    }
}
