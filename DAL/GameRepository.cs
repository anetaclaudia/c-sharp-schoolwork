using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class GameRepository
    {
        private readonly AppDbContext _context;

        public GameRepository()
        {
            _context = new AppDbContext();
        }

        public SavedGame SaveGame(SavedGame savedGame)
        {
            return _context.SavedGames.Update(savedGame).Entity;
        }

        public async Task<ICollection<SavedGame>> LoadAllGames()
        
        {
            var savedGames = await _context.SavedGames
                .Include(game => game.PlayerOne)
                .Include(game => game.PlayerTwo)
                .ToListAsync();
            return savedGames;
        }

        public async Task<SavedGame> LoadGameAsync(int gameId)
        {
            var savedGame = await _context.SavedGames
                .Include(game => game.PlayerOne)
                .Include(game => game.PlayerTwo)
                .FirstOrDefaultAsync(game => game.SavedGameId == gameId);
            return savedGame;
        }

        public void UpdateGame(SavedGame savedGame)
        {
            _context.SavedGames.Update(savedGame);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}