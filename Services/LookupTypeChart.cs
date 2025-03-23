using ClosedXML.Excel;
using PokemonBattleSimulator.Constants;
using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class LookupTypeChart: ILookupTypeChart
    { 
        public async Task<float> ExecuteAsync(Typing moveType, Typing defenderType)
        {
            int moveTypeIndex = (int)moveType;
            int defenderTypeIndex = (int)defenderType;


            float effectiveness = TypeEffectivenessChart.TypeChart[moveTypeIndex, defenderTypeIndex];

            return effectiveness;
        }
    }
}