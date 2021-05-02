using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Game
{
    public class NewGame : PageModel
    {
        [BindProperty] public string? GamePlayerOneName { get; set; }
        [BindProperty] public string? GamePlayerTwoName { get; set; }
        [BindProperty] public int Height { get; set; }
        [BindProperty] public int Width { get; set; }
        [BindProperty] public bool PlaceRandomly { get; set; }

        public void OnGet()
        {
        }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();
            return RedirectToPage("./PlayGame", new
            {
                playerOne = GamePlayerOneName, playerTwo = GamePlayerTwoName, 
                height = Height, width = Width, placeRandomly = PlaceRandomly ? 1 : 0
            });
        }
    }
}