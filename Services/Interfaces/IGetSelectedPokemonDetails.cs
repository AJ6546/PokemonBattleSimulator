using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IGetSelectedPokemonDetails
    {
        public Task<PokemonModel> ExecuteAsync(int id, List<Move> moves = null);
    }
}
