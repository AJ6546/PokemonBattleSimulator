using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class StatusEffectHandler: IStatusEffectHandler
    {
        private readonly Random random;
        private readonly IGetSelectedPokemonDetails getSelectedPokemonDetails;

        public StatusEffectHandler(IGetSelectedPokemonDetails getSelectedPokemonDetails)
        {
            random = new();
            this.getSelectedPokemonDetails = getSelectedPokemonDetails;
        }

        public Task ApplyStatusEffectAsync(PokemonModel attacker, PokemonModel target, MoveModel move, StringBuilder logBuilder)
        {
            var effect = move.StatusEffect;
            if (effect != null && IsStatusEffectTriggered(move.Probability))
            {
                if (effect.IsMajorStatus)
                {
                    var oldStatusEffect = target.ActiveStatusEffects.FirstOrDefault(e => e.Effect.IsMajorStatus);
                    target.ActiveStatusEffects.RemoveAll(e => e.Effect.IsMajorStatus);

                    AddEffect(attacker, target, effect);

                    if (oldStatusEffect != null)
                    {
                        logBuilder.AppendLine($"{target.Pokemon} is no longer affected by {oldStatusEffect.Effect.Name}!");
                    }

                    logBuilder.AppendLine($"{target.Pokemon} is now affected by {effect.Name}!");
                }
                else
                {
                    if (target.ActiveStatusEffects.All(e => e.Effect != effect))
                    {
                        AddEffect(attacker, target, effect);

                        logBuilder.AppendLine($"{target.Pokemon} is now affected by {effect.Name}!");
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool IsStatusEffectTriggered(int probability) => 
            random.Next(0, 100) < probability;

        private void AddEffect(PokemonModel attacker, PokemonModel target, StatusEffect effect)
        {
            int duration = effect.MinDuration.HasValue && effect.MaxDuration.HasValue
                ? random.Next(effect.MinDuration.Value, effect.MaxDuration.Value + 1)
                : int.MaxValue;

            target.ActiveStatusEffects.Add(new ActiveStatusEffect
            {
                Effect = effect,
                RemainingTurns = duration,
                SourcePokemon = attacker
            });
        }

        public async Task<bool> ProcessStatusEffectTurnAsync(PokemonModel pokemon, StringBuilder logBuilder)
        {
            if (!pokemon.ActiveStatusEffects.Any()) return false;
            var preventsAction = false;

            foreach (var activeStatusEffect in pokemon.ActiveStatusEffects)
            {
                var statusEffect = activeStatusEffect.Effect;

                if (activeStatusEffect.RemainingTurns > 0)
                {
                    if (statusEffect.DamagePerTurn > 0)
                    {
                        var MaxHp = (await getSelectedPokemonDetails.ExecuteAsync(pokemon.Id)).Stats.HP;
                        int damage = (int)(MaxHp * statusEffect.DamagePerTurn.Value);
                        pokemon.Stats.HP -= damage;
                        logBuilder.AppendLine($"{pokemon.Pokemon} takes {damage} damage from {statusEffect.Name}!");

                        if(statusEffect.HealsAttacker && activeStatusEffect.SourcePokemon != null)
                        {
                            var attackerMaxHp = (await getSelectedPokemonDetails.ExecuteAsync(activeStatusEffect.SourcePokemon.Id)).Stats.HP;
                            activeStatusEffect.SourcePokemon.Stats.HP = Math.Min(attackerMaxHp, activeStatusEffect.SourcePokemon.Stats.HP
                                + (int)(MaxHp * statusEffect.HealFraction));

                            logBuilder.AppendLine($"{activeStatusEffect.SourcePokemon.Pokemon} is healed by HP from {statusEffect.Name}!");
                        }

                        if (pokemon.Stats.HP <= 0)
                        {
                            logBuilder.AppendLine($"{pokemon.Pokemon} fainted due to {statusEffect.Name}!");
                            return true;
                        }
                    }

                    if (statusEffect.PreventsAction && random.NextDouble() < (statusEffect.ActionFailChance ?? 0))
                    {
                        logBuilder.AppendLine($"{pokemon.Pokemon} is affected by {statusEffect.Name} and can't move!");
                        preventsAction = true;
                    }

                    activeStatusEffect.RemainingTurns--;
                }
                else
                {
                    logBuilder.AppendLine($"{pokemon.Pokemon} is no longer affected by {statusEffect.Name}!");
                    pokemon.ActiveStatusEffects.Remove(activeStatusEffect);
                }
            }

            return preventsAction;
        }
    }
}
