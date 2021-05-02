using System;
using System.Collections.Generic;

namespace GameBrain
{
    public class Game
    {
        public int GameId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int ShipCount { get; set; }
        public int RandomBoard { get; set; }
        
        public Player? PlayerOne = new Player();
        public Player? PlayerTwo = new Player();
        public string? WinningPlayer = null;

        public List<Move> Moves = new List<Move>();
        public int MoveCount = 0;

        public Game()
        {
            ShipCount = 5; 
            Height = 5;
            Width = 5;
            RandomBoard = 0;
        }

        public void AddMove(Move move)
        {
            Moves.Add(move);
        }

        public string InitGame()
        {
            InitPlayers();
            return "";
        }

        public string? IsGameOver()
        {
            int playerOneCount = 0;
            int playerTwoCount = 0;
            
            // counting how many ships are blown up for each player
            foreach (var ship in PlayerOne!.Ships)
            {
                if (ship.IsSunk())
                {
                    playerOneCount++;
                }
            }

            foreach (var ship in PlayerTwo!.Ships)
            {
                if (ship.IsSunk())
                {
                    playerTwoCount++;
                }
            }

            if (playerOneCount == PlayerOne.Ships.Count)
            {
                return PlayerTwo.Name;
            }

            if (playerTwoCount == PlayerTwo.Ships.Count)
            {
                return PlayerOne.Name;
            }

            return null;
        }

        private void InitPlayers()
        {
            Console.Clear();
            string? userNamePlayerOne = "";
            while (userNamePlayerOne == null || userNamePlayerOne.Length > 20 || userNamePlayerOne.Length == 0)
            {
                Console.WriteLine("Player One Username:");
                userNamePlayerOne = Console.ReadLine();
                Console.Clear();
            }
            PlayerOne!.Name = userNamePlayerOne;
            PlayerOne.Color = ConsoleColor.Red;
            PlayerOne.InitPanels(Height, Width);


            string? userNamePlayerTwo = "";
            while (userNamePlayerTwo == null || userNamePlayerTwo.Length > 20 || userNamePlayerTwo.Length == 0)
            {
                Console.WriteLine("Player Two Username:");
                userNamePlayerTwo = Console.ReadLine();
                Console.Clear();
            }
            PlayerTwo!.Name = userNamePlayerTwo;
            PlayerTwo.Color = ConsoleColor.Blue;
            PlayerTwo.InitPanels(Height, Width);
            Console.Clear();
        }
    }
}