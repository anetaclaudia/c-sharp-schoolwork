namespace Domain
{
    public class SavedGame
    {
        public int SavedGameId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Player? PlayerOne { get; set; }
        public Player? PlayerTwo { get; set; }
        public string? WinningPlayer { get; set; }
        public string? Moves { get; set; }

        // this method is used for displaying the information about  current game state
        // output example "1: test1 vs test 2 - test1 Won!" if the game has been won
        public override string ToString()
        {
            if (WinningPlayer != null)
            {
                return SavedGameId + ": " + PlayerOne!.Name + " vs " + PlayerTwo!.Name + " - " + WinningPlayer + " Won!";
            }
            return SavedGameId + ": " + PlayerOne!.Name + " vs " + PlayerTwo!.Name;
        }
    }
}