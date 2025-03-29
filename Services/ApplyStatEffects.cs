using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class ApplyStatEffects: IApplyStatEffects
    {
        private const int StageConstant = 2;
        private const int BattleStatsStageConstant = 3;
        private const int SpeedLimitConst = 10000;

        public async Task ExecuteAsync(PokemonModel attacker,
            PokemonModel target, List<StatEffect> statEffects, StringBuilder logBuilder)
        {
            foreach (StatEffect statEffect in statEffects)
            {
                var statEffectProbability = new Random().NextDouble() * 100;

                if (statEffectProbability > statEffect.Probability)
                {
                    logBuilder.AppendLine("Stats effect did not work.");
                    continue;
                }

                var pokemon = statEffect.IsForUser ? attacker : target;

                ApplyChange(statEffect, pokemon, logBuilder);
            }
        }

        private void ApplyChange(StatEffect statEffect, PokemonModel pokemon, StringBuilder logBuilder)
        {
            var statChange = statEffect.Stage > 0 ? "increased" : "decreased";
            double oldStat = 0;

            switch (statEffect.StatEffectType)
            {
                case StatEffectType.Accuracy:
                    oldStat = pokemon.Stats.BattleStats.Accuracy;
                    pokemon.Stats.BattleStats.Accuracy *= GetStageMultiplier(statEffect.Stage, BattleStatsStageConstant);
                    break;
                case StatEffectType.Attack:
                    oldStat = pokemon.Stats.Attack;
                    pokemon.Stats.Attack *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case StatEffectType.Defense:
                    oldStat = pokemon.Stats.Defense;
                    pokemon.Stats.Defense *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case StatEffectType.Evasion:
                    oldStat = pokemon.Stats.BattleStats.Evasion;
                    pokemon.Stats.BattleStats.Evasion *= GetStageMultiplier(statEffect.Stage, BattleStatsStageConstant);
                    break;
                case StatEffectType.SpecialAttack:
                    oldStat = pokemon.Stats.SpecialAttack;
                    pokemon.Stats.SpecialAttack *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case StatEffectType.SpecialDefense:
                    oldStat = pokemon.Stats.SpecialDefense;
                    pokemon.Stats.SpecialDefense *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case StatEffectType.Speed:
                    oldStat = pokemon.Stats.Speed;
                    pokemon.Stats.Speed = Math.Min(SpeedLimitConst, pokemon.Stats.Speed * GetStageMultiplier(statEffect.Stage, StageConstant));
                    break;
            }

            logBuilder.AppendLine($"{pokemon.Pokemon}'s {statEffect.StatEffectType} is {statChange} from {Math.Round(oldStat, 2)} to {Math.Round(GetStatValue(statEffect, pokemon), 2)}.");
        }

        private double GetStatValue(StatEffect statEffect, PokemonModel pokemon)
        {
            return statEffect.StatEffectType switch
            {
                StatEffectType.Accuracy => pokemon.Stats.BattleStats.Accuracy,
                StatEffectType.Attack => pokemon.Stats.Attack,
                StatEffectType.Defense => pokemon.Stats.Defense,
                StatEffectType.Evasion => pokemon.Stats.BattleStats.Evasion,
                StatEffectType.SpecialAttack => pokemon.Stats.SpecialAttack,
                StatEffectType.SpecialDefense => pokemon.Stats.SpecialDefense,
                StatEffectType.Speed => pokemon.Stats.Speed,
                _ => 0
            };
        }

        private double GetStageMultiplier(int stage, int constant)
        {
            return (double)Math.Max(constant, constant + stage) /
                Math.Max(constant, Math.Max(1, constant - stage));
        }
    }
}
