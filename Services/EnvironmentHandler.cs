using PokemonBattleSimulator.Contexts;
using PokemonBattleSimulator.Models;
using PokemonBattleSimulator.Services.Interfaces;
using System.Text;

namespace PokemonBattleSimulator.Services
{
    public class EnvironmentHandler: IEnvironmentHandler
    {
        private readonly IGetSelectedPokemonDetails getSelectedPokemonDetails;

        public EnvironmentHandler(IGetSelectedPokemonDetails getSelectedPokemonDetails)
        {
            this.getSelectedPokemonDetails = getSelectedPokemonDetails;
        }

        public async Task ApplyEnvironmentEffect(EnvironmentSetter environmentSetter , PokemonModel user, BattleContext context)
        {
            context.CurrentEnvironment = environmentSetter;
            context.SourcePokemon = user;
        }

        public async Task TickEnvironment(BattleContext context, StringBuilder logBuilder, List<PokemonModel> allPokemon)
        {
            if (context.CurrentEnvironment != null && context.CurrentEnvironment.Duration > 0)
            {
                context.CurrentEnvironment.Duration--;

                foreach(var pokemon in allPokemon) 
                {
                    var primaryImmune = context.CurrentEnvironment.ImmuneTypes.Contains(pokemon.PrimaryType);
                    var secondaryImmune = context.CurrentEnvironment.ImmuneTypes.Contains(pokemon.SecondaryType);

                    if (!primaryImmune && !secondaryImmune)
                    {
                        var MaxHp = (await getSelectedPokemonDetails.ExecuteAsync(pokemon.Id)).Stats.HP;
                        pokemon.Stats.HP = Math.Max(0, pokemon.Stats.HP - MaxHp * context.CurrentEnvironment.DamageFactor);
                        logBuilder?.AppendLine($"{pokemon.Pokemon} was hurt by {context.CurrentEnvironment.EnvironmentEffect}.");
                    }
                }

                if (context.CurrentEnvironment.Duration == 0)
                {
                    logBuilder?.AppendLine($"The {context.CurrentEnvironment} effect has worn off.");
                    context.CurrentEnvironment = null;
                    context.SourcePokemon = null;
                }
            }
        }
    }
}
