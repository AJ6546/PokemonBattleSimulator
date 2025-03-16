using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class BattleSimulation: IBattleSimulation
    {
        private readonly Random random;
        private readonly ICacheMoves cacheMoves;
        private readonly ICalculateDamage calculateDamage;

        public BattleSimulation(
            ICacheMoves cacheMoves, ICalculateDamage calculateDamage)
        {
            random = new Random();
            this.cacheMoves = cacheMoves;
            this.calculateDamage = calculateDamage;
        }

        public async Task ExecuteAsync(List<Team> teamsList)
        {
            if (teamsList.Count < 2)
            {
                throw new ArgumentException("Battle requires at least two teams.");
            }

            var allPokemon = teamsList.SelectMany(team => team.Pokemon).ToList();
            List<PokemonModel> attackOrder = DetermineAttackOrder(allPokemon);

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

                if(attacker.Moves.Count < 1)
                {
                    continue;
                }

                var move = attacker.Moves.OrderBy(_ => random.Next()).FirstOrDefault();
                var allMoves = await cacheMoves.ReadCacheAsync();

                var selectedMove = allMoves.FirstOrDefault(m => m.Move.Equals(move));
                
                if (selectedMove != null)
                {
                    if (!attacker.MovesPP.Keys.Contains(selectedMove.Move))
                    {
                        attacker.MovesPP[selectedMove.Move] = selectedMove.PP;
                    }

                    var enemies = teamsList
                        .Where(team => !team.Pokemon.Contains(attacker))
                        .SelectMany(team => team.Pokemon)
                        .Where(p => p.Stats.HP > 0)
                        .ToList();

                    if (enemies.Any())
                    {
                        var target = enemies[random.Next(enemies.Count)];
                        target.Stats.HP -= Math.Max(0, await calculateDamage.ExecuteAsync(attacker, target, selectedMove));
                        attacker.MovesPP[selectedMove.Move] -= 1;

                        if (attacker.MovesPP[selectedMove.Move] <= 0)
                        {
                            attacker.Moves.Remove(selectedMove.Move);
                            attacker.MovesPP.Remove(selectedMove.Move);
                        }

                        if (target.Stats.HP <= 0)
                        {
                            attackOrder.Remove(target);
                        }
                    }
                }

                turnIndex = (turnIndex + 1) % attackOrder.Count;
                attackOrder = DetermineAttackOrder(allPokemon);
            }
        }

        private List<PokemonModel> DetermineAttackOrder(List<PokemonModel> allPokemon)
        {
            return allPokemon
                            .OrderByDescending(p => p.Stats.Speed)
                            .ThenBy(_ => random.Next())
                            .ToList();
        }

        public bool IsBattleOver(List<Team> teamsList)
        {
            var teamsWithActivePokemon = teamsList
                .Where(team => team.Pokemon.Any(pokemon => pokemon.Stats.HP > 0))
                .ToList();

            return teamsWithActivePokemon.Count <= 1;
        }
    }
}
