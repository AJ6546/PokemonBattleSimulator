using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IMoveSelector
    {
        public Task<MoveModel> ExecuteAsync(List<Move> moves);
        public Task<MoveModel> GetSelectedMoveDetails(Move move);
    }
}
