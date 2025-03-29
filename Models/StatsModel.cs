namespace PokemonBattleSimulator.Models
{
    public class StatsModel
    {
        public double HP { get; set; }
        public double Attack { get; set; }
        public double Defense { get; set; }
        public double SpecialAttack { get; set; }
        public double SpecialDefense { get; set; }
        public double Speed { get; set; }
        public double Total { get; set; }

        public BattleStats BattleStats = new BattleStats();
    }
}
