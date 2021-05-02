using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using DAL;
using Domain;
using GameBrain;
using MenuSystem;
using Newtonsoft.Json;
using Move = GameBrain.Move;
using Panel = GameBrain.Panel;
using Player = GameBrain.Player;

namespace ConsoleApp
{
    internal static class Program
    {
        private static Game Game = new Game();
        private static Menu mainMenu = new Menu("Main Menu");
        private static Menu newGameMenu = new Menu("New game Menu");
        private static Menu loadSavedGamesMenu = new Menu("Saved games Menu");
        public delegate double Calculate(double value);

        private static void Main(string[] args)
        {
            mainMenu.AddMenuItem(new MenuItem("New game", newGameMenu.RunMenu));
            mainMenu.AddMenuItem(new MenuItem("Saved game", LoadGames));
            mainMenu.AddMenuItem(new MenuItem("Replay saved games", ReplayGames));
            mainMenu.AddMenuItem(new MenuItem("Exit", Exit));
            
            newGameMenu.AddMenuItem(new MenuItem("Start", StartNewGame));
            newGameMenu.AddMenuItem(new MenuItem("Map Height", MenuItem.MenuSubTypeEnum.MapHeight, Game));
            newGameMenu.AddMenuItem(new MenuItem("Map Width", MenuItem.MenuSubTypeEnum.MapWidth, Game));
            newGameMenu.AddMenuItem(new MenuItem("Randomize board", MenuItem.MenuSubTypeEnum.RandomBoard, Game));
            newGameMenu.AddMenuItem(new MenuItem("Back", mainMenu.RunMenu));
            
            mainMenu.RunMenu();
        }

        private static string ReplayGames()
        {
            Console.Clear();
            Console.WriteLine("Loading...");
            var loadedGames = new GameRepository().LoadAllGames().Result;
            loadSavedGamesMenu.MenuItems = new List<MenuItem>();
            loadSavedGamesMenu.AddMenuItem(new MenuItem("Back", mainMenu.RunMenu));
            foreach (var savedGame in loadedGames)
            {
                loadSavedGamesMenu.AddMenuItem(new MenuItem(savedGame.ToString(), savedGame.ReplayGame));
            }
            loadSavedGamesMenu.AddMenuItem(new MenuItem("Back", mainMenu.RunMenu));
            return loadSavedGamesMenu.RunMenu();
        }

        private static string ReplayGame(this SavedGame game)
        {
            var loadedGame = new GameRepository().LoadGameAsync(game.SavedGameId).Result;
            Game = Mapping.GameToGame(loadedGame);
            StartReplayGame();
            return "";
        }

        private static (Player?, Player?) GetCurrentAndOpposingPlayers(int playerId)
        {
            return Game.PlayerOne!.PlayerId == playerId ? (Game.PlayerOne, Game.PlayerTwo) : (Game.PlayerTwo, Game.PlayerOne);
        }

        private static void StartReplayGame()
        {
            Console.CursorVisible = false;
            ConsoleKey key;
            int i = 0;
            do
            {
                Player? controllingPlayer = Game.PlayerOne!;
                Player? opposingPlayer = Game.PlayerTwo!;
                
                // setting both players panels into the beginning state
                // using jsonconvert for creating deep copy of the boards 
                controllingPlayer.Panels = JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(Game.PlayerOne!.StartingPanels));
                opposingPlayer.Panels = JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(Game.PlayerTwo!.StartingPanels));
                
                if (Game.Moves.Count <= 0)
                {
                    Console.Clear();
                    Console.Write("There haven't been any moves made in this game."); // this is important when user is trying to replay moves
                    Thread.Sleep(2000);
                    mainMenu.RunMenu();
                    return;
                }
                Move currentMove = Game.Moves[0];
                for (int j = 0; j < i; j++)
                {
                    currentMove = Game.Moves[j];
                    (controllingPlayer, opposingPlayer) = GetCurrentAndOpposingPlayers(currentMove!.MovePlayerId)!;
                    opposingPlayer!.Panels.At(currentMove.XCoordinate, currentMove.YCoordinate).OccupationType =
                        currentMove.OccupationType; // simulating the game movement, overwrites the panel's value
                    controllingPlayer!.SelectedHeight = currentMove.XCoordinate;
                    controllingPlayer.SelectedWidth = currentMove.YCoordinate;
                }
                Console.Clear();
                DrawBoardConsole(controllingPlayer, opposingPlayer);
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                    {
                        if (currentMove.MoveNumber > 1)
                        {
                            i -= 1;
                        }

                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        if (currentMove.MoveNumber < Game.Moves.Count)
                        {
                            i += 1;
                        }

                        break;
                    }
                }
            } while (key != ConsoleKey.Escape); // pressing ESC button allows user to exit from replay-mode

            Game = new Game();
            mainMenu.RunMenu();
        }

        private static string LoadGames()
        {
            Console.Clear();
            Console.WriteLine("Loading...");
            var loadedGames = new GameRepository().LoadAllGames().Result;
            loadSavedGamesMenu.MenuItems = new List<MenuItem>();
            loadSavedGamesMenu.AddMenuItem(new MenuItem("Back", mainMenu.RunMenu));
            foreach (var savedGame in loadedGames)
            {
                loadSavedGamesMenu.AddMenuItem(new MenuItem(savedGame.ToString(), savedGame.LoadGame));
            }

            loadSavedGamesMenu.AddMenuItem(new MenuItem("Back", mainMenu.RunMenu));
            return loadSavedGamesMenu.RunMenu();
        }

        private static string LoadGame(this SavedGame game)
        {
            var loadedGame = new GameRepository().LoadGameAsync(game.SavedGameId).Result;
            if (loadedGame.WinningPlayer != null)
            {
                return loadSavedGamesMenu.RunMenu();
            }

            Game = Mapping.GameToGame(loadedGame);
            PlayGame();
            return "";
        }

        private static string Exit()
        {
            Console.Clear();
            Console.WriteLine("Closing down");
            return "";
        }

        private static string StartNewGame()
        {
            Game.InitGame();
            if (Game.RandomBoard == 1)
            {
                Game.PlayerOne!.PlaceShipsRandom(Game.Height, Game.Width);
                Game.PlayerTwo!.PlaceShipsRandom(Game.Height, Game.Width);
            }
            else
            {
                PlaceShipsConsole(Game.PlayerOne!);
                PlaceShipsConsole(Game.PlayerTwo!);
            }
            Game.PlayerOne!.StartingPanels = JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(Game.PlayerOne.Panels));
            Game.PlayerTwo!.StartingPanels = JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(Game.PlayerTwo.Panels));
            var repo = new GameRepository();
            SavedGame savedGame = repo.SaveGame(Mapping.GameToGame(Game));
            repo.SaveChangesAsync();
            Game = Mapping.GameToGame(savedGame);
            PlayGame();
            return "";
        }

        private static void PlayGame()
        {
            // int i = 0;
            var controllingPlayerId = Game.Moves.Count != 0 ? Game.Moves[^1].MovePlayerId : Game.PlayerOne!.PlayerId;
            string? winningPlayer = Game!.IsGameOver();
            while (winningPlayer == null)
            {
                var wasHit = false; // this checks if the player has hit a ship or not, important for further gameplay
               
                    if (controllingPlayerId == Game.PlayerOne!.PlayerId)
                    {
                        wasHit = PlayerTurn(Game.PlayerOne, Game.PlayerTwo!);
                    }
                    else
                    {
                        wasHit = PlayerTurn(Game.PlayerTwo!, Game.PlayerOne);
                    }

                    if (!wasHit)
                    {
                        if (Game.Moves[^1].MovePlayerId != Game.PlayerOne.PlayerId)
                        {
                            controllingPlayerId = Game.PlayerOne.PlayerId;
                        }
                        else
                        {
                            controllingPlayerId = Game.PlayerTwo!.PlayerId;
                        }
                    }

                winningPlayer = Game.IsGameOver();
            }

            Console.Clear();
            
            //following block is meant for printing out the info in the center of the console
            Console.Write(string.Concat(Enumerable.Repeat(Environment.NewLine, Game.Height)));
            Console.Write(string.Concat(Enumerable.Repeat("    ", Game.Width / 2)));
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(winningPlayer + " WON!");
            
            Game.WinningPlayer = winningPlayer;
            var repo = new GameRepository();
            var gameToSave = Mapping.GameToGame(Game);
            repo.UpdateGame(gameToSave);
            repo.SaveChangesAsync();
            Thread.Sleep(1000);
            mainMenu.RunMenu();
        }

        private static bool PlayerTurn(Player controllingPlayer, Player opposingPlayer)
        {
            Console.CursorVisible = false;
            ConsoleKey key;
            do
            {
                Console.Clear();
                DrawBoardConsole(controllingPlayer, opposingPlayer);
                key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    {
                        if (controllingPlayer.SelectedHeight > 0)
                        {
                            controllingPlayer.SelectedHeight -= 1;
                        }

                        break;
                    }
                    case ConsoleKey.DownArrow:
                    {
                        if (controllingPlayer.SelectedHeight < Game.Height - 1)
                        {
                            controllingPlayer.SelectedHeight += 1;
                        }

                        break;
                    }
                    case ConsoleKey.LeftArrow:
                    {
                        if (controllingPlayer.SelectedWidth > 0)
                        {
                            controllingPlayer.SelectedWidth -= 1;
                        }

                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        if (controllingPlayer.SelectedWidth < Game.Width - 1)
                        {
                            controllingPlayer.SelectedWidth += 1;
                        }

                        break;
                    }
                }
            } while (key != ConsoleKey.Enter);
            
            Panel panelAtCurrentLocation =
                opposingPlayer.Panels.At(controllingPlayer.SelectedHeight, controllingPlayer.SelectedWidth);
            if (panelAtCurrentLocation.OccupationType == OccupationType.Hit ||
                panelAtCurrentLocation.OccupationType == OccupationType.Miss)
            {
                return true; // the current controlling player proceeds, since they actually didnt make a move
            }
            Move move = new Move
            {
                MoveNumber = Game.Moves.Count + 1,
                XCoordinate = controllingPlayer.SelectedHeight,
                YCoordinate = controllingPlayer.SelectedWidth,
                MovePlayerId = controllingPlayer.PlayerId
            };
            if (panelAtCurrentLocation.IsOccupied)
            {
                panelAtCurrentLocation.OccupationType = OccupationType.Hit;
                Console.Clear();
                Console.Write(string.Concat(Enumerable.Repeat(Environment.NewLine, Game.Height)));
                Console.Write(string.Concat(Enumerable.Repeat("    ", Game.Width / 2)));
                if (panelAtCurrentLocation.ship!.IsSunk())
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.Write(panelAtCurrentLocation.ship.ShipType + " SUNK!");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.Write("HIT!");
                }

                Thread.Sleep(1000);
                Console.ResetColor();
                move.OccupationType = OccupationType.Hit;
                Game.AddMove(move);
                UpdateGame();
                return true; // the player's turn proceeds since they got a hit or a sunk
            }
            panelAtCurrentLocation.OccupationType = OccupationType.Miss;
            move.OccupationType = OccupationType.Miss;
            Game.AddMove(move);
            UpdateGame();
            return false;
        }
        
        private static void UpdateGame()
        {
            var repo = new GameRepository();
            var gameToSave = Mapping.GameToGame(Game);
            repo.UpdateGame(gameToSave);
            repo.SaveChangesAsync();
        }

        private static void DrawBoardConsole(Player controllingPlayer, Player? opposingPlayer = null, int endrow = 0, int endcolumn = 0)
        {
            Console.Write(controllingPlayer.Name + "'s turn");
            if (opposingPlayer == null)
            {
                Console.Write(" to place ship:");
            }

            Console.WriteLine();
            Utils.WriteLettersForBoard(Game.Width, controllingPlayer);
            Console.WriteLine(
                "   +" + string.Concat(Enumerable.Repeat("___+", Game.Width)).Substring(0, Game.Width * 4));
            for (int row = 0; row < Game.Height; row++)
            {
                Utils.WriteFirstEdges(row, controllingPlayer);

                for (int ownColumn = 0; ownColumn < Game.Width; ownColumn++)
                {
                    if (opposingPlayer == null)
                    {
                        // Write ship being placed panels as controlling player's color and *
                        if (row >= controllingPlayer.SelectedHeight && row <= endrow
                                                                    && ownColumn >= controllingPlayer.SelectedWidth &&
                                                                    ownColumn <= endcolumn)
                        {
                            Utils.WritePanelMessage(controllingPlayer.SelectedHeight, controllingPlayer.SelectedWidth,
                                "*", controllingPlayer);
                            continue;
                        }
                    }

                    string description;
                    // If opposing player is present means it's a real game (not placing ships)
                    if (opposingPlayer != null)
                    {
                        var panel = opposingPlayer.Panels.At(row, ownColumn);
                        if (panel.IsOccupied)
                        {
                            // Don't show opposing players ships.
                            description = OccupationType.Empty.GetAttributeOfType<DescriptionAttribute>()!.Description;
                        }
                        else
                        {
                            // description will remain the same - hit, miss, empty
                            description = panel
                                    .OccupationType
                                    .GetAttributeOfType<DescriptionAttribute>()!
                                .Description;
                        }
                    }
                    // Placing ships
                    else
                    {
                        // Show controlling player's own board
                        var panel = controllingPlayer.Panels.At(row, ownColumn);
                        description = panel
                                .OccupationType
                                .GetAttributeOfType<DescriptionAttribute>()!
                            .Description;
                    }

                    Utils.WritePanelMessage(row, ownColumn, description, controllingPlayer);
                }

                Utils.WriteSecondaryEdges(Game.Width, row, controllingPlayer);
                
            }

            Utils.WriteLettersForBoard(Game.Width, controllingPlayer);
        }

        private static void PlaceShipsConsole(Player controllingPlayer)
        {
            Console.CursorVisible = false;
            ConsoleKey key;
            bool orientation = true; // True is vertical
            for (int i = 0; i < controllingPlayer.Ships.Count; i++)
            {
                var ship = controllingPlayer.Ships[i];
                controllingPlayer.SelectedHeight = 0;
                controllingPlayer.SelectedWidth = 0;
                int endrow;
                int endcolumn;
                do
                {
                    Console.Clear();
                    (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(orientation, ship,
                        controllingPlayer.SelectedHeight, controllingPlayer.SelectedWidth);
                    DrawBoardConsole(controllingPlayer, null, endrow, endcolumn);
                    Console.Write("To turn the ship, please press Q.");
                    key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.UpArrow:
                        {
                            (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(orientation, ship,
                                controllingPlayer.SelectedHeight - 1,
                                controllingPlayer.SelectedWidth);
                            if (!Utils.WillBeOutOfBounds(
                                controllingPlayer.SelectedHeight - 1,
                                controllingPlayer.SelectedWidth,
                                endrow,
                                endcolumn, Game
                            ))
                            {
                                controllingPlayer.SelectedHeight -= 1;
                            }
                            break;
                        }
                        case ConsoleKey.DownArrow:
                        {
                            (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(orientation, ship,
                                controllingPlayer.SelectedHeight + 1,
                                controllingPlayer.SelectedWidth);
                            if (!Utils.WillBeOutOfBounds(
                                controllingPlayer.SelectedHeight + 1,
                                controllingPlayer.SelectedWidth,
                                endrow,
                                endcolumn, Game
                            ))
                            {
                                controllingPlayer.SelectedHeight += 1;
                            }

                            break;
                        }
                        case ConsoleKey.LeftArrow:
                        {
                            (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(orientation, ship,
                                controllingPlayer.SelectedHeight,
                                controllingPlayer.SelectedWidth - 1);
                            if (!Utils.WillBeOutOfBounds(
                                controllingPlayer.SelectedHeight,
                                controllingPlayer.SelectedWidth - 1,
                                endrow,
                                endcolumn, Game
                            ))
                            {
                                controllingPlayer.SelectedWidth -= 1;
                            }

                            break;
                        }
                        case ConsoleKey.RightArrow:
                        {
                            (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(orientation, ship,
                                controllingPlayer.SelectedHeight,
                                controllingPlayer.SelectedWidth + 1);
                            if (!Utils.WillBeOutOfBounds(
                                controllingPlayer.SelectedHeight,
                                controllingPlayer.SelectedWidth + 1,
                                endrow,
                                endcolumn, Game
                            ))
                            {
                                controllingPlayer.SelectedWidth += 1;
                            }

                            break;
                        }
                        case ConsoleKey.Q:
                        {
                            (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(!orientation, ship,
                                controllingPlayer.SelectedHeight, controllingPlayer.SelectedWidth);
                            if (!Utils.WillBeOutOfBounds(controllingPlayer.SelectedHeight,
                                controllingPlayer.SelectedWidth,
                                endrow, endcolumn, Game))
                            {
                                orientation = !orientation;
                            }

                            break;
                        }
                    }
                } while (key != ConsoleKey.Enter);

                var affectedPanels = controllingPlayer.Panels.Range(controllingPlayer.SelectedHeight,
                    controllingPlayer.SelectedWidth, endrow, endcolumn);
                if (affectedPanels.Any(x => x.IsOccupied))
                {
                    i--;
                }
                else
                {
                    ship.Panels = affectedPanels;
                    foreach (var panel in affectedPanels)
                    {
                        panel.OccupationType = ship.GetOccupationType();
                        panel.ship = ship;
                    }
                }
            }

            controllingPlayer.SelectedHeight = 0;
            controllingPlayer.SelectedWidth = 0;
        }
    }
}