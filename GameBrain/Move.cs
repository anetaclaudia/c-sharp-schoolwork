namespace GameBrain
{
    // move is containing information that needs to be later on used for showing the game replay
    public class Move
    {
        public int MoveId { get; set; }
        public int MoveNumber { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }

        public OccupationType OccupationType { get; set; }
        public int MovePlayerId { get; set; }
    }
}