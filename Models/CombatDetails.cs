using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Models
{
    public class CombatDetails
    {
        public int DamageDealt { get; set; }
        public int KnockOutCount { get; set; }
        public List<Pokemon> KnockedOutPokemon = new List<Pokemon>();
    }
}
