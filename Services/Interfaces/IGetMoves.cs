using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IGetMoves
    {
        public Task<List<MoveModel>> ExecuteAsync();
    }
}
