using BlackJack.Module;
using System;
using System.Threading.Tasks;

namespace BlackJack
{
    internal class Game
    {
        public Player player { get; private set; }
        public Dealer dealer { get; private set; }
        public Deck deck { get; private set; }

        public int CurrentBet { get; private set; }

        public event Action<string> OnGameMessage;
        public event Action<string> OnPlayerHandUpdate;
        public event Action<string> OnDealerHandUpdate;
        public event Action OnGameEnded;

        public Game()
        {
            player = new Player();
            dealer = new Dealer();
            deck = new Deck();

            player.OnPlayerHandUpdate += hand => OnPlayerHandUpdate?.Invoke(hand);
            dealer.OnDealerHandChanged += hand => OnDealerHandUpdate?.Invoke(hand);
        }

        public void PlaceBet(int amount)
        {
            CurrentBet = amount;
        }

        public async Task startGame()
        {
            deck.shuffle();

            player.receiveCard(dealer.dealCard(deck));
            dealer.receiveCard(dealer.dealCard(deck), isHidden: true);
            player.receiveCard(dealer.dealCard(deck));
            dealer.receiveCard(dealer.dealCard(deck));

            OnPlayerHandUpdate?.Invoke(player.GetHandString());
            OnDealerHandUpdate?.Invoke(dealer.GetHandString(hidden: true));

            if (player.hasBlackjack() || dealer.hasBlackjack())
            {
                await endGame();
            }
        }

        public async Task endGame()
        {
            dealer.revealHiddenCard();

            OnPlayerHandUpdate?.Invoke(player.GetHandString());
            OnDealerHandUpdate?.Invoke(dealer.GetHandString(hidden: false));

            determineWinner(player, dealer);
            OnGameEnded?.Invoke();
        }

        public async Task nextRound()
        {
            player.clearHand();
            dealer.clearHand();
            deck.shuffle();
        }

        public void determineWinner(Player player, Dealer dealer)
        {
            int playerScore = player.calculateScore();
            int dealerScore = dealer.calculateScore();

            if (playerScore > 21)
            {
                player.loseBet(CurrentBet);
            }
            else if (dealerScore > 21)
            {
                player.addWinnings(CurrentBet * 2);
            }
            else if (playerScore > dealerScore)
            {
                player.addWinnings(CurrentBet * 2);
            }
            else if (dealerScore > playerScore)
            {
                player.loseBet(CurrentBet);
            }
            else
            {
                player.addWinnings(CurrentBet);
            }

            CurrentBet = 0;
        }
    }
}