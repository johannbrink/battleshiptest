using System.Collections.Generic;
using System.Linq;
using Battleship.API.Model;

namespace Battleship.API.Extensions
{
    public static class PositionMatrixExtension
    {
        public static PositionRegister At(this List<PositionRegister> panels, int row, int column)
        {
            return panels.First(x => x.Coordinate.Row == row && x.Coordinate.Column == column);
        }

        public static List<PositionRegister> Range(this List<PositionRegister> panels, int startRow, int startColumn,
            int endRow, int endColumn)
        {
            return panels.Where(x => x.Coordinate.Row >= startRow
                                     && x.Coordinate.Column >= startColumn
                                     && x.Coordinate.Row <= endRow
                                     && x.Coordinate.Column <= endColumn).ToList();
        }
    }
}
