using Microsoft.AspNetCore.Mvc;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Controllers
{
    public class PokemonController : Controller
    {
        private readonly ILogger<PokemonController> logger;
        private readonly ICachePokemon cachePokemon;
        private readonly ICacheMoves cacheMoves;
        public PokemonController(ILogger<PokemonController> logger, ICachePokemon cachePokemon, ICacheMoves cacheMoves)
        {
            this.logger = logger;
            this.cachePokemon = cachePokemon;
            this.cacheMoves = cacheMoves;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult RenderPokemonInput(int teamId)
        {
            var team = new TeamViewModel { TeamId = teamId };
            return PartialView("_Team", team);
        }

        [HttpPost]
        public async Task<IActionResult> GetPokemon([FromBody] int pokemon)
        {
            var allPokemon = await cachePokemon.ReadCacheAsync();
            var selectedPokemon = allPokemon.FirstOrDefault(p => p.Pokemon.Equals((Pokemon)pokemon));

            if (selectedPokemon == null)
            {
                logger.LogError("Selected pokemon was not found.");
                return NotFound("Pokemon not found");
            }

            var allMoves = await cacheMoves.ReadCacheAsync();

            var pokemonMoves = allMoves
                .Where(moveModel => selectedPokemon.Moves.Contains(moveModel.Move))
                .ToList();

            return PartialView("_Pokemon", selectedPokemon);
        }
    }
}

// var moves = await cacheMoves.ReadCacheAsync();
