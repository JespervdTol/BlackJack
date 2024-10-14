using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack.Module
{
    internal class Player
    {
        private int balance;
        private List<Card> hand;

        public event Action<string> OnPlayerMessage;
        public event Action<string> OnPlayerHandUpdate;

        public Player(int initialBalance = 10000)
        {
            balance = initialBalance;
            hand = new List<Card>();
        }

        public int Balance => balance;

        //Method for Player???
        //public void stand()
        //{
        //    OnPlayerMessage?.Invoke("Player folds. You lose the bet.");
        //}

        public void hit(Card card)
        {
            hand.Add(card);
            UpdateHandDisplay();
        }

        public bool doubleDown(int amount, Card card)
        {
            if (balance >= amount)
            {
                balance -= amount;
                hand.Add(card);
                UpdateHandDisplay();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool placeBet(int amount)
        {
            if (balance >= amount)
            {
                balance -= amount;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void addWinnings(int amount)
        {
            balance += amount;
        }

        public void loseBet(int amount)
        {
            //Nodig?
        }

        public void receiveCard(Card card)
        {
            hand.Add(card);
            OnPlayerMessage?.Invoke($"Player receives: {card}");
            UpdateHandDisplay();
        }

        public bool hasBlackjack()
        {
            return calculateScore() == 21 && hand.Count == 2;
        }

        //Hand Class???
        public int calculateScore()
        {
            int score = 0;
            int aceCount = 0;

            foreach (var card in hand)
            {
                score += card.Value;
                if (card.Rank == "A")
                {
                    aceCount++;
                }

                if (score > 21 && aceCount > 0 && card.Rank == "A" && card.Value == 11)
                {
                    card.selectAceValue(false);
                    score -= 10;
                    aceCount--;
                }
            }

            return score;
        }

        private void UpdateHandDisplay()
        {
            OnPlayerHandUpdate?.Invoke(GetHandString());
        }

        public string GetHandString()
        {
            return string.Join(", ", hand.Select(card => card.ToString())) + $" | Total score: {calculateScore()}";
        }

        public bool isBusted()
        {
            return calculateScore() > 21;
        }

        public void clearHand()
        {
            hand.Clear();
        }
    }
}