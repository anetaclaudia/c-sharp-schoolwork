using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Newtonsoft.Json;

namespace GameBrain
{
    public static class Mapping
    {
        public static SavedGame GameToGame(Game game)
        {
            SavedGame savedGame = new SavedGame();
            savedGame.SavedGameId = game.GameId;
            savedGame.Height = game.Height;
            savedGame.Width = game.Width;
            savedGame.PlayerOne = PlayerToPlayer(game.PlayerOne);
            savedGame.PlayerTwo = PlayerToPlayer(game.PlayerTwo);
            savedGame.WinningPlayer = game.WinningPlayer;
            savedGame.Moves = JsonConvert.SerializeObject(game.Moves);
            return savedGame;
        }

        public static Game GameToGame(SavedGame savedGame)
        {
            Game game = new Game();
            game.GameId = savedGame.SavedGameId;
            game.Height = savedGame.Height;
            game.Width = savedGame.Width;
            game.PlayerOne = PlayerToPlayer(savedGame.PlayerOne!);
            game.PlayerOne!.Color = ConsoleColor.Magenta;
            game.PlayerTwo = PlayerToPlayer(savedGame.PlayerTwo!);
            game.PlayerTwo!.Color = ConsoleColor.Blue;
            game.WinningPlayer = savedGame.WinningPlayer;
            game.Moves = JsonConvert.DeserializeObject<List<Move>>(savedGame.Moves!);
            return game;
        }

        public static Domain.Player? PlayerToPlayer(GameBrain.Player? player)
        {
            if (player == null)
            {
                return null;
            }

            Domain.Player newPlayer = new Domain.Player();
            newPlayer.Name = player.Name;
            newPlayer.PlayerId = player.PlayerId;
            newPlayer.PanelsAndShips = JsonConvert.SerializeObject(player.Panels);
            newPlayer.StartingPanelsAndShips = JsonConvert.SerializeObject(player.StartingPanels);
            return newPlayer;
        }

        public static GameBrain.Player? PlayerToPlayer(Domain.Player? player)
        {
            if (player == null)
            {
                return null;
            }

            GameBrain.Player newPlayer = new GameBrain.Player();
            newPlayer.Name = player.Name!;
            newPlayer.PlayerId = player.PlayerId;
            newPlayer.Panels = JsonConvert.DeserializeObject<List<Panel>>(player.PanelsAndShips!);
            (newPlayer.Ships, newPlayer.Panels) = PanelsToShips(newPlayer.Panels);
            newPlayer.StartingPanels = JsonConvert.DeserializeObject<List<Panel>>(player.StartingPanelsAndShips!);
            return newPlayer;
        }


        private static (List<Ship>, List<Panel>) PanelsToShips(List<GameBrain.Panel> panels)
        {
            Dictionary<ShipType, Ship> ships = new Dictionary<ShipType, Ship>();
            ships[ShipType.Battleship] = new Ship(ShipType.Battleship);
            ships[ShipType.Carrier] = new Ship(ShipType.Carrier);
            ships[ShipType.Cruiser] = new Ship(ShipType.Cruiser);
            ships[ShipType.Destroyer] = new Ship(ShipType.Destroyer);
            ships[ShipType.Submarine] = new Ship(ShipType.Submarine);
            List<Panel> finalizedPanels = new List<Panel>();
            foreach (var panel in panels)
            {
                if (panel.ship != null)
                {
                    Ship ship = ships[panel.ship.ShipType];
                    ship.ShipId = panel.ship.ShipId;
                    panel.ship = ship;
                    ship.Panels.Add(panel);
                }
                finalizedPanels.Add(panel);
            }
            return (ships.Values.ToList(), finalizedPanels);
        }
    }
}