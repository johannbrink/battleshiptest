using System.Linq;
using System.Net;
using Battleship.API.Extensions;
using Battleship.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace Battleship.API.Controllers
{
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly IGameBoardListContainer _gameBoardListContainer;

        public GameController(IGameBoardListContainer gameBoardListContainer)
        {
            _gameBoardListContainer = gameBoardListContainer;
        }

        /// <summary>
        /// Gets Game board based on 0 based board number
        /// </summary>
        /// <param name="boardNo">0 based board number</param>
        /// <returns></returns>
        [HttpGet]
        [Route("board")]
        public IActionResult Get(int boardNo)
        {
            if (boardNo + 1 > _gameBoardListContainer.GameBoards.Count)
                return BadRequest("Invalid boardNo");

            return Ok(_gameBoardListContainer.GameBoards[boardNo]);
        }

        /// <summary>
        /// Get ship based on 0 based board number and 0 based ship number
        /// </summary>
        /// <param name="boardNo">0 based board number</param>
        /// <param name="shipNo">0 based ship number</param>
        /// <returns>Ship</returns>
        [HttpGet]
        [Route("ship")]
        public IActionResult Get(int boardNo, int shipNo)
        {
            if (boardNo + 1 > _gameBoardListContainer.GameBoards.Count)
                return BadRequest("Invalid boardNo");

            var gameBoard = _gameBoardListContainer.GameBoards[boardNo];

            if (shipNo + 1 > gameBoard.Ships.Count)
                return BadRequest("Invalid boardNo");

            return Ok(gameBoard.Ships[shipNo]);
        }

        /// <summary>
        /// Creates a new board
        /// </summary>
        /// <returns>Ship URI</returns>
        [HttpPost]
        [Route("CreateBoard")]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        public IActionResult CreateBoard()
        {
            var board = new GameBoard();
            _gameBoardListContainer.GameBoards.Add(board);
            return Created($"/api/game/board/{_gameBoardListContainer.GameBoards.IndexOf(board)}", null);
        }

        /// <summary>
        /// Adds a ship to the specified board
        /// </summary>
        /// <param name="boardNo">0 based board number</param>
        /// <param name="startColumn">column number between 1 and 10</param>
        /// <param name="startRow">row number between 1 and 10</param>
        /// <param name="horizontal">Horizontal (true) / Vertical (false)</param>
        /// <param name="shipLength">Length between 1 and 5</param>
        [HttpPost]
        [Route("AddShip")]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public IActionResult AddShip(int boardNo, int startColumn, int startRow, bool horizontal, int shipLength)
        {
            var basicValidation = BasicValidation(boardNo, startColumn, startRow);
            if (basicValidation != null)
                return basicValidation;
            if(shipLength < 1 || shipLength > 5)
                return BadRequest("Invalid ship length");

            var gameBoard = _gameBoardListContainer.GameBoards[boardNo];
            int endColumn = startColumn, endRow = startRow;

            if (horizontal)
                endColumn = startColumn + shipLength - 1;
            else
                endRow = startRow + shipLength - 1;

            if (endRow > 10 || endColumn > 10)
                return BadRequest("Cannot place ships beyond the boundaries of the board");

            var affectedRegisters = gameBoard.PositionMatrix.Range(startRow, startColumn, endRow, endColumn);
            if (affectedRegisters.Any(x => x.IsOccupied))
                return BadRequest("Position is already occupied");

            foreach (var register in affectedRegisters)
            {
                register.OccupationType = OccupationType.Ship;
            }

            var ship = new Ship(affectedRegisters, horizontal, shipLength);
            gameBoard.Ships.Add(ship);

            return Created($"/api/game/board/{boardNo},{gameBoard.Ships.IndexOf(ship)}", null);
        }

        /// <summary>
        /// Returns whether specified column / row combination results in a hit and records the hit against the affected ship.
        /// </summary>
        /// <param name="boardNo">0 based board number</param>
        /// <param name="column">column number between 1 and 10</param>
        /// <param name="row">row number between 1 and 10</param>
        /// <returns></returns>
        [HttpPut]
        [Route("Attack")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult Attack(int boardNo, int column, int row)
        {
            var basicValidation = BasicValidation(boardNo, column, row);
            if (basicValidation != null)
                return basicValidation;

            var gameBoard = _gameBoardListContainer.GameBoards[boardNo];
            var affectedRegisters = gameBoard.PositionMatrix.Range(row, column, row, column);

            foreach (var register in affectedRegisters)
            {
                register.OccupationType = OccupationType.Hit;
            }

            return Accepted(affectedRegisters.Any(x => x.IsOccupied));
        }

        private IActionResult BasicValidation(int boardNo, int startColumn, int startRow)
        {
            if (boardNo + 1 > _gameBoardListContainer.GameBoards.Count)
                return BadRequest("Invalid board");
            if (startColumn < 1 || startColumn > 10)
                return BadRequest("Invalid column");
            if (startRow < 1 || startRow > 10)
                return BadRequest("Invalid row");
            return null;
        }
    }
}