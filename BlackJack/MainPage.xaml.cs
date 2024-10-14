using BlackJack.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using BlackJack.Pages;

namespace BlackJack
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void onStartGameClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
        }

        private async void onInstructionsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("InstructionPage");
        }
    }
}
