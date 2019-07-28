using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Battleship.API.Model
{
    public interface IGameBoardListContainer
    {
        List<GameBoard> GameBoards { get; set; }
    }
}
