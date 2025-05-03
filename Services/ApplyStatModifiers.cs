using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class ApplyStatModifiers: IApplyStatModifiers
    {
        private const int StageConstant = 2;
        private const int BattleStatsStageConstant = 3;
        private const int SpeedLimitConst = 10000;

        public async Task ExecuteAsync(PokemonModel attacker,
            PokemonModel target, List<StatModifier> statModifiers, StringBuilder logBuilder)
        {
            foreach (StatModifier statEffect in statModifiers)
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

        public async Task<double> GetEffectiveStat(Stat stat, PokemonModel pokemon,
            EnvironmentSetter environment)
        {
            double baseStat = GetStatValue(stat, pokemon);
            var types = new[] { pokemon.PrimaryType, pokemon.SecondaryType };

            foreach (var type in types)
            {
                if (type.Equals(pokemon.PrimaryType)) continue;

                if (environment?.TypingStatModifiers != null &&
                    environment.TypingStatModifiers.TryGetValue(type, out var statModifier) &&
                    statModifier.TryGetValue(stat, out var modifier))
                {
                    baseStat *= modifier;
                }
            }
            return baseStat;
        }

        private void ApplyChange(StatModifier statEffect, PokemonModel pokemon, StringBuilder logBuilder)
        {
            var statChange = statEffect.Stage > 0 ? "increased" : "decreased";
            double oldStat = 0;

            switch (statEffect.StatModifierType)
            {
                case Stat.Accuracy:
                    oldStat = pokemon.Stats.BattleStats.Accuracy;
                    pokemon.Stats.BattleStats.Accuracy *= GetStageMultiplier(statEffect.Stage, BattleStatsStageConstant);
                    break;
                case Stat.Attack:
                    oldStat = pokemon.Stats.Attack;
                    pokemon.Stats.Attack *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case Stat.Defense:
                    oldStat = pokemon.Stats.Defense;
                    pokemon.Stats.Defense *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case Stat.Evasion:
                    oldStat = pokemon.Stats.BattleStats.Evasion;
                    pokemon.Stats.BattleStats.Evasion *= GetStageMultiplier(statEffect.Stage, BattleStatsStageConstant);
                    break;
                case Stat.SpecialAttack:
                    oldStat = pokemon.Stats.SpecialAttack;
                    pokemon.Stats.SpecialAttack *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case Stat.SpecialDefense:
                    oldStat = pokemon.Stats.SpecialDefense;
                    pokemon.Stats.SpecialDefense *= GetStageMultiplier(statEffect.Stage, StageConstant);
                    break;
                case Stat.Speed:
                    oldStat = pokemon.Stats.Speed;
                    pokemon.Stats.Speed = Math.Min(SpeedLimitConst, pokemon.Stats.Speed * GetStageMultiplier(statEffect.Stage, StageConstant));
                    break;
            }

            logBuilder.AppendLine($"{pokemon.Pokemon}'s {statEffect.StatModifierType} is {statChange} from {Math.Round(oldStat, 2)} to {Math.Round(GetStatValue(statEffect.StatModifierType, pokemon), 2)}.");
        }

        private double GetStatValue(Stat statEffect, PokemonModel pokemon)
        {
            return statEffect switch
            {
                Stat.Accuracy => pokemon.Stats.BattleStats.Accuracy,
                Stat.Attack => pokemon.Stats.Attack,
                Stat.Defense => pokemon.Stats.Defense,
                Stat.Evasion => pokemon.Stats.BattleStats.Evasion,
                Stat.SpecialAttack => pokemon.Stats.SpecialAttack,
                Stat.SpecialDefense => pokemon.Stats.SpecialDefense,
                Stat.Speed => pokemon.Stats.Speed,
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
