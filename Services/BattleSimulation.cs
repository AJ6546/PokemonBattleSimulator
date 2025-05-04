using PokemonBattleSimulator.Contexts;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;
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
        private readonly IApplyStatModifiers applyStatModifiers;
        private readonly ITurnManager turnManager;
        private readonly IStatusEffectHandler statusEffectHandler;
        private readonly IEnvironmentHandler environmentHandler;
        private BattleContext battleContext;
        public BattleSimulation(IBattleLog battleLog, IMoveSelector moveSelector,
            IAttackExecutor attackExecutor, ITurnManager turnManager, IApplyStatModifiers applyStatModifiers,
            IStatusEffectHandler statusEffectHandler, IEnvironmentHandler environmentHandler)
        {
            random = new Random();
            this.battleLog = battleLog;
            this.moveSelector = moveSelector;
            this.attackExecutor = attackExecutor;
            this.applyStatModifiers = applyStatModifiers;
            this.turnManager = turnManager;
            this.statusEffectHandler = statusEffectHandler;
            this.environmentHandler = environmentHandler;
            battleContext = new BattleContext();
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
            List<PokemonModel> attackOrder = await turnManager.ExecuteAsync(allPokemon, battleContext.CurrentEnvironment);

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine("Attack Order - Speed");
            foreach (var pokemon in attackOrder)
            {
                logBuilder.AppendLine($"{pokemon.Pokemon.ToString()} - {pokemon.Stats.Speed}");
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

                var preventsAttack = await statusEffectHandler.ProcessStatusEffectTurnAsync(attacker, logBuilder);
                if(preventsAttack)
                {
                    turnIndex = (turnIndex + 1) % attackOrder.Count;
                    turnCounter ++;
                    continue;
                }

                if(attacker.MoveChargeTurnCounter.Any())
                {
                    var key = attacker.MoveChargeTurnCounter.First().Key;
                    attacker.MoveChargeTurnCounter[key]--;

                    if (attacker.MoveChargeTurnCounter[key] > 0)
                    {
                        turnIndex = (turnIndex + 1) % attackOrder.Count;
                        turnCounter++;
                        continue;
                    }
                    else
                    {
                        attacker.MoveChargeTurnCounter.Remove(key);
                    }
                }

                var selectedMove = await GetMoveAsync(attacker);

                if(selectedMove.Environment != null && !selectedMove.Environment
                    .EnvironmentEffect.Equals(EnvironmentEffect.None))
                {
                    await environmentHandler.ApplyEnvironmentEffect(selectedMove.Environment, 
                        attacker, battleContext);
                    if (selectedMove.StatusEffect != null)
                    {
                        await environmentHandler.ApplyStatusEffect(selectedMove, attacker, 
                            allPokemon, logBuilder);
                    }
                }

                var enemies = teamsList
                        .Where(team => !team.Pokemon.Contains(attacker))
                        .SelectMany(team => team.Pokemon)
                        .Where(p => p.Stats.HP > 0)
                        .ToList();

                if (enemies.Any())
                {
                    var target = enemies[random.Next(enemies.Count)];

                    logBuilder.AppendLine($"{attacker.Pokemon} uses {selectedMove.Move}");

                    if (selectedMove.StatModifiers != null && selectedMove.StatModifiers.Count > 0)
                    {
                        await applyStatModifiers.ExecuteAsync(attacker, target, selectedMove.StatModifiers, logBuilder);
                    }

                    if(selectedMove.Power > 0 || (selectedMove.VariablePower != null && 
                        selectedMove.VariablePower.Count > 0))
                    {
                        await attackExecutor.ExecuteAsync(attacker, target, selectedMove, battleContext, logBuilder);
                    }

                    await statusEffectHandler.ApplyStatusEffectAsync(attacker, target, selectedMove, logBuilder);

                    if (target.Stats.HP <= 0)
                    {
                        attackOrder.Remove(target);
                        allPokemon.Remove(target);
                    }
                }

                attackOrder = await turnManager.ExecuteAsync(allPokemon, battleContext.CurrentEnvironment);

                if (attackOrder.Count == 0)
                    break;

                if ((turnIndex + 1) % attackOrder.Count == 0)
                {
                    await environmentHandler.TickEnvironment(battleContext, logBuilder, allPokemon);
                }

                turnCounter++;
                turnIndex = (turnIndex + 1) % attackOrder.Count;
            }

            var winningTeam = teamsList.FirstOrDefault(team => team.Pokemon.Any(pokemon => pokemon.Stats.HP > 0));
            
            if (winningTeam != null)
            {
                logBuilder.AppendLine($"\nTeam {winningTeam.TeamId} wins\n");
                logBuilder.AppendLine("Pokemon that survived:");
                foreach (var pokemon in winningTeam.Pokemon)
                {
                    logBuilder.AppendLine(pokemon.Pokemon.ToString());
                }
            }

            logBuilder.AppendLine($"\nCombat Detail\n");

            foreach(var team in teamsList)
            {
                logBuilder.AppendLine($"Team {team.TeamId}");
                foreach(var pokemon in team.Pokemon)
                {
                    logBuilder.AppendLine($"Pokemon: {pokemon.Pokemon}")
                        .AppendLine($"Damage Dealt: {pokemon.CombatDetails.DamageDealt}")
                        .AppendLine($"KnockOuts: {pokemon.CombatDetails.KnockOutCount}");
                        if (pokemon.CombatDetails.KnockedOutPokemon.Count > 0)
                        {
                            logBuilder.AppendLine($"Knocked out Pokémon: " +
                                $"{string.Join(", ", pokemon.CombatDetails.KnockedOutPokemon)}");
                        }
                    logBuilder.AppendLine();
                }
                logBuilder.AppendLine();
            }
            var mvp = teamsList
                .SelectMany(team => team.Pokemon, (team, pokemon) => new { TeamId = team.TeamId, Pokemon = pokemon })
                .OrderByDescending(entry => entry.Pokemon.CombatDetails.KnockOutCount)
                .ThenByDescending(entry => entry.Pokemon.CombatDetails.DamageDealt)   
                .ThenByDescending(entry => entry.TeamId.Equals(winningTeam?.TeamId))
                .FirstOrDefault();

            if (mvp != null && mvp.Pokemon.CombatDetails.KnockOutCount > 0)
            {
                logBuilder.AppendLine($"MVP: {mvp.Pokemon.Pokemon} (Knockouts: {mvp.Pokemon.CombatDetails.KnockOutCount}) " +
                    $"- Team {mvp.TeamId}");
            }

            logBuilder.AppendLine($"\nBattle duration: {stopwatch.ElapsedMilliseconds} ms.");

            await battleLog.WriteAsync(logBuilder.ToString());
        }

        private async Task<MoveModel> GetMoveAsync(PokemonModel attacker)
        {
            var selectedMove = await moveSelector.ExecuteAsync(attacker.Moves);
            if (selectedMove.RequiresCharging)
            {
                attacker.MoveChargeTurnCounter[selectedMove.Move] = selectedMove.ChargeTurnsRequired;
            }
            return selectedMove;
        }

        public bool IsBattleOver(List<Team> teamsList)
        {
            return teamsList.Count(team => team.Pokemon.Any(pokemon => pokemon.Stats.HP > 0)) <= 1;
        }
    }
}
