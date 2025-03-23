using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ILookupTypeChart
    {
        public Task<float> ExecuteAsync(Typing moveType, Typing defenderType);
    }
}
