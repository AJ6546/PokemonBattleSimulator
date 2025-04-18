using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Models
{
    public class StatModifier
    {
        public StatModifierType StatModifierType { get; set; }
        public int Stage { get; set; }
        public int Probability { get; set; } = 100;
        public bool IsForUser { get; set; }
    }
}
