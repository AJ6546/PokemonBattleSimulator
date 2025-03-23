using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Diagnostics;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class BattleSimulation: IBattleSimulation
    {
        private readonly Random random;
        private readonly IBattleLog battleLog;
        private readonly IMoveSelector moveSelector;
        private readonly IAttackExecutor attackExecutor;
        private readonly ITurnManager turnManager;

        public BattleSimulation(IBattleLog battleLog, IMoveSelector moveSelector,
            IAttackExecutor attackExecutor, ITurnManager turnManager)
        {
            random = new Random();
            this.battleLog = battleLog;
            this.moveSelector = moveSelector;
            this.attackExecutor = attackExecutor;
            this.turnManager = turnManager;
        }

        public async Task ExecuteAsync(List<Team> teamsList)
        {
            if (teamsList.Count < 2)
            {
                throw new ArgumentException("Battle requires at least two teams.");
            }

            await battleLog.ClearAsync();

            var stopwatch = Stopwatch.StartNew();

            var allPokemon = teamsList.SelectMany(team => team.Pokemon).ToList();
            List<PokemonModel> attackOrder = turnManager.ExecuteAsync(allPokemon);

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine("Attack Order:\n");
            foreach (var pokemon in attackOrder)
            {
                logBuilder.AppendLine(pokemon.Pokemon.ToString());
            }

            int turnIndex = 0;
            int turnCounter = 1;

            while (!IsBattleOver(teamsList))
            {
                logBuilder.AppendLine($"\nTurn {turnCounter} begins\n");

                var attacker = attackOrder[turnIndex];

                if (attacker.Stats.HP <= 0)
                {
                    allPokemon.Remove(attacker);
                    attackOrder.RemoveAt(turnIndex);
                    if (attackOrder.Count == 0) break;
                    continue;
                }


                var selectedMove = await moveSelector.ExecuteAsync(attacker.Moves);
                if (selectedMove == null) continue;

                var enemies = teamsList
                        .Where(team => !team.Pokemon.Contains(attacker))
                        .SelectMany(team => team.Pokemon)
                        .Where(p => p.Stats.HP > 0)
                        .ToList();

                if (enemies.Any())
                {
                    var target = enemies[random.Next(enemies.Count)];
                    await attackExecutor.ExecuteAsync(attacker, target, selectedMove, logBuilder);

                    if (target.Stats.HP <= 0)
                    {
                        attackOrder.Remove(target);
                        allPokemon.Remove(target);
                    }
                }

                attackOrder = turnManager.ExecuteAsync(allPokemon);
                turnCounter++;
                turnIndex = (turnIndex + 1) % attackOrder.Count;
            }

            foreach (var team in teamsList)
            {
                team.Pokemon = team.Pokemon.Where(pokemon => pokemon.Stats.HP > 0).ToList();
            }

            var winningTeam = teamsList.FirstOrDefault(team => team.Pokemon.Any());
            if (winningTeam != null)
            {
                logBuilder.AppendLine($"\nTeam {winningTeam.TeamId} wins\n");
                logBuilder.AppendLine("Pokemon that survived:");
                foreach (var pokemon in winningTeam.Pokemon)
                {
                    logBuilder.AppendLine(pokemon.Pokemon.ToString());
                }
            }

            logBuilder.AppendLine($"\nBattle duration: {stopwatch.ElapsedMilliseconds} ms.");

            await battleLog.WriteAsync(logBuilder.ToString());
        }

        public bool IsBattleOver(List<Team> teamsList)
        {
            return teamsList.Count(team => team.Pokemon.Any(pokemon => pokemon.Stats.HP > 0)) <= 1;
        }
    }
}
