using CampusLove2.Aplication.UI;

namespace CampusLove2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MenuPrincipal menu = new MenuPrincipal();
            await menu.MostrarMenu();
        }
    }
}