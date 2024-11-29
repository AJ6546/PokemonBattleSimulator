using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokemonBattleSimulator.Services
{
    public class GetPokemon: IGetPokemon
    {
        private readonly string filePath;

        public GetPokemon()
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "pokemon.json");
        }

        public async Task<List<PokemonModel>> ExecuteAsync()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Pokemon JSON file not found.", filePath);
            }

            var jsonData = await File.ReadAllTextAsync(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            var pokemonList = JsonSerializer.Deserialize<List<PokemonModel>>(jsonData, options);


            return pokemonList;
        }
    }
}
