using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Models
{
    public class MoveModel
    {
        public int Id { get; set; }
        public Move Move { get; set; }
        public Typing Typing { get; set; }
        public Category Category { get; set; }
        public int MinTimesHit { get; set; } = 1;
        public int MaxTimesHit { get; set; }
        public bool RequiresCharging { get; set; }
        public int ChargeTurnsRequired { get; set; }
        public int Power { get; set; }
        public List<int> VariablePower { get; set; }
        public double UserHealthModifier { get; set; } = 0;
        public int Accuracy { get; set; }
        public int PP { get; set; }
        public List<StatModifier> StatModifiers { get; set; }
        public StatusEffect StatusEffect { get; set; }
        public EnvironmentSetter Environment { get; set; }
        public string Description { get; set; }
        public int Probability { get; set; }
    }
}
