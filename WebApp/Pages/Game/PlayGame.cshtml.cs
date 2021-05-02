using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameBrain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace WebApp.Pages.Game
{
    public class PlayGame : PageModel
    {
        [BindProperty] public GameBrain.Game? Game { get; set; }
        [BindProperty] public GameBrain.Player? ControllingPlayer { get; set; }
        [BindProperty] public GameBrain.Player? OpposingPlayer { get; set; }
        [BindProperty] public GameBrain.Ship? LastHitShip { get; set; }

        [BindProperty] public GameBrain.Ship? ShipBeingPlaced { get; set; }

        [BindProperty] public bool Replay { get; set; }
        [BindProperty] public bool PlaceShips { get; set; } = false;

        [BindProperty] public bool ShipOrientation { get; set; } = false;

        [BindProperty] public int MoveCount { get; set; }


        public async Task<ActionResult> OnGet(
            int? gameId,
            int? replayGameId,
            string playerOne, string playerTwo, int? height, int? width, int? placeRandomly)
        {
            if (gameId != null && replayGameId == null)
            {
                return await LoadGameById(gameId);
            }

            if (replayGameId != null && gameId == null)
            {
                return await LoadGameById(replayGameId, true);
            }

            if (string.IsNullOrEmpty(playerOne) || playerOne.Length > 20
                                                || string.IsNullOrEmpty(playerTwo) || playerTwo.Length > 20
                                                || height < 5 || height > 26 || height == null
                                                || width < 5 || width > 26 || width == null
                                                || placeRandomly < 0 || placeRandomly > 1 || placeRandomly == null)
            {
                return RedirectToPage("./NewGame");
            }

            return await NewGame(playerOne, playerTwo, height.Value, width.Value, placeRandomly.Value);
        }

        public async Task<ActionResult> PlaceShipsByGameId(int? placeShipsGameId, bool rotation, int? row, int? column)
        {
            if (!placeShipsGameId.HasValue)
            {
                return RedirectToPage("./NewGame");
            }

            var loadedGame = await new GameRepository().LoadGameAsync(placeShipsGameId.Value);
            Game = Mapping.GameToGame(loadedGame);
            (ControllingPlayer, OpposingPlayer) = GetCurrentAndOpposingPlayersForPlacingShips();
            PlaceShips = true;
            ShipOrientation = rotation;
            if (row == null && column == null) // means we are only rotating the ship, not placing it
            {
                ShipOrientation = !rotation;
            }

            ShipBeingPlaced = GetNextShipToBePlaced();
            if (ShipBeingPlaced == null)
            {
                return await LoadGameById(Game.GameId);
            }

            if (row != null && column != null)
            {
                int endrow;
                int endcolumn;
                (endrow, endcolumn) = Utils.getEndsBasedOnOrientationAndShip(rotation, ShipBeingPlaced,
                    row.Value, column.Value);
                List<Panel> shipPanels = GetAffectedPanels(row.Value, column.Value, endrow, endcolumn);
                if (shipPanels.Any(x => x.IsOccupied) || 
                    Utils.WillBeOutOfBounds(row.Value, column.Value, endrow, endcolumn, Game))
                {
                    return Page();
                }
                else
                {
                    SaveShipToPanels(shipPanels);
                    UpdateGame();
                    (ControllingPlayer, OpposingPlayer) = GetCurrentAndOpposingPlayersForPlacingShips();
                    ShipBeingPlaced = GetNextShipToBePlaced();
                    if (ShipBeingPlaced == null)
                    {
                        PlaceShips = false;
                        return await LoadGameById(Game.GameId);
                    }
                }
            }

            return Page();
        }

        private void SaveShipToPanels(List<Panel> shipPanels)
        {
            foreach (var panel in shipPanels)
            {
                panel.ship = ShipBeingPlaced;
                panel.OccupationType = ShipBeingPlaced!.GetOccupationType();
                ShipBeingPlaced.Panels.Add(panel);
            }
        }

        private List<Panel> GetAffectedPanels(int startrow, int startcolumn, int endrow, int endcolumn)
        {
            return ControllingPlayer!.Panels.Range(startrow, startcolumn, endrow, endcolumn);
        }

        private Ship? GetNextShipToBePlaced()
        {
            foreach (var ship in ControllingPlayer!.Ships)
            {
                if (!ship.HasBeenPlaced())
                {
                    return ship;
                }
            }

            return null;
        }

        public async Task<ActionResult> LoadGameById(int? gameId, bool replayGame = false)
        {
            if (!gameId.HasValue)
            {
                return RedirectToPage("./NewGame");
            }

            var loadedGame = await new GameRepository().LoadGameAsync(gameId.Value);
            Game = Mapping.GameToGame(loadedGame);
            if (!replayGame)
            {
                InitSavedGame();
            }
            else
            {
                SetupReplay();
                return await LoadGameReplay(Game.GameId, 0);
            }

            LastHitShip = null;
            return Page();
        }

        private void SetupReplay()
        {
            ControllingPlayer = Game!.PlayerOne;
            OpposingPlayer = Game!.PlayerTwo;
            Replay = true;
            MoveCount = 0;
        }

        private void InitSavedGame()
        {
            var controllingPlayerId = Game!.Moves.Count != 0 ? Game!.Moves[^1].MovePlayerId : Game!.PlayerOne!.PlayerId;
            if (Game.PlayerOne!.PlayerId == controllingPlayerId)
            {
                ControllingPlayer = Game.PlayerOne;
                OpposingPlayer = Game.PlayerTwo;
            }
            else
            {
                ControllingPlayer = Game.PlayerTwo;
                OpposingPlayer = Game.PlayerOne;
            }
        }

        public async Task<ActionResult> NewGame(string playerOne, string playerTwo, int height, int width,
            int placeRandomly)
        {
            LastHitShip = null;
            GameBrain.Game game = new GameBrain.Game();
            game.Height = height;
            game.Width = width;
            game.PlayerOne!.Name = playerOne;
            game.PlayerOne.InitPanels(height, width);
            game.PlayerTwo!.Name = playerTwo;
            game.PlayerTwo.InitPanels(height, width);
            if (placeRandomly == 1)
            {
                game.PlayerOne.PlaceShipsRandom(height, width);
                game.PlayerTwo.PlaceShipsRandom(height, width);
            }
            else
            {
                PlaceShips = true;
                ShipBeingPlaced = game.PlayerOne.Ships[0];
            }

            LastHitShip = null;
            ControllingPlayer = game.PlayerOne;
            OpposingPlayer = game.PlayerTwo;
            game.PlayerOne.StartingPanels =
                JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(game.PlayerOne.Panels));
            game.PlayerTwo.StartingPanels =
                JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(game.PlayerTwo.Panels));
            var repo = new GameRepository();
            SavedGame savedGame = repo.SaveGame(Mapping.GameToGame(game));
            await repo.SaveChangesAsync();
            this.Game = Mapping.GameToGame(savedGame);
            ControllingPlayer = this.Game.PlayerOne;
            OpposingPlayer = this.Game.PlayerTwo;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? row, int? column, int gameId, int? controllingPlayerId,
            int? moveCount,
            int? placeShipsGameId, bool? rotation)
        {
            if (moveCount != null)
            {
                return await LoadGameReplay(gameId, moveCount.Value);
            }

            if (placeShipsGameId != null && rotation != null)
            {
                return await PlaceShipsByGameId(placeShipsGameId, rotation.Value, row, column);
            }

            if (row == null || column == null || controllingPlayerId == null)
            {
                return Page();
            }

            return await LoadGameNormal(row.Value, column.Value, gameId, controllingPlayerId.Value);
        }

        public async Task<ActionResult> LoadGameReplay(int gameId, int moveCount)
        {
            var loadedGame = await new GameRepository().LoadGameAsync(gameId);
            Game = Mapping.GameToGame(loadedGame);
            ControllingPlayer = Game.PlayerOne;
            OpposingPlayer = Game.PlayerTwo;
            ControllingPlayer!.Panels =
                JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(Game.PlayerOne!.StartingPanels));
            OpposingPlayer!.Panels =
                JsonConvert.DeserializeObject<List<Panel>>(JsonConvert.SerializeObject(Game.PlayerTwo!.StartingPanels));
            Game.WinningPlayer = null;
            for (int i = 0; i < moveCount; i++)
            {
                var currentMove = Game.Moves[i];
                (ControllingPlayer, OpposingPlayer) = GetCurrentAndOpposingPlayers(currentMove.MovePlayerId);
                OpposingPlayer!.Panels.At(currentMove.XCoordinate, currentMove.YCoordinate).OccupationType =
                    currentMove.OccupationType;
            }

            MoveCount = moveCount;
            Replay = true;
            LastHitShip = null;
            return Page();
        }

        private (GameBrain.Player?, GameBrain.Player?) GetCurrentAndOpposingPlayersForPlacingShips()
        {
            return Game!.PlayerOne!.HasPlacedAllShips()
                ? (Game.PlayerTwo, Game.PlayerOne)
                : (Game.PlayerOne, Game.PlayerTwo);
        }

        private (GameBrain.Player?, GameBrain.Player?) GetCurrentAndOpposingPlayers(int playerId)
        {
            return Game!.PlayerOne!.PlayerId == playerId
                ? (Game.PlayerOne, Game.PlayerTwo)
                : (Game.PlayerTwo, Game.PlayerOne);
        }

        public async Task<IActionResult> LoadGameNormal(int row, int column, int gameId, int? controllingPlayerId)
        {
            var loadedGame = await new GameRepository().LoadGameAsync(gameId);
            Game = Mapping.GameToGame(loadedGame);
            if (Game.PlayerOne!.PlayerId == controllingPlayerId)
            {
                ControllingPlayer = Game.PlayerOne;
                OpposingPlayer = Game.PlayerTwo;
            }
            else
            {
                ControllingPlayer = Game.PlayerTwo;
                OpposingPlayer = Game.PlayerOne;
            }

            var panelAtCurrentLocation = OpposingPlayer!.Panels.At(row, column);
            var move = new Move
            {
                MoveNumber = Game.Moves.Count + 1,
                XCoordinate = row,
                YCoordinate = column,
                MovePlayerId = ControllingPlayer!.PlayerId
            };
            if (panelAtCurrentLocation.IsOccupied)
            {
                panelAtCurrentLocation.OccupationType = OccupationType.Hit;
                move.OccupationType = panelAtCurrentLocation.OccupationType;
                Game.AddMove(move);
                Game.WinningPlayer = Game.IsGameOver();
                LastHitShip = panelAtCurrentLocation.ship;
                UpdateGame();
                return Page();
            }

            panelAtCurrentLocation.OccupationType = OccupationType.Miss;
            move.OccupationType = panelAtCurrentLocation.OccupationType;
            var tempControllingPLayer = ControllingPlayer;
            var tempOpposingPLayer = OpposingPlayer;
            ControllingPlayer = tempOpposingPLayer;
            OpposingPlayer = tempControllingPLayer;
            Game.AddMove(move);
            Game.WinningPlayer = Game.IsGameOver();
            LastHitShip = null;
            UpdateGame();
            return Page();
        }

        private void UpdateGame()
        {
            var repo = new GameRepository();
            var gameToSave = Mapping.GameToGame(Game!);
            repo.UpdateGame(gameToSave);
            repo.SaveChangesAsync();
        }
    }
}