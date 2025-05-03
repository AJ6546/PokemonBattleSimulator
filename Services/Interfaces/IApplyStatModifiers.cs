using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IApplyStatModifiers
    {
        public Task ExecuteAsync(PokemonModel attacker,
            PokemonModel target, List<StatModifier> statModifiers, StringBuilder logBuilder);
        public Task<double> GetEffectiveStat(StatModifierType stat, PokemonModel pokemon,
            EnvironmentSetter environment);
    }
}
