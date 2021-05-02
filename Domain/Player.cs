using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Domain
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string? Name { get; set; }
        public string? PanelsAndShips { get; set; }
        public string? StartingPanelsAndShips { get; set; } // this holds the information about the game state in the beginning
    }
}