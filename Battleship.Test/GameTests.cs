using Battleship.API.Model;
using Microsoft.AspNetCore.Mvc;
using Battleship.API.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Battleship.Test
{
    [TestClass]
    public class GameTests
    {
        private GameController _gameController;

        [TestInitialize]
        public void SetupTests()
        {
            _gameController = new GameController(new GameBoardListContainer());
            _gameController.CreateBoard();
        }

        [TestMethod]
        public void Can_Get_BadRequest_From_GetBoard_If_There_Are_No_Boards()
        {
            _gameController = new GameController(new GameBoardListContainer());
            var getResult = _gameController.Get(0, 0);
            Assert.IsInstanceOfType(getResult, typeof(BadRequestObjectResult));
            Assert.AreEqual(400, ((BadRequestObjectResult) getResult).StatusCode);
        }

        [TestMethod]
        public void Can_Create_Board_Successfully()
        {
            var createBoardResult = _gameController.CreateBoard();
            Assert.IsInstanceOfType(createBoardResult, typeof(CreatedResult));
            Assert.AreEqual(201, ((CreatedResult)createBoardResult).StatusCode);
        }

        [TestMethod]
        public void Can_GetBoard()
        {
            var getBoardResult = _gameController.Get(0);
            Assert.IsInstanceOfType(getBoardResult, typeof(OkObjectResult));
            Assert.AreEqual(200, ((OkObjectResult) getBoardResult).StatusCode);
        }

        [TestMethod]
        public void Attack_In_UnoccupiedPosition_Yields_False()
        {
            var attackResult = _gameController.Attack(0, 10, 10);
            Assert.IsInstanceOfType(attackResult, typeof(AcceptedResult));
            Assert.AreEqual(false, ((AcceptedResult) attackResult).Value);
        }

        public void Can_AddShip()
        {
            var addShipResult = _gameController.AddShip(0, 1, 1, true, 2);
            Assert.IsInstanceOfType(addShipResult, typeof(CreatedResult));
            Assert.AreEqual(201, ((CreatedResult)addShipResult).StatusCode);
        }

        public void Can_Get_Added_Ship()
        {
            var addShipResult = _gameController.AddShip(0, 1, 1, true, 2);
            Assert.IsInstanceOfType(addShipResult, typeof(CreatedResult));
            Assert.AreEqual(201, ((CreatedResult)addShipResult).StatusCode);

            var getAddedShipResult = _gameController.Get(0, 0);
            Assert.IsInstanceOfType(getAddedShipResult, typeof(OkObjectResult));
            Assert.AreEqual(200, ((OkObjectResult)getAddedShipResult).StatusCode);
        }

        [TestMethod]
        public void Attack_In_All_Occupied_Positions_Yields_True_And_Sunk_Ship_Horizontal()
        {
            var addShipResult = _gameController.AddShip(0, 1, 1, true, 2);
            var getAddedShipResult = _gameController.Get(0, 0);
            var addedShip = (Ship) ((OkObjectResult) getAddedShipResult).Value;
            Assert.AreEqual(false, addedShip.Sunk); //Ship should not be sunk after no hits;
            
            _gameController.Attack(0, 1, 1); //Column 1
            var getAttack1Ship = _gameController.Get(0, 0);
            var attack1Ship = (Ship) ((OkObjectResult) getAttack1Ship).Value;
            Assert.AreEqual(false, attack1Ship.Sunk); //Ship should not be sunk after 1 hit;

            _gameController.Attack(0, 2, 1); //Column 2
            var getAttack2Ship = _gameController.Get(0, 0);
            var attack2Ship = (Ship) ((OkObjectResult) getAttack2Ship).Value;
            Assert.AreEqual(true, attack2Ship.Sunk); //Ship should now be sunk after 2 hit;
        }

        [TestMethod]
        public void Attack_In_All_Occupied_Positions_Yields_True_And_Sunk_Ship_Vertical()
        {
            var addShipResult = _gameController.AddShip(0, 1, 1, false, 2);
            var getAddedShipResult = _gameController.Get(0, 0);
            var addedShip = (Ship)((OkObjectResult)getAddedShipResult).Value;
            Assert.AreEqual(false, addedShip.Sunk); //Ship should not be sunk after no hits;

            _gameController.Attack(0, 1, 1); //Row 1
            var getAttack1Ship = _gameController.Get(0, 0);
            var attack1Ship = (Ship)((OkObjectResult)getAttack1Ship).Value;
            Assert.AreEqual(false, attack1Ship.Sunk); //Ship should not be sunk after 1 hit;

            _gameController.Attack(0, 1, 2); //Row 2
            var getAttack2Ship = _gameController.Get(0, 0);
            var attack2Ship = (Ship)((OkObjectResult)getAttack2Ship).Value;
            Assert.AreEqual(true, attack2Ship.Sunk); //Ship should now be sunk after 2 hit;
        }
    }
}
