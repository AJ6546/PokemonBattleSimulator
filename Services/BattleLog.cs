using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class BattleLog: IBattleLog
    {
        private readonly string filePath;

        public BattleLog()
        {
           filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "battlelog.txt");
        }

        public async Task WriteAsync(string message)
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true, Encoding.UTF8))
            {
                await writer.WriteLineAsync(message);
            }
        }

        public async Task<string> ReadAsync()
        {
            if (!File.Exists(filePath))
            {
                return string.Empty;
            }

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task ClearAsync()
        {
            if (File.Exists(filePath))
            {
                await File.WriteAllTextAsync(filePath, string.Empty);
            }
        }
    }
}
