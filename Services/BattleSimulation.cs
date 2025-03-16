using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class BattleSimulation: IBattleSimulation
    {
        private readonly Random random;
        private readonly ICacheMoves cacheMoves;
        private readonly ILookupTypeChart lookupTypeChart;

        public BattleSimulation(
            ICacheMoves cacheMoves, ILookupTypeChart lookupTypeChart)
        {
            random = new Random();
            this.cacheMoves = cacheMoves;
            this.lookupTypeChart = lookupTypeChart;
        }

        public async Task ExecuteAsync(List<Team> teamsList)
        {
            if (teamsList.Count < 2)
            {
                throw new ArgumentException("Battle requires at least two teams.");
            }

            var allPokemon = teamsList.SelectMany(team => team.Pokemon).ToList();

            var attackOrder = allPokemon.OrderBy(_ => random.Next()).ToList();

            int turnIndex = 0;

            while (!IsBattleOver(teamsList))
            {
                var attacker = attackOrder[turnIndex];

                if (attacker.Stats.HP <= 0)
                {
                    attackOrder.RemoveAt(turnIndex);
                    if (attackOrder.Count == 0) break;
                    continue;
                }

                var move = attacker.Moves.OrderBy(_ => random.Next()).FirstOrDefault();
                var allMoves = await cacheMoves.ReadCacheAsync();

                var selectedMove = allMoves.FirstOrDefault(m => m.Move.Equals(move));

                if(selectedMove != null)
                {
                    var enemies = teamsList
                        .Where(team => !team.Pokemon.Contains(attacker)) 
                        .SelectMany(team => team.Pokemon)
                        .Where(p => p.Stats.HP > 0) 
                        .ToList();

                    if (enemies.Any())
                    {
                        var target = enemies[random.Next(enemies.Count)];
                        ApplyDamage(target, selectedMove);

                        if (target.Stats.HP <= 0)
                        {
                            attackOrder.Remove(target);
                        }
                    }
                }

                turnIndex = (turnIndex + 1) % attackOrder.Count;
            }
        }

        public bool IsBattleOver(List<Team> teamsList)
        {
            var teamsWithActivePokemon = teamsList
                .Where(team => team.Pokemon.Any(pokemon => pokemon.Stats.HP > 0))
                .ToList();

            return teamsWithActivePokemon.Count <= 1;
        }

        public void ApplyDamage(PokemonModel defender, MoveModel selectedMove)
        {
            int effectiveness = lookupTypeChart.ExecuteAsync(selectedMove.Typing, defender.PrimaryType);

            if (!defender.SecondaryType.Equals(Typing.None))
            {
                effectiveness *= lookupTypeChart.ExecuteAsync(selectedMove.Typing, defender.SecondaryType);
            }

            defender.Stats.HP -= Math.Max(0, defender.Stats.HP - (effectiveness * selectedMove.Power));
        }
    }
}
