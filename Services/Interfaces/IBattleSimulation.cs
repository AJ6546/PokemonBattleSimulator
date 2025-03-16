using PokemonBattleSimulator.Models;

namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IBattleSimulation
    {
        public Task ExecuteAsync(List<Team> teamsList);
    }
}
