using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class MoveSelector: IMoveSelector
    {
        private readonly Random random;
        private readonly ICacheMoves cacheMoves;

        public MoveSelector(ICacheMoves cacheMoves)
        {
            this.cacheMoves = cacheMoves;
            random = new Random();
        }

        public async Task<MoveModel> ExecuteAsync(List<Move> moves)
        {
            if (moves.Count < 1) return null;

            var move = moves.OrderBy(_ => random.Next()).FirstOrDefault();
            var allMoves = await cacheMoves.ReadCacheAsync();
            return allMoves.FirstOrDefault(m => m.Move.Equals(move));
        }

        public async Task<MoveModel> GetSelectedMoveDetails (Move move)
        {
            var allMoves = await cacheMoves.ReadCacheAsync();
            return allMoves.FirstOrDefault(m => m.Move.Equals(move));
        }
    }
}
