using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackJack.Module
{
    internal class Dealer
    {
        private List<Card> hand;
        private Card hiddenCard;

        public event Action<string> OnDealerHandChanged;

        public Dealer()
        {
            hand = new List<Card>();
        }

        public Card dealCard(Deck deck)
        {
            Card card = deck.DrawCard();

            return card;
        }

        public void showCards(bool revealAll = false)
        {
            string handDisplay;

            if (revealAll)
            {
                handDisplay = "Dealer's hand: " + GetHandString(revealAll);
            }
            else
            {
                handDisplay = "Dealer's hand: " + GetHandString(hidden: true);
            }

            OnDealerHandChanged?.Invoke(handDisplay);
        }

        public void receiveCard(Card card, bool isHidden = false)
        {
            if (isHidden)
            {
                hiddenCard = card;
            }
            else
            {
                hand.Add(card);
            }
        }

        public string GetHandString(bool hidden = false)
        {
            if (hidden && hiddenCard != null && hand.Count > 0)
            {
                return $"[Hidden], {string.Join(", ", hand.Select(c => c.ToString()))}";
            }

            return string.Join(", ", hand.Select(card => card.ToString())) + $" | Total score: {calculateScore()}";
        }

        public void revealHiddenCard()
        {
            if (hiddenCard != null)
            {
                hand.Add(hiddenCard);
                hiddenCard = null;
            }
        }

        public bool hasBlackjack()
        {
            return calculateScore() == 21 && hand.Count == 2;
        }

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

        public bool shouldHit()
        {
            return calculateScore() < 17;
        }

        public void playTurn(Deck deck)
        {
            revealHiddenCard();
            OnDealerHandChanged?.Invoke(GetHandString(hidden: false));


            while (shouldHit())
            {
                receiveCard(dealCard(deck));
                showCards(revealAll: true);

                if (isBusted())
                {
                    return;
                }
            }
        }

        public void clearHand()
        {
            hand.Clear();
            hiddenCard = null;
        }

        public bool isBusted()
        {
            return calculateScore() > 21;
        }
    }
}