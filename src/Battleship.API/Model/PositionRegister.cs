namespace Battleship.API.Model
{
    public class PositionRegister
    {
        public OccupationType OccupationType { get; set; }
        public Coordinate Coordinate { get; set; }

        public PositionRegister(int row, int column)
        {
            Coordinate = new Coordinate(row, column);
            OccupationType = OccupationType.Empty;
        }

        public bool IsOccupied => OccupationType != OccupationType.Empty;

    }
}
