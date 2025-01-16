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
        private readonly IGetSelectedPokemonDetails getSelectedPokemonDetails;

        public PokemonController(ILogger<PokemonController> logger, ICachePokemon cachePokemon,
            ICacheMoves cacheMoves, IGetSelectedPokemonDetails getSelectedPokemonDetails)
        {
            this.logger = logger;
            this.cachePokemon = cachePokemon;
            this.cacheMoves = cacheMoves;
            this.getSelectedPokemonDetails = getSelectedPokemonDetails;
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

        [HttpPost]
        public async Task<IActionResult> GetMove([FromBody] string moveName)
        {
            Enum.TryParse<Move>(moveName, true, out var parsedMove);
            var allMoves = await cacheMoves.ReadCacheAsync();
            var selectedMove = allMoves.FirstOrDefault(m => m.Move.Equals(parsedMove));

            if (selectedMove == null)
            {
                logger.LogError("Selected move was not found.");
                return NotFound("Move not found");
            }

            return PartialView("_SelectedMove", selectedMove);
        }

        [HttpPost]
        public async Task<IActionResult> Battle([FromBody] List<TeamViewModel> teams)
        {
            foreach (var team in teams)
            {
                foreach(var pokemon in team.Pokemon)
                {
                    await getSelectedPokemonDetails.ExecuteAsync(pokemon);
                }
            }
            return Json(new { success = true, message = "Teams received successfully." });
        }
    }
}

