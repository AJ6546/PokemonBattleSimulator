using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Models
{
    public class PokemonModel
    {
        public int Id { get; set; }
        public Pokemon Pokemon { get; set; }
        public Typing PrimaryType { get; set; }
        public Typing SecondaryType { get; set; }
        public StatsModel Stats { get; set; }
        public List<Move> Moves { get; set; }
        public Dictionary<Move, int> MovesPP = new Dictionary<Move, int>();
        public CombatDetails CombatDetails = new CombatDetails();
    }
}
