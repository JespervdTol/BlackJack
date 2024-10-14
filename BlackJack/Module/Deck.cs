using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack.Module
{
    internal class Deck
    {
        private List<Card> cards;

        public Deck(int numberOfDecks = 3)
        {
            cards = new List<Card>();
            InitializeDeck(numberOfDecks);
        }

        private void InitializeDeck(int numberOfDecks)
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

            for (int i = 0; i < numberOfDecks; i++)
            {
                foreach (var suit in suits)
                {
                    foreach (var rank in ranks)
                    {
                        int value = GetCardValue(rank);
                        cards.Add(new Card(rank, value, suit));
                    }
                }
            }
        }

        public void shuffle()
        {
            Random rng = new Random();
            int n = cards.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                Card temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }
        }

        public Card DrawCard()
        {
            if (cards.Count == 0)
            {
                return null;
            }

            Card drawnCard = cards[0];
            cards.RemoveAt(0);
            return drawnCard;
        }

        private static int GetCardValue(string rank)
        {
            return rank switch
            {
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "6" => 6,
                "7" => 7,
                "8" => 8,
                "9" => 9,
                "10" => 10,
                "J" => 10,
                "Q" => 10,
                "K" => 10,
                "A" => 11,
                _ => 0,
            };
        }

        public int RemainingCards()
        {
            return cards.Count;
        }
    }
}