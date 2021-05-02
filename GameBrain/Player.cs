using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GameBrain
{
    public class Player
    {
        public int PlayerId { get; set; }
        [MaxLength(128)] public string Name { get; set; } = null!;
        public ConsoleColor Color { get; set; }
        public int SelectedWidth = 0; // this variable is just meant for the console game
        public int SelectedHeight = 0; // same goes for this variable

        public List<Panel> Panels = new List<Panel>();
        
        // this holds the information about the panels at the beginning of the game
        // this field is important when user wants to replay the moves for a game
        public List<Panel> StartingPanels = new List<Panel>(); 

        public List<Ship> Ships = new List<Ship>()
        {
            new Ship(ShipType.Battleship),
            new Ship(ShipType.Carrier),
            new Ship(ShipType.Destroyer),
            new Ship(ShipType.Submarine),
            new Ship(ShipType.Cruiser)
        };

        public void InitPanels(int height, int width)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Panels.Add(new Panel(i, j));
                }
            }
        }

        public bool HasPlacedAllShips()
        {
            return Ships.All(ship => ship.Width == ship.Panels.Count);
        }

        public void PlaceShipsRandom(int height, int width)
        {
            //Random class creation from http://stackoverflow.com/a/18267477/106356
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            foreach (var ship in Ships)
            {
                //Select a random row/column combination, then select a random orientation.
                //If none of the proposed panels are occupied, place the ship
                //Do this for all ships

                bool isOpen = true;
                while (isOpen)
                {
                    var startcolumn = rand.Next(0, width);
                    var startrow = rand.Next(0, height);
                    int endrow = startrow, endcolumn = startcolumn;
                    var orientation = rand.Next(1, 101) % 2; //0 for Horizontal

                    if (orientation == 0)
                    {
                        endrow += ship.Width - 1;
                    }
                    else
                    {
                        endcolumn += ship.Width - 1;
                    }

                    //We cannot place ships beyond the boundaries of the board
                    if (endrow >= height || endcolumn >= width)
                    {
                        continue;
                    }

                    //Check if specified panels are occupied
                    var affectedPanels = Panels.Range(startrow, startcolumn, endrow, endcolumn);
                    if (affectedPanels.Any(x => x.IsOccupied))
                    {
                        continue;
                    }

                    ship.Panels = affectedPanels;
                    foreach (var panel in affectedPanels)
                    {
                        panel.OccupationType = ship.GetOccupationType();
                        panel.ship = ship;
                    }

                    isOpen = false; // ends while loop
                }
            }
        }
    }
}