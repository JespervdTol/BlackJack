using BlackJack.Module;
using Microsoft.Maui.Controls;
using System;

namespace BlackJack.Pages
{
    public partial class GamePage : ContentPage
    {
        Game game;
        private int currentBet;
        private int totalBet;

        public GamePage()
        {
            InitializeComponent();
            game = new Game();
            totalBet = 0;

            game.OnGameMessage += UpdateStatusMessage;
            game.OnPlayerHandUpdate += UpdatePlayerHand;
            game.OnDealerHandUpdate += UpdateDealerHand;
            game.OnGameEnded += GameEnded;

            UpdateBalanceDisplay();
        }

        private void UpdateUI()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayerHandLabel.Text = game.player.GetHandString();
                DealerHandLabel.Text = game.dealer.GetHandString(hidden: true);

                HitButton.IsEnabled = true;
                StandButton.IsEnabled = true;
                DoubleDownButton.IsEnabled = true;
                HitButton.IsVisible = true;
                StandButton.IsVisible = true;
                DoubleDownButton.IsVisible = true;
                BetAmountEntry.IsVisible = false;
                PlaceBetButton.IsVisible = false;
                StatusLabel.Text = "Game has started.";
            });
        }

        private void UpdatePlayerHand(string handDescription)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayerHandLabel.Text = handDescription;
            });
        }

        private void UpdateDealerHand(string handDescription)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                DealerHandLabel.Text = handDescription;
            });
        }

        private void UpdateStatusMessage(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StatusLabel.Text = message;
            });
        }

        private void UpdateBalanceDisplay()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PlayerBalanceLabel.Text = $"Balance: ${game.player.Balance}";
            });
        }

        private void GameEnded()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                HitButton.IsEnabled = false;
                StandButton.IsEnabled = false;
                DoubleDownButton.IsEnabled = false;

                int playerScore = game.player.calculateScore();
                int dealerScore = game.dealer.calculateScore();

                if (game.player.isBusted())
                {
                    game.player.loseBet(totalBet);
                    UpdateStatusMessage("You busted! Dealer wins.");
                }
                else if (game.dealer.isBusted())
                {
                    game.player.addWinnings(totalBet * 2);
                    UpdateStatusMessage("Dealer busted! You win!");
                }
                else if (playerScore > dealerScore)
                {
                    game.player.addWinnings(totalBet * 2);
                    UpdateStatusMessage("You win!");
                }
                else if (playerScore < dealerScore)
                {
                    game.player.loseBet(totalBet);
                    UpdateStatusMessage("Dealer wins!");
                }
                else
                {
                    game.player.addWinnings(totalBet);
                    UpdateStatusMessage("It's a push!");
                }

                UpdateBalanceDisplay();
                UpdateDealerHand(game.dealer.GetHandString(hidden: false));
                currentBet = 0;
                totalBet = 0;
                NextRoundButton.IsEnabled = true;
                NextRoundButton.IsVisible = true;
                HitButton.IsVisible = false;
                StandButton.IsVisible = false;
                DoubleDownButton.IsVisible = false;
            });
        }

        private async void onHitButtonClicked(object sender, EventArgs e)
        {
            game.player.receiveCard(game.dealer.dealCard(game.deck));
            UpdatePlayerHand(game.player.GetHandString());

            if (game.player.isBusted())
            {
                HitButton.IsEnabled = false;
                StandButton.IsEnabled = false;
                await game.endGame();
            }
        }

        private async void onStandButtonClicked(object sender, EventArgs e)
        {
            game.dealer.playTurn(game.deck);
            UpdateDealerHand(game.dealer.GetHandString(hidden: false));

            if (game.dealer.isBusted())
            {
                HitButton.IsEnabled = false;
                StandButton.IsEnabled = false;
                await game.endGame();
            }
            else
            {
                await game.endGame();
            }
        }

        private void OnNextRoundButtonClicked(object sender, EventArgs e)
        {
            PlayerHandLabel.Text = "";
            DealerHandLabel.Text = "";
            UpdateStatusMessage("Place your bet to start the new round!");

            NextRoundButton.IsEnabled = false;
            NextRoundButton.IsVisible = false;
            HitButton.IsVisible = false;
            StandButton.IsVisible = false;
            DoubleDownButton.IsVisible = false;
            BetAmountEntry.IsVisible = true;
            PlaceBetButton.IsVisible = true;

            game.nextRound();
        }

        private async void OnPlaceBetButtonClicked(object sender, EventArgs e)
        {
            if (int.TryParse(BetAmountEntry.Text, out int betAmount) && betAmount > 0)
            {
                if (game.player.placeBet(betAmount))
                {
                    currentBet = betAmount;
                    totalBet = currentBet;
                    UpdateBalanceDisplay();
                    await game.startGame();
                    UpdateUI();
                }
                else
                {
                    UpdateStatusMessage("Insufficient balance to place this bet.");
                }
            }
            else
            {
                UpdateStatusMessage("Please enter a valid bet amount.");
            }
        }

        private async void OnDoubleDownButtonClicked(object sender, EventArgs e)
        {
            if (currentBet > 0 && game.player.Balance >= currentBet)
            {
                if (game.player.doubleDown(currentBet, game.dealer.dealCard(game.deck)))
                {
                    totalBet += currentBet;
                    UpdatePlayerHand(game.player.GetHandString());
                    UpdateBalanceDisplay();

                    if (game.player.isBusted())
                    {
                        await game.endGame();
                        return;
                    }

                    game.dealer.playTurn(game.deck);
                    UpdateDealerHand(game.dealer.GetHandString(hidden: false));

                    await game.endGame();
                }
            }
            else
            {
                UpdateStatusMessage("Insufficient balance to double down.");
            }
        }
    }
}