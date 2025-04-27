namespace PokemonBattleSimulator.Models
{
    public class StatusEffect
    {
        public string Name { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public double? DamagePerTurn { get; set; }
        public bool PreventsAction { get; set; }
        public double? ActionFailChance { get; set; }
        public bool HealsAttacker { get; set; }
        public double HealFraction { get; set; } = 1;
        public bool IsMajorStatus { get; set; }
    }
}
