using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Models
{
    public class EnvironmentSetter
    {
        public EnvironmentEffect EnvironmentEffect { get; set; } 
        public int Duration { get; set; }
        public int DamageFactor { get; set; }
        public List<Typing> ImmuneTypes { get; set; }
        public Dictionary<Typing, double> MoveTypeMultipliers { get; set; } = new();
        public Dictionary<Typing, Dictionary<Stat, double>> TypingStatModifiers { get; set; } 
    }
}
