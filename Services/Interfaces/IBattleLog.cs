namespace PokemonBattleSimulator.Services.Interfaces
{
    public interface IBattleLog
    {
        public Task WriteAsync(string message);
        public Task<string> ReadAsync();
        public Task ClearAsync();
    }
}
