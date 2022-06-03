using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QLearner.Resources;

namespace Poker
{
 
    public class Deck 
    {
        static Random rnd = new Random();
        public int size = 52;
        public List<Card> deck = new List<Card>();
        public List<Card> removed = new List<Card>();

        public Deck() 
        {
            List<Rank> ranks = new List<Rank> {
                Rank.Two,
                Rank.Three,
                Rank.Four,
                Rank.Five,
                Rank.Six,
                Rank.Seven,
                Rank.Eight,
                Rank.Nine,
                Rank.Ten,
                Rank.Jack,
                Rank.Queen,
                Rank.King,
                Rank.Ace
            };
            List<Suite> suites = new List<Suite> {
                Suite.Club,
                Suite.Diamond,
                Suite.Heart,
                Suite.Spade,
            };

            foreach (Rank i in ranks)
            {
                foreach (Suite j in suites)
                {
                    Card card = new Card(i, j);
                    deck.Add(card);
                }
            }

        }

        public void Shuffle()
        {
            if (removed.Any())
            {
                foreach (Card c in removed)
                    deck.Add(c);
                removed.Clear();
            }
            for(int j=0; j<10; j++)
            for (int i = size - 1; i >= 0; i--)
            {
                int secondCardIndex = rnd.Next(size);
                Card tmp = deck[i];
                deck[i] = deck[secondCardIndex];
                deck[secondCardIndex] = tmp;
            }
        }

        public Card DealCard()
        {
            Card card = deck.Last();
            deck.RemoveAt(deck.Count - 1);
            removed.Add(card);
            return card;
        }

        public Card DealCard(Card c)
        {
            deck.Remove(c);
            removed.Add(c);
            return c;
        }

        public void Burn()
        {
            removed.Add(deck.Last());
            deck.RemoveAt(deck.Count - 1);
        }
    }
}