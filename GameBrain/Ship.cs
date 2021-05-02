using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameBrain
{
    public class Ship
    {
        public int ShipId { get; set; }
        public ShipType ShipType { get; set; }
        public int Width { get; set; }

        [JsonIgnore] public List<Panel> Panels = new List<Panel>(); // jsonignore is meant for avoiding infinite loop

        public bool IsSunk()
        {
            return Panels.All(panel => !panel.IsOccupied);
        }

        public Ship()
        {
        }

        public bool HasBeenPlaced()
        {
            return Width == Panels.Count;
        }

        public Ship(ShipType shipType)
        {
            ShipType = shipType;
            Width = (int) shipType;
        }

        public OccupationType GetOccupationType()
        {
            switch (ShipType)
            {
                case ShipType.Battleship:
                    return OccupationType.Battleship;
                case ShipType.Cruiser:
                    return OccupationType.Cruiser;
                case ShipType.Destroyer:
                    return OccupationType.Destroyer;
                case ShipType.Submarine:
                    return OccupationType.Submarine;
                case ShipType.Carrier:
                    return OccupationType.Carrier;
                default:
                    return OccupationType.Empty;
            }
        }
    }
}