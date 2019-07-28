
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Battleship.API.Model
{
    public class Ship
    {
        public List<PositionRegister> PositionRegisters { get; }
        public bool Horizontal { get; }
        public int Length { get; }

        public Ship(List<PositionRegister> positionRegisters, bool horizontal, int length)
        {
            PositionRegisters = positionRegisters;
            Horizontal = horizontal;
            Length = length;
        }

        public bool Sunk => PositionRegisters.TrueForAll(x => x.OccupationType == OccupationType.Hit);
    }
}