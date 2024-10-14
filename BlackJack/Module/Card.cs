using System;

namespace BlackJack.Module
{
    internal class Card
    {
        private string rank;
        private int originalValue;
        private int currentValue;
        private string suit;

        public string Rank => rank;
        public int Value => currentValue;
        public string Suit => suit;

        public Card(string rank, int value, string suit)
        {
            this.rank = rank;
            originalValue = value;
            currentValue = value;
            this.suit = suit;
        }

        public void selectAceValue(bool isAceHigh)
        {
            if (rank == "A")
            {
                currentValue = isAceHigh ? 11 : 1;
            }
        }

        public override string ToString()
        {
            return $"{rank} of {suit} (Value: {currentValue})";
        }
    }
}