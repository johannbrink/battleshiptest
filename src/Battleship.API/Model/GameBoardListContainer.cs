using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleship.API.Model
{
    public class GameBoardListContainer: IGameBoardListContainer
    {
        public List<GameBoard> GameBoards { get; set; }

        public GameBoardListContainer()
        {
            GameBoards = new List<GameBoard>();
        }
    }
}
