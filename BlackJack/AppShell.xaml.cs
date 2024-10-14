using BlackJack.Pages;

namespace BlackJack
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
;
            Routing.RegisterRoute("GamePage", typeof(BlackJack.Pages.GamePage));
            Routing.RegisterRoute("InstructionPage", typeof(BlackJack.Pages.InstructionPage));
        }
    }
}
