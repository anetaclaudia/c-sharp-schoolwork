using System.Collections.Generic;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class SavedGames : PageModel
    {
        public ICollection<SavedGame>? SavedGame { get; set; }

        public async Task OnGetAsync()
        {
            SavedGame = await new GameRepository().LoadAllGames();
        }
    }
}