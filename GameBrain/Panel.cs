namespace GameBrain
{
    public class Panel
    {
        public int PanelId { get; set; }
        public OccupationType OccupationType { get; set; }
        public Coordinates Coordinates { get; set; }

        public Ship? ship = null;

        public Panel(int row, int column)
        {
            Coordinates = new Coordinates(row, column);
            OccupationType = OccupationType.Empty;
        }

        public bool IsOccupied
        {
            get
            {
                return OccupationType == OccupationType.Battleship
                       || OccupationType == OccupationType.Destroyer
                       || OccupationType == OccupationType.Cruiser
                       || OccupationType == OccupationType.Submarine
                       || OccupationType == OccupationType.Carrier;
            }
        }
        
        public bool IsRandomAvailable
        {
            get
            {
                return (Coordinates.Row % 2 == 0 && Coordinates.Column % 2 == 0)
                       || (Coordinates.Row % 2 == 1 && Coordinates.Column % 2 == 1);
            }
        }
    }
}