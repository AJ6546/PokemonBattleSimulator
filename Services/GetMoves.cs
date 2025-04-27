using PokemonBattleSimulator.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class GetMoves: IGetMoves
    {
        private readonly string filePath;

        public GetMoves()
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "moves.json");
        }

        public async Task<List<MoveModel>> ExecuteAsync()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Pokemon JSON file not found.", filePath);
            }

            var jsonData = await File.ReadAllTextAsync(filePath);

            var options = new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            var movesList = JsonSerializer.Deserialize<List<MoveModel>>(jsonData, options);

            return movesList;
        }
    }
}
