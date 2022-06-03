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
    public enum Rank : int
    {
        Two = 2,
        Three = 3,
        Four = 5,
        Five = 7,
        Six = 11,
        Seven = 13,
        Eight = 17,
        Nine = 19,
        Ten = 23,
        Jack = 29,
        Queen = 31,
        King = 37,
        Ace = 41
    }
    public enum Suite : int
    {
        Spade,
        Heart,
        Club,
        Diamond
    }
    public enum CardToStringFormatEnum
    {
        ShortCardName,
        LongCardName
    }

    public class Card
    {
        public Rank rank { get; set; }
        public Suite suite { get; set; }
        public float accuracy = 1.0f;

        public static readonly Dictionary<Rank, string> RankName = new Dictionary<Rank, string>()
        {
            {Rank.Two, "2"},
            {Rank.Three, "3"},
            {Rank.Four, "4"},
            {Rank.Five, "5"},
            {Rank.Six, "6"},
            {Rank.Seven, "7"},
            {Rank.Eight, "8"},
            {Rank.Nine, "9"},
            {Rank.Ten, "T"},
            {Rank.Jack, "J"},
            {Rank.Queen, "Q"},
            {Rank.King, "K"},
            {Rank.Ace, "A"},
        };

        public static readonly Dictionary<Suite, string> SuiteName = new Dictionary<Suite, string>()
        {
            {Suite.Diamond, "d"},
            {Suite.Club, "c"},
            {Suite.Heart, "h"},
            {Suite.Spade, "s"}
        };

        public Card(Rank rank, Suite suite) {
            this.rank = rank;
            this.suite = suite;        
        }

        public override string ToString()
        {
            return RankName[rank] + SuiteName[suite];
        }

        public string ToString(CardToStringFormatEnum format)
        {
            switch (format)
            {
                case CardToStringFormatEnum.LongCardName:
                    {
                        return rank.ToString() + " of " + suite.ToString();
                    }

                case CardToStringFormatEnum.ShortCardName:
                    {
                        switch (rank)
                        {
                            case Rank.Two:
                                {
                                    return "2" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Three:
                                {
                                    return "3" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Four:
                                {
                                    return "4" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Five:
                                {
                                    return "5" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Six:
                                {
                                    return "6" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Seven:
                                {
                                    return "7" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Eight:
                                {
                                    return "8" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Nine:
                                {
                                    return "9" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Ten:
                                {
                                    return "T" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Jack:
                                {
                                    return "J" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Queen:
                                {
                                    return "Q" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.King:
                                {
                                    return "K" + suite.ToString().Substring(0, 1).ToLower();
                                }

                            case Rank.Ace:
                                {
                                    return "A" + suite.ToString().Substring(0, 1).ToLower();
                                }
                        }

                        break;
                    }
            }

            return "<Card value not set>";
        }

        public string NiceName()
        {
            return ToString().Replace("T", "10");
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            return ((Card)(obj)).rank == this.rank && ((Card)(obj)).suite == this.suite;
        }
        public static bool operator ==(Card a, Card b)
        {
            if (object.ReferenceEquals(a, null)) return object.ReferenceEquals(b, null);
            return a.Equals(b);
        }
        public static bool operator !=(Card a, Card b)
        {
            return !(a == b);
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public float accuracyAdjustment { 
            
            get {

                if (rank == Rank.Five && suite == Suite.Club) return -.01f;
                if (rank == Rank.Five && suite == Suite.Heart) return -.018f;
                if (rank == Rank.Six && suite == Suite.Spade) return -.01f;
                if (rank == Rank.Seven && suite == Suite.Club) return -.01f;
                switch(rank) {
                    case Rank.King:
                        return -.015f;
                    case Rank.Queen:
                        return -.02f;
                    case Rank.Jack:
                        return -.015f;
                    //case Rank.Nine:
                        //return .01f;
                    case Rank.Eight:
                        return .01f;
                    case Rank.Six:
                        return -.01f;
                    case Rank.Three:
                        return -.01f;
                    case Rank.Two:
                        return -.02f;
                    default:
                        return 0f;
                }
            }
        }

        public List<Rank> looksSimilarTo
        {
            get
            {
                switch (rank)
                {
                    case Rank.Ace:
                        return new List<Rank>() { Rank.Ace };
                    case Rank.Nine:
                        return new List<Rank>() { Rank.Eight };
                    case Rank.Eight:
                        return new List<Rank>() {Rank.Nine, Rank.Seven};
                    case Rank.Six:
                        return new List<Rank>() { Rank.Five, Rank.Four };
                    case Rank.Five:
                        return new List<Rank>() { Rank.Six, Rank.Four };
                    case Rank.Four:
                        return new List<Rank>() { Rank.Five};
                    case Rank.Three:
                        return new List<Rank>() { Rank.Two, Rank.Three };
                    case Rank.Two:
                        return new List<Rank>() { Rank.Three, Rank.Two };
                    default:
                        return null;
                }
            }
        }

        //////// Hand calculations
        public static bool HasStraightDraw(List<Card> cards, int maxGaps=1)
        {
            //Console.WriteLine("Checking for straight draw on " + string.Join(", ", cards.Select(c => c.rank.ToString())));
            Dictionary<Rank, int> gapsFrom = new Dictionary<Rank, int>();
            HashSet<Rank> ranks = new HashSet<Rank>();
            foreach (Card card in cards) ranks.Add(card.rank);
            foreach (Rank r in Card.RankName.Keys.OrderByDescending(k => k))
            {

                if (!gapsFrom.ContainsKey(r)) gapsFrom[r] = 0;
                int c = 0;
                if (!ranks.Contains(r))
                {
                    foreach (Rank rr in gapsFrom.Keys.OrderBy(k => k))
                    {
                        if (rr >= Rank.Five)
                        {
                            gapsFrom[rr]++;
                            
                        }
                        c++;
                        
                        if (c >= 5) break;
                    }
                }
            }
            // Special case for Ace low
            int cc = 0;
            if (!ranks.Contains(Rank.Ace))
            {
                foreach (Rank rr in gapsFrom.Keys.OrderBy(k => k))
                {
                    if (rr >= Rank.Five)
                    {
                        gapsFrom[rr]++;
                        
                    }
                    cc++;
                    if (cc >= 4) break;
                }
            }
            if (gapsFrom.Where(kv => kv.Key >= Rank.Five && kv.Value <= maxGaps && kv.Value > 0).Any())
            {
                //Console.WriteLine("Found straight draw to " + gapsFrom.Where(kv => kv.Value <= maxGaps && kv.Value > 0).First().Key+" with "+maxGaps+" gaps.");
                return true;
            }
            //foreach (KeyValuePair<Rank, int> kv in gapsFrom) Console.WriteLine(" - " + kv.Key + " = " + kv.Value);
            return false;
        }
        public static Rank? HasStraight(List<Card> cards, int gaps=0, bool returnLowestMissing=false)
        {
            //Console.WriteLine("Checking for straight draw on " + string.Join(", ", cards.Select(c => c.rank.ToString())));
            Dictionary<Rank, int> gapsFrom = new Dictionary<Rank, int>();
            Dictionary<Rank, Rank> lowestGapFrom = new Dictionary<Rank, Rank>();
            HashSet<Rank> ranks = new HashSet<Rank>();
            foreach (Card card in cards) ranks.Add(card.rank);
            foreach (Rank r in Card.RankName.Keys.OrderByDescending(k => k))
            {
                //Console.WriteLine(r + ": ");
                if (!gapsFrom.ContainsKey(r))
                {
                    gapsFrom[r] = 0;
                    lowestGapFrom[r] = Rank.Ace;
                }
                int c = 0;
                if (!ranks.Contains(r))
                {
                    foreach (Rank rr in gapsFrom.Keys.OrderBy(k => k))
                    {
                        if (rr >= Rank.Five)
                        {
                            gapsFrom[rr]++;
                            lowestGapFrom[rr] = r;
                            //Console.WriteLine(" - " + rr + ": " + gapsFrom[rr]);
                        }
                        c++;

                        if (c >= 5) break;
                    }
                }
            }
            // Special case for Ace low
            int cc = 0;
            if (!ranks.Contains(Rank.Ace))
            {
                foreach (Rank rr in gapsFrom.Keys.OrderBy(k => k))
                {
                    if (rr >= Rank.Five)
                    {
                        gapsFrom[rr]++;

                    }
                    cc++;
                    if (cc >= 4) break;
                }
            }
            if (gapsFrom.Where(kv => kv.Key>=Rank.Five && kv.Value <=gaps).Any())
            {
                //Console.WriteLine("Found straight to " + gapsFrom.Where(kv => kv.Value == 0).First().Key+".");
                Rank highest= gapsFrom.Where(kv => kv.Key >= Rank.Five && kv.Value <= gaps).Max(c => c.Key);
                if (returnLowestMissing) return lowestGapFrom[highest];
                else return highest;
            }
            //foreach (KeyValuePair<Rank, int> kv in gapsFrom) Console.WriteLine(" - " + kv.Key + " = " + kv.Value);
            return null;
        }
        public static bool HasStraightDrawOpenEnded(List<Card> cards)
        {
            //Console.WriteLine("Checking for straight draw on " + string.Join(", ", cards.Select(c => c.rank.ToString())));
            Dictionary<Rank, int> gapsFrom = new Dictionary<Rank, int>();
            Dictionary<Rank, Rank> lowestGapFrom = new Dictionary<Rank, Rank>();
            HashSet<Rank> ranks = new HashSet<Rank>();
            foreach (Card card in cards) ranks.Add(card.rank);
            foreach (Rank r in Card.RankName.Keys.OrderByDescending(k => k))
            {
                if (r == Rank.Ace) continue;
                //Console.WriteLine(r + ": ");
                if (!gapsFrom.ContainsKey(r))
                {
                    gapsFrom[r] = 0;
                    lowestGapFrom[r] = Rank.Ace;
                }
                int c = 0;
                if (!ranks.Contains(r))
                {
                    foreach (Rank rr in gapsFrom.Keys.OrderBy(k => k))
                    {
                        if (rr >= Rank.Five)
                        {
                            gapsFrom[rr]++;
                            lowestGapFrom[rr] = r;
                            //Console.WriteLine(" - " + rr + ": " + gapsFrom[rr]);
                        }
                        c++;

                        if (c >= 4) break;
                    }
                }
            }
            // Special case for Ace low
            int cc = 0;
            if (!ranks.Contains(Rank.Ace))
            {
                foreach (Rank rr in gapsFrom.Keys.OrderBy(k => k))
                {
                    if (rr >= Rank.Five)
                    {
                        gapsFrom[rr]++;

                    }
                    cc++;
                    if (cc >= 3) break;
                }
            }
            if (gapsFrom.Where(kv => kv.Key >= Rank.Five && kv.Value <= 0).Any())
            {
                return true;
            }
            //foreach (KeyValuePair<Rank, int> kv in gapsFrom) Console.WriteLine(" - " + kv.Key + " = " + kv.Value);
            return false;
        }
    }

  
}
