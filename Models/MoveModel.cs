using PokemonBattleSimulator.Models.Enum;

namespace PokemonBattleSimulator.Models
{
    public class MoveModel
    {
        public int Id { get; set; }
        public Move Move { get; set; }
        public Typing Typing { get; set; }
        public Category Category { get; set; }
        public int Power { get; set; }
        public List<int> VariablePower { get; set; }
        public int Accuracy { get; set; }
        public int PP { get; set; }
        public List<StatEffect> StatEffects { get; set; }
        public string Description { get; set; }
    }
}
