using ClosedXML.Excel;
using PokemonBattleSimulator.Models.Enum;
using PokemonBattleSimulator.Services.Interfaces;

namespace PokemonBattleSimulator.Services
{
    public class LookupTypeChart: ILookupTypeChart
    { 
        public int ExecuteAsync(Typing moveType, Typing defenderType)
        {
            string binDirectory = AppDomain.CurrentDomain.BaseDirectory;
            int binIndex = binDirectory.IndexOf("bin", StringComparison.OrdinalIgnoreCase);
            var directory = binDirectory.Substring(0, binIndex);

            var filePath = Path.Combine(directory, "Resources", "PokemonTypeChart.xlsx");

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(1);

                var moveTypeRow = FindRowForType(worksheet, moveType);
                var defenderTypeColumn = FindColumnForType(worksheet, defenderType);

                int effectiveness = worksheet.Cell(moveTypeRow, defenderTypeColumn).GetValue<int>();

                return effectiveness;
            } 
        }

        private int FindRowForType(IXLWorksheet worksheet, Typing type)
        {
            for (int i = 2; i <= worksheet.LastRowUsed().RowNumber(); i++)
            {
                if (worksheet.Cell(i, 1).GetValue<string>() == type.ToString())
                {
                    return i; 
                }
            }
            return -1; 
        }

        private int FindColumnForType(IXLWorksheet worksheet, Typing type)
        {
            for (int j = 2; j <= worksheet.LastColumnUsed().ColumnNumber(); j++)
            {
                if (worksheet.Cell(1, j).GetValue<string>() == type.ToString())
                {
                    return j; 
                }
            }
            return -1; 
        }
    }
}