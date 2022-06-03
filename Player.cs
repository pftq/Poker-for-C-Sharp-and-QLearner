using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QLearner.Resources;
using QLearner.QStates;

namespace Poker
{
    public class Player 
    {
        public string name ="";
        public int chips =0;
        public List<Card> cards = new List<Card>();
        public Card card1 { get { return cards.Any()? cards[0]:null; } }
        public Card card2 { get { return cards.Any()&&cards.Count>1? cards[1]:null; } }
        public int bet = 0;
        public bool folded, all_in, check, call, raise;
        private double[] oddsArray;
        public double[] oddsMatrix
        {
            get { return oddsArray; }
            set
            {
                oddsArray = value;
                odds = oddsArray.Sum(p=>p*100);
            }
        }
        public double odds = 0;
        public string pocketHand = "";
        public string currentHand = "";
        public uint handRank = 0;
        public bool isPlayable = false;
        public int maxPot = 0;
        public int table_pos = 0;
        public int outs = 0;
        public bool hasStraightDraw = false;
        public bool hasFlushDraw = false;
        public bool hasDraw { get { return hasStraightDraw || hasFlushDraw; } }
        public string handNotes = "";

        public bool isAlive = true;

        public  static long threadLock = 0;
        public static bool useCache = false, calculateOdds=true;
        public static uint cacheHits = 0, nonCacheHits = 0;
        public static Dictionary<ulong, double[]> cachedOdds = new Dictionary<ulong, double[]>();

        public Player()
        {

        }

        public Player(int table_pos, string username, int chips)
        {
            this.table_pos = table_pos;
            name = username;
            this.chips = chips;
        }

        public void Reset()
        {
            folded = false;
            maxPot = bet = 0;
            all_in = false;
            check = false;
            call = false;
            raise = false;
            cards = new List<Card>();
        }

        public void Die()
        {
            if (isAlive)
            {
                isAlive = false;
                Console.WriteLine(name + " has busted out.");
            }
        }

        public void DealCard(Card c)
        {
            if (cards.Count >= 2) cards.Clear();
            cards.Add(c);
        }

        public void DealCards(Card c1, Card c2) {
            cards = new List<Card>() { c1, c2 };
        }

        public void Fold()
        {
            folded = true;
        }

        public void Check()
        {
            check = true;
        }
        public void Call()
        {
            call = true;
        }

        public void Raise()
        {
            raise = true;
        }

        public void ClearBet()
        {
            bet = 0;
            check = false;
            call = false;
            raise = false;
        }

        public virtual void UpdateChips(int currentChips)
        {

        }

        public virtual void WonChips(int amt)
        {

        }

        public virtual void LostChips(int amt)
        {

        }



        public virtual PokerAction BestAction(int currentCall, int minBet, int maxEnemyChips, int minEnemyChips, int averageEnemyChips, int numEnemies)
        {
            if (!isAlive || folded || chips<=0)
            {
                return PokerAction.Skip;
            }

            if (oddsMatrix == null) return PokerAction.Allin;

            if (odds>=70)
            {
                return PokerAction.Allin;
            }
            if (currentHand != "High card" && currentCall<chips/2)
                if (currentCall >minBet) return PokerAction.Call;
                else 
                    if (currentCall == 0) return PokerAction.Bet;
                    else return PokerAction.Raise;

            if (currentCall>0 && currentCall <= chips / 50) return PokerAction.Call;
            
            return PokerAction.Fold;
        }

        public void EvaluateHand(Card flop1 = null, Card flop2 = null, Card flop3 = null, Card turn = null, Card river = null)
        {
            if (!isAlive) return;

            int count = 0;
            double[] oddsList = new double[9];
            double[] opponent = new double[9];

            string Pocket = cards[0] + " " + cards[1];
            string Board = flop1 + " " + flop2 + " " + flop3 + " " + turn + " " + river;

            ulong mask = HoldemHand.Hand.ParseHand(Pocket + " " + Board, ref count);

            if (calculateOdds)
            {
                if (useCache && cachedOdds.ContainsKey(mask))
                {
                    oddsList = cachedOdds[mask];
                    cacheHits++;
                }
                else
                {
                    if (flop1 == null) // not threadsafe if no board, as it uses a non-threadsafe dictionary for pockets
                        while (Interlocked.Increment(ref threadLock) > 1)
                        {
                            Interlocked.Decrement(ref threadLock);
                        }

                    HoldemHand.Hand.HandPlayerOpponentOdds(Pocket, Board, ref oddsList, ref opponent);

                    if (flop1 == null) 
                        Interlocked.Decrement(ref threadLock);

                    if (useCache)
                    {
                        while (Interlocked.Increment(ref threadLock) > 1)
                        {
                            Interlocked.Decrement(ref threadLock);
                        }
                        cachedOdds[mask] = oddsList;
                        Interlocked.Decrement(ref threadLock);
                        nonCacheHits++;
                    }
                    
                }

                oddsMatrix = oddsList;
            }

            currentHand = HoldemHand.Hand.DescriptionFromMask(mask).Replace("A straight", "Straight").Replace("A flush", "Flush").Replace("A pair", "Pair").Replace("A fullhouse", "Fullhouse").Split(new string[] { "," }, StringSplitOptions.None).First().Split(new string[] { " (" }, StringSplitOptions.None).First().Split(new string[] { ":" }, StringSplitOptions.None).First().Trim();
            hasFlushDraw = false;
            hasStraightDraw = false;
            outs = 0;
            if (flop1 == null)
            {
                if (currentHand == "One pair") currentHand = "Pocket pair";
                else if (currentHand == "High card" && HoldemHand.Hand.IsSuited(mask) && HoldemHand.Hand.GapCount(mask) <= 3) currentHand = "Suited connected";
                else if (currentHand == "High card" && HoldemHand.Hand.GapCount(mask) <= 3) currentHand = "Pocket connected";
                else if (currentHand == "High card" && HoldemHand.Hand.IsSuited(mask)) currentHand = "Pocket suited";

                pocketHand = currentHand;
            }
            else if (turn == null || river == null)
            {
                outs = HoldemHand.Hand.Outs(HoldemHand.Hand.ParseHand(Pocket), HoldemHand.Hand.ParseHand(Board));

                if (!currentHand.ToLower().Contains("straight"))
                {
                    List<Card> allCards = new List<Card>() { cards[0], cards[1], flop1, flop2, flop3 };
                    if (turn != null) allCards.Add(turn);
                    hasStraightDraw = Card.HasStraightDraw(allCards);
                }

                if (!currentHand.ToLower().Contains("flush"))
                {
                    List<Suite> allSuites = new List<Suite> { flop1.suite, flop2.suite, flop3.suite, cards[0].suite, cards[1].suite };
                    if (turn != null) allSuites.Add(turn.suite);
                    if (allSuites.GroupBy(s => s).Max(s => s.Count()) == 4) hasFlushDraw = true;
                }

            }
            handRank = HoldemHand.Hand.Evaluate(mask, count);

        }

        public bool SetHandNotes(string s="")
        {
            handNotes = s;
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public enum PokerAction
    {
        Fold = 0,
        Allin,
        Check,
        Call,
        Skip,
        Bet,
        Raise
    }
}
