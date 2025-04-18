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
            if (move.StatusEffect != null && random.Next(0, 100) < move.Probability)
            {
                if (target.ActiveStatusEffect == null)
                {
                    var effect = move.StatusEffect;
                    int duration = effect.MinDuration.HasValue && effect.MaxDuration.HasValue
                        ? random.Next(effect.MinDuration.Value, effect.MaxDuration.Value + 1)
                        : int.MaxValue;

                    target.ActiveStatusEffect = new ActiveStatusEffect
                    {
                        Effect = effect,
                        RemainingTurns = duration
                    };

                    logBuilder.AppendLine($"{target.Pokemon} is now affected by {effect.Name}!");
                }
            }

            return Task.CompletedTask;
        }

        public async Task<bool> ProcessStatusEffectTurnAsync(PokemonModel pokemon, StringBuilder logBuilder)
        {
            if (pokemon.ActiveStatusEffect == null) return false; 

            var statusEffect = pokemon.ActiveStatusEffect.Effect;

            if (pokemon.ActiveStatusEffect.RemainingTurns > 0)
            {
                if (statusEffect.DamagePerTurn > 0)
                {
                    var MaxHp = (await getSelectedPokemonDetails.ExecuteAsync(pokemon.Id)).Stats.HP;
                    int damage = (int)(MaxHp * statusEffect.DamagePerTurn.Value);
                    pokemon.Stats.HP -= damage;
                    logBuilder.AppendLine($"{pokemon.Pokemon} takes {damage} damage from {statusEffect.Name}!");

                    if (pokemon.Stats.HP <= 0)
                    {
                        logBuilder.AppendLine($"{pokemon.Pokemon} fainted due to {statusEffect.Name}!");
                        return true;
                    }
                }
                pokemon.ActiveStatusEffect.RemainingTurns--;
                if (statusEffect.PreventsAction && random.NextDouble() < (statusEffect.ActionFailChance ?? 0))
                {
                    logBuilder.AppendLine($"{pokemon.Pokemon} is affected by {statusEffect.Name} and can't move!");

                    return true;
                }
            }
            else
            {
                logBuilder.AppendLine($"{pokemon.Pokemon} is no longer affected by {statusEffect.Name}!");
                pokemon.ActiveStatusEffect = null;
            }

            return false;
        }
    }
}
