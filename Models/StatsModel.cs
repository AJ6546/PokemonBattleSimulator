namespace PokemonBattleSimulator.Models
{
    public class StatsModel
    {
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpecialAttack { get; set; }
        public int SpecialDefense { get; set; }
        public int Speed { get; set; }
        public int Total { get; set; }

        public BattleStats BattleStats { get; set; }
    }
}
