using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface ILookupTypeChart
    {
        public int ExecuteAsync(Typing moveType, Typing defenderType);
    }
}
