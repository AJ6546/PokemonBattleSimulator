using PokemonBattleSimulator.Models;
using System.Text;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IStatusEffectHandler
    {
        public Task ApplyStatusEffectAsync(PokemonModel attacker, PokemonModel target, MoveModel move, StringBuilder logBuilder);
        public Task<bool> ProcessStatusEffectTurnAsync(PokemonModel pokemon, StringBuilder logBuilder);
    }
}
