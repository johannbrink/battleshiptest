using System.Collections.Generic;

namespace Battleship.API.Model
{
    public class GameBoard
    {
        public int Id { get; set; }
        public List<PositionRegister> PositionMatrix { get; set; }
        public List<Ship> Ships { get; set; }

        public GameBoard()
        {
            PositionMatrix = new List<PositionRegister>();
            Ships = new List<Ship>();

            for (var i = 1; i <= 10; i++)
            {
                for (var j = 1; j <= 10; j++)
                {
                    PositionMatrix.Add(new PositionRegister(i, j));
                }
            }
        }
    }
}
