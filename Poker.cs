using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QLearner.Resources;
using Poker;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;

namespace QLearner.QStates
{
    public class Poker : QState
    {

        private bool enableGUI = false; // disable to simulate 1M games
        private bool showAllCards = false;
        private bool hideOwnCards = false;

        // Useful settings for simulating a million games
        private int numHandsToSimulate = 10000;
        private bool showHandStats = true; // show stats of what hands win each game
        private bool calculateOdds = true; // expensive, turn off for 10x faster and to have bots just play everything (raw stats). 
        private bool useCache = true; // Speeds up the above.  Download precomputed cache in link in readme
        private bool resetEachHandInNonGUIMode = false; // turn on for simulations to reset every hand, otherwise resets after player1 dies or all other players die
        
        // Modify this function if you want to test strategy ideas or AI code (the player argument passed here is an instance of PlayerAI with its BestAction) 
        // flag for enableGUI must be false
        // If not implement, it defaults to same logic as other bots when no GUI to take human input
        private PokerAction CustomStrategy(Player player, int currentCall, int minBet, int avgBet, int maxEnemyChips, int minEnemyChips, int enemyAvgChips, int numEnemies)
        {
            return player.BestAction(currentCall, minBet, maxEnemyChips, minEnemyChips, enemyAvgChips, numEnemies);
        }

        // Settings for pulling data from a live poker game on screen
        private bool useLiveData = false; // Set true for playing as a bot against real games on screen, must be configured to read your specific game/gui.

        private int refreshRate = 100;

        private bool reset = true;

        private int small_blind = 1;
        private int big_blind = 3;
        private int stack = 100;
        private int num_players = 8;
        private int ai_position = 0;
        private List<Player> players = new List<Player>();
        private int dealerPos = -1, smallBlindPos=-1, bigBlindPos=-1;
        private int pot;
        private List<Card> table_cards;


        private BackgroundWorker backgroundProcess = new BackgroundWorker();

        private uint handCount = 0;
        private int gamesPlayed = 0, gamesWon = 0, gamesSecondPlace=0, handsWon=0, handsPlayed=0;

        private PokerGUI gui;

        private Dictionary<string, int> winningHandCounts = new Dictionary<string, int>();
        private Dictionary<string, int> startingVsWinningHandCounts = new Dictionary<string, int>();

       
        private Deck deck = new Deck();

        

        public readonly QAction ALLIN = new QAction_String("ALLIN");
        public readonly QAction FOLD = new QAction_String("FOLD");
        public readonly QAction CALL = new QAction_String("CALL");
        public readonly QAction CHECK = new QAction_String("CHECK");
        public readonly QAction BET = new QAction_String("BET");
        public readonly QAction RAISE = new QAction_String("RAISE");

        private QAction AI_Action = null;

        private ManualResetEvent wait;

        private Poker master;

        [STAThread]
        public static void Main(string[] args=null)
        {
            try
            {
                IntPtr ptr = GetConsoleWindow();
                MoveWindow(ptr, 0, 0, Screen.PrimaryScreen.Bounds.Width / 3, Screen.PrimaryScreen.Bounds.Height / 2, true);

                new Poker().StandaloneRun();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
                Console.Read();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        

        public void StandaloneRun()
        {
            
            if (useLiveData) PlayLive();
            else if (enableGUI)
            {
                useCache = false;

                LaunchGUI();
            }

            else
            {
                if (!calculateOdds) useCache = false;

                if (useCache) Player.useCache = true;
                Player.calculateOdds = calculateOdds;

                if (useCache && File.Exists("cache"))
                {
                    WriteLine("Loading cached odds...");
                    Player.cachedOdds = OpenDictionaryULongDoubleFromFile("cache");
                    WriteLine("Loaded " + Player.cachedOdds.Count + " cached odds.");
                }

                DateTime start = DateTime.Now;

                for (int i = 0; i < numHandsToSimulate; i++)
                {
                    StartHand();
                    Play();
                }
                if (showHandStats)
                {
                    foreach (KeyValuePair<string, int> kv in winningHandCounts.OrderByDescending(kv => kv.Value))
                    WriteLine(kv.Key + ": " + kv.Value + " (" + (100.0 * kv.Value / handCount).ToString("N2") + "%)");
                }
                
                if (useCache)
                {
                    WriteLine("Cached calculations: " + Player.cachedOdds.Count);
                    WriteLine("Cache Hits: " + Player.cacheHits + "/" + (Player.cacheHits + Player.nonCacheHits) + " (" + (100.0 * Player.cacheHits / (Player.cacheHits + Player.nonCacheHits)).ToString("N1") + "%)");
                }
                WriteLine("Completed in " + (DateTime.Now - start));


                if (useCache && Player.nonCacheHits > 0 && Player.cacheHits < Player.nonCacheHits)
                {
                    WriteLine("Saving cached odds (" + Player.cachedOdds.Count + ") ...");
                    SaveDictionaryULongDoubleToFile("cache", Player.cachedOdds);
                    WriteLine("Finished saving cached odds.");

                }
                Console.ReadLine();
            }
        }

        public void LaunchGUI(object obj=null)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            gui = new PokerGUI(this);
            backgroundProcess.WorkerSupportsCancellation = true;
            backgroundProcess.DoWork += (o, s) => { StartHand(); };
            backgroundProcess.RunWorkerCompleted += (o, s) =>
            {
                if (obj != null)
                {
                    ManualResetEvent w = (ManualResetEvent)((object[])obj)[0];
                    w.Set();
                }
                else Play();
            };
            backgroundProcess.RunWorkerAsync();
            
            Application.Run(gui);
        }

        

        public void StartHand()
        {

            if (reset || (resetEachHandInNonGUIMode && gui==null) || (gui==null && !players[0].isAlive) || (gui==null && !players.Where(p=>p.isAlive&&!p.isPlayable).Any()))
            {
                
                WriteLine("Resetting players and chips...");
                players.Clear();
                dealerPos = bigBlindPos = smallBlindPos = -1;
                reset = false;
                gamesPlayed++;
                //Console.ReadLine();
                
            }

            if (!players.Any())
            {
                WriteLine("Initiating " + num_players + " players...");
                for (int i = 1; i <= num_players; i++)
                {
                    Player p;

                    if (i > 1) p = new Player(i, "Player" + i, stack);
                    else p = new PlayerAI(i, "Player" + i, stack); // AI is in the same slot as the player

                    players.Add(p);
                    if (i == 1) p.isPlayable = true;
                }

            }


            handCount++;
            WriteLine("Starting new hand #" + handCount + "...");


            deck.Shuffle();

            if (gui != null)
            {
                gui.ClearWinningPlayer();
                gui.ClearPlayerActions();
                gui.ClearTableCards();
                gui.RefreshPlayer(players[0]);
                gui.SetPot(0);
                gui.ClearOddsGrid();
            }

            foreach (Player player in players)
            {
                player.Reset();
            }
            for (int i = 0; i < 2; i++)
            {
                foreach (Player player in players.Where(p => p.isAlive))
                {
                    if (useLiveData && player.isPlayable && Live.card1 != null && Live.card2 != null)
                    {
                        if (player.cards.Count < 2)
                        {
                            Card c1 = deck.DealCard(Live.card1);
                            Card c2 = deck.DealCard(Live.card2);
                            player.DealCards(c1, c2);
                        }
                    }
                    else
                    {
                        Card c = deck.DealCard();
                        player.DealCard(c);
                    }
                }
            }
            if (players[0].isAlive && !hideOwnCards)
            {
                if (gui != null) gui.SetHand(players[0].cards[0], players[0].cards[1]);
                WriteLine("Player hand: " + players[0].cards[0] + " " + players[0].cards[1]);
            }
            else if (gui != null) gui.ClearHand();

            if (gui != null)
            {
                foreach (Player player in players.Where(p => !p.isPlayable))
                {
                    gui.RefreshPlayer(player);
                    if (showAllCards && player.isAlive)
                    {
                        gui.ShowPlayerHand(player);
                    }
                    else gui.ClearPlayerHand(player);
                }
            }

            EvaluateAllPlayerHands(); // multithreaded



            if (dealerPos == -1)
            {
                dealerPos = new Random().Next(0, num_players);
                smallBlindPos = GetNextValidPlayer(dealerPos);
                bigBlindPos = GetNextValidPlayer(smallBlindPos);
                WriteLine("Initial positions: dealer=" + dealerPos + ", smallBlind=" + smallBlindPos + ", bigBlind=" + bigBlindPos);
            }

            players[bigBlindPos].bet = Math.Min(players[bigBlindPos].chips, big_blind);
            players[smallBlindPos].bet = Math.Min(players[smallBlindPos].chips, small_blind);

            if (gui != null)
            {
                gui.RefreshPlayer(players[0]);
                gui.SetDealer(players[dealerPos].name);
                gui.SetPlayerSmallBlind(players[smallBlindPos]);
                gui.SetPlayerBigBlind(players[bigBlindPos]);
            }
        }

        public void Play()
        {
            

            CollectBets(bigBlindPos);

            if (!players[0].folded) handsPlayed++;

            table_cards = new List<Card>();
            

            deck.Burn();
            Card flop1 = deck.DealCard();
            Card flop2 = deck.DealCard();
            Card flop3 = deck.DealCard();

            table_cards.Add(flop1);
            table_cards.Add(flop2);
            table_cards.Add(flop3);

            EvaluateAllPlayerHands(flop1, flop2, flop3);

            Player bestFlopPlayer = players.Where(p=>p.isAlive && !p.folded).OrderByDescending(p => p.handRank).First();
            WriteLine(bestFlopPlayer + " leading flop with " + bestFlopPlayer.currentHand);
            string bestFlopHandType = bestFlopPlayer.currentHand;
            
            if (gui != null)
            {
                gui.SetFlopCards(flop1, flop2, flop3);
            }

            CollectBets(dealerPos);


            deck.Burn();
            Card turn = deck.DealCard();
            table_cards.Add(turn);

            EvaluateAllPlayerHands(flop1, flop2, flop3, turn);

            if (gui != null)
            {
                gui.SetTurnCard(turn);
            }

            CollectBets(dealerPos);


            deck.Burn();
            Card river = deck.DealCard();
            table_cards.Add(river);
            EvaluateAllPlayerHands(flop1, flop2, flop3, turn, river);

            if (gui != null)
            {
                gui.SetRiverCard(river);
            }

            CollectBets(dealerPos);

            if (players[0].isAlive && hideOwnCards)
            {
                if (gui != null) gui.SetHand(players[0].cards[0], players[0].cards[1]);
                WriteLine("Player hand: " + players[0].cards[0] + " " + players[0].cards[1]);
                gui.SetWinningOdds(players[0].oddsMatrix);
                gui.SetHandType(players[0].currentHand);
            }

            if (gui != null)
            {
                players.ForEach(p => { if (!p.isPlayable && p.isAlive) { gui.ShowPlayerHand(p); } });
            }

            Player bestPlayer = players.Where(p=> !p.folded && p.isAlive).OrderByDescending(p => p.handRank).First();
            
            

            IEnumerable<Player> tiedPlayers = players.Where(p => p.isAlive && p.handRank == bestPlayer.handRank && !p.folded);
            int numWinners = tiedPlayers.Count();
            if (numWinners == 1)
            {
                WriteLine(bestPlayer + " wins with " + bestPlayer.currentHand);
                if (gui != null) gui.SetWinningPlayers(new List<Player>(){bestPlayer});

                bestPlayer.chips += bestPlayer.maxPot;
                pot -= bestPlayer.maxPot;
                bestPlayer.maxPot = 0;

                
            }
            else
            {
                WriteLine(string.Join(", ", tiedPlayers) + " tie with " + bestPlayer.currentHand);
                
                foreach (Player p in tiedPlayers)
                {
                    int award = p.maxPot / numWinners;
                    p.chips += award;
                    pot -= award;
                    p.maxPot = 0;
                }
                if (pot == 1) { tiedPlayers.First().chips += 1; pot = 0; }
                if (gui != null) gui.SetWinningPlayers(tiedPlayers.ToList());
            }
            if (pot > 0)
            {
                WriteLine("Remaining " + pot + " chips returned to losing players.");
                foreach (Player player in players.Where(p => !p.folded && p.isAlive && p.maxPot>0).OrderByDescending(p => p.handRank))
                {
                    tiedPlayers = players.Where(p => p.isAlive && p.handRank == player.handRank && !p.folded);
                    numWinners = tiedPlayers.Count();
                    if (numWinners == 1)
                    {
                        WriteLine(player + " wins remaining chips with " + player.currentHand);

                        int maxAward = Math.Min(pot, player.maxPot);
                        player.chips += maxAward;
                        pot -= maxAward;
                        player.maxPot = 0;
                    }
                    else
                    {
                        WriteLine(string.Join(", ", tiedPlayers) + " tie for remaining chips with " + bestPlayer.currentHand);

                        foreach (Player p in tiedPlayers)
                        {
                            int award = Math.Min(pot, p.maxPot) / numWinners;
                            p.chips += award;
                            pot -= award;
                            p.maxPot = 0;
                        }
                        if (pot == 1) { tiedPlayers.First().chips += 1; pot = 0; }
                    }
                    if (pot <= 0) break;
                }
            }


            string winningHand = bestPlayer.currentHand;
            string startingVsWinningHandType = bestPlayer.pocketHand+" => "+ bestPlayer.currentHand;

            if (winningHandCounts.ContainsKey(winningHand))
            {
                winningHandCounts[winningHand]++;
            }
            else winningHandCounts[winningHand] = 1;

            if (startingVsWinningHandCounts.ContainsKey(startingVsWinningHandType))
                startingVsWinningHandCounts[startingVsWinningHandType]++;
            else startingVsWinningHandCounts[startingVsWinningHandType] = 0;


           
            pot = 0;

            if (gui != null)
            {
                Thread.Sleep(1000);
                gui.SetPot(pot);
                players.ForEach(p => gui.RefreshPlayer(p));
                gui.EnableRedeal();
            }

            int remainingOpponentsOld = players.Where(p => p.isAlive && !p.isPlayable).Count();

            players.ForEach(p => { if (p.chips <= 0 && p.isAlive) p.Die(); });

            int remainingOpponents = players.Where(p => p.isAlive && !p.isPlayable).Count();

            if (bestPlayer.isPlayable) handsWon++;
            if (bestPlayer.isPlayable && (resetEachHandInNonGUIMode || remainingOpponents==0))
            {
                gamesWon++;
            }
            if (remainingOpponents <= 1 && remainingOpponentsOld > 1)
            {
                gamesSecondPlace++;
            }

            WriteLine("Remaining players ("+players.Where(p => p.isAlive).Count()+"): " + string.Join(", ", players.Where(p => p.isAlive).Select(p => p.name + " (" + p.chips + ")")));
            WriteLine("Total Hands Won: " + handsWon + "/" + handsPlayed + " (" + (100.0 * handsWon / handsPlayed).ToString("N0") + "%)");
            WriteLine("Total Games Won: " + gamesWon + "/" + gamesPlayed + " (" + (100.0 * gamesWon / gamesPlayed).ToString("N0") + "%) | Games Placed Top Two: " + gamesSecondPlace + "/" + gamesPlayed + " (" + (100.0 * gamesSecondPlace / gamesPlayed).ToString("N0") + "%)");

            dealerPos = GetNextValidPlayer(dealerPos);
            smallBlindPos = GetNextValidPlayer(dealerPos);
            bigBlindPos = GetNextValidPlayer(smallBlindPos);

            

        }

        public int GetNextValidPlayer(int startingPos)
        {
            int c = 0;
            int i = startingPos;
            while (c<players.Count)
            {
                i++;
                if (i >= players.Count) i = 0;
                if (players[i].isAlive) return i;
            }
            return startingPos;
        }


        public void HandlePlayerAction(Player player)
        {
            if (player.all_in || player.folded || !player.isAlive || player.chips<=0 || players.Where(p=>p.isAlive && !p.folded && p.chips>0).Count()<=1) return;
            int maxBet = players.Max(p=>p.bet);
            gui.ToggleBetRaise(maxBet > 0);
            PokerAction action = gui.getPlayerAction(player, (player.bet>=maxBet), Math.Max(big_blind, maxBet) );
            WriteLine("Action selected: "+action.ToString());
            switch (action)
            {
                case PokerAction.Fold:
                    player.Fold();
                    break;
                case PokerAction.Check:
                    player.Check();
                    break;
                case PokerAction.Call:
                    player.Call();
                    player.bet = Math.Min(player.chips, maxBet);
                    break;
                case PokerAction.Bet:
                    if (maxBet > 0)
                    {
                        player.Raise();
                        player.bet += maxBet;
                    }
                    player.bet = Math.Min(player.bet, player.chips);
                    break;
                case PokerAction.Allin:
                    player.bet = player.chips;
                    player.all_in = true;
                    break;
                case PokerAction.Skip:
                    break;
            }
            if (gui != null)
            {
                gui.RefreshPlayer(player);
                gui.SetPot(pot);
                Thread.Sleep(500);
            }

        }


        public void PlaceBet(Player player)
        {
            if (player.all_in || player.folded || !player.isAlive || player.chips <= 0 || players.Where(p => p.isAlive && !p.folded && p.chips > 0).Count() <= 1) return;

            int maxBet = players.Max(p => p.bet);
            PokerAction action = player.BestAction(maxBet, big_blind, players.Where(p => !p.isPlayable).Max(p => p.chips), players.Where(p => !p.isPlayable).Min(p => p.chips), (int)players.Where(p => !p.isPlayable).Average(p => p.chips), players.Where(p => p.isAlive && !p.folded).Count());

            if (player.table_pos == ai_position && AI_Action != null)
            {
                if (AI_Action == ALLIN) action = PokerAction.Allin;
                else if (AI_Action == FOLD) action = PokerAction.Fold;
                else action = PokerAction.Check;
            }
            else if (player.isPlayable && gui == null) action = CustomStrategy(player, maxBet, big_blind, maxBet, players.Where(p => !p.isPlayable).Max(p => p.chips), players.Where(p => !p.isPlayable).Min(p => p.chips), (int)players.Where(p => !p.isPlayable).Average(p => p.chips), players.Where(p => !p.isPlayable && p.isAlive && !p.folded).Count());

            if (action == PokerAction.Fold && player.bet == maxBet) action = PokerAction.Check;
            if (action == PokerAction.Check && player.bet < maxBet) action = PokerAction.Fold;
            switch (action)
            {
                case PokerAction.Fold:
                    player.Fold();
                    break;
                case PokerAction.Check:
                    player.Check();
                    break;
                case PokerAction.Allin:
                    player.bet = player.chips;
                    player.all_in = true;
                    break;
                case PokerAction.Call:
                    player.Call();
                    player.bet = Math.Min(player.chips, maxBet);
                    break;
                case PokerAction.Bet:
                    player.bet = Math.Min(player.chips, big_blind);
                    break;
                case PokerAction.Raise:
                    player.bet = Math.Min(player.chips, maxBet + big_blind);
                    player.Raise();
                    break;
                case PokerAction.Skip:
                    break;
            }
            if (gui != null)
            {
                gui.RefreshPlayer(player);
                gui.SetPot(pot);
            }
        }

        public void CollectBets(int startingPos)
        {
            if (gui != null) Thread.Sleep(refreshRate);
            int i = GetNextValidPlayer(startingPos);
            int currentBet = players[startingPos].bet;
            bool called = false;
            while (i != startingPos)
            {
                //WriteLine(players[i].name + "'s turn. Started at " + players[startingPos].name);
                if (i == 0 && enableGUI)
                {
                    HandlePlayerAction(players[i]);
                }
                else
                {
                    PlaceBet(players[i]);
                }
                if (players[i].bet > currentBet)
                {
                    startingPos = i;
                    currentBet = players[i].bet;
                    called = false;
                }
                else if (players[i].bet == currentBet) called = true;

                i = GetNextValidPlayer(i);
                if(gui!=null) Thread.Sleep(refreshRate);
            }
            if (gui != null) Thread.Sleep(refreshRate);

            if (!called) players[startingPos].bet = players.Where(p => p.name != players[startingPos].name).Max(p => p.bet);
            players.ForEach(p => p.maxPot += players.Where(q => q.isAlive).Sum(q => Math.Min(q.bet, p.bet)));
            pot += players.Sum(p => p.bet);
            players.ForEach(p => p.chips -= p.bet);
            players.ForEach(p => p.ClearBet());
            if (gui != null)
            {
                players.ForEach(p => gui.RefreshPlayer(p));
                gui.SetPot(pot);
            }
        }


        public void EvaluateAllPlayerHands(Card flop1 = null, Card flop2 = null, Card flop3 = null, Card turn = null, Card river = null)
        {
            
            Parallel.ForEach(players, player=>
            {
                player.EvaluateHand(flop1, flop2, flop3, turn, river);
            });
            if (gui != null)
            {
                foreach (Player player in players.Where(p=>p.isAlive))
                {
                    if (!(hideOwnCards && player.isPlayable))
                    {
                        if (player.isPlayable || showAllCards)
                        {
                            gui.SetPlayerOdds(player, player.oddsMatrix);
                            gui.SetPlayerHandType(player, player.currentHand);
                        }
                    }
                }
            }
        }

        

        public bool WriteLine(string s, bool important=false)
        {
            Console.WriteLine(s);
            
            
            return true;
        }

        
        /// QLearner Interface ////////////////////////////////////////////////////////

        public override QState Start()
        {
            if (HideOutput) enableGUI = false;
            else enableGUI = true;

            master = this;
            WriteOutput("Initializing poker...", true);

            wait = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(new WaitCallback(LaunchGUI), new object[] { wait });
            wait.WaitOne();

            return this;
        }


        public override bool Equals(object obj)
        {
            return GetHashCode()==((Poker)obj).GetHashCode();
        }
        public override int GetHashCode()
        {
            if (!players.Any()) return 0;
            return players[ai_position].cards.GetHashCode();
        }
        public override QState GetNewState(QAction action)
        {
            AI_Action = action;
            Play();
            return this;
        }
        public override QAction[] GetActions()
        {

            return new QAction[]
            {
                ALLIN, FOLD
            };
        }
        
        public override decimal GetValue()
        {
            return players.Count > ai_position? players[ai_position].chips:0;
        }
        public override bool IsEnd()
        {
            return (players.Count>ai_position && !players[ai_position].isAlive) || !players.Where(p=>p.isAlive && p.table_pos!=ai_position).Any();
        }
        public override string ToString()
        {
            return players[ai_position].currentHand;
        }
        public override void Step()
        {
            
        }
        public override void End()
        {
           
        }
        public override bool HasSettings
        {
            get
            {
                return true;
            }
        }
        public override void Settings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>(){
                {"BuyIn", ""+stack},
                {"BigBlind", ""+big_blind},
                {"SmallBlind", ""+small_blind},
                {"Num.Players", ""+num_players},
                {"AIPlayerPosition", ""+ai_position}
            };
            Dictionary<string, string> values = NewSettingsForm(settings);
            int oldstack = stack;
            int oldplayers = num_players;
            try
            {
                stack = Convert.ToInt32(values["BuyIn"]);
                big_blind = Convert.ToInt32(values["BigBlind"]);
                small_blind = Convert.ToInt32(values["SmallBlind"]);
                num_players = Convert.ToInt32(values["Num.Players"]);
                ai_position = Convert.ToInt32(values["AIPlayerPosition"]);

                if (enableGUI)
                {
                    if (oldstack != stack || oldplayers != num_players)
                    {
                        reset = true;
                    }
                }
            }
            catch (Exception)
            {
                WriteOutput("Invalid settings! Please only use integer numbers.");
            }
        }
        public void BlindCards()
        {
            hideOwnCards = !hideOwnCards;
            if (hideOwnCards)
            {
                gui.ClearHand();
                gui.ClearOddsGrid();
            }
            else
            {
                gui.SetHand(players[0].cards[0], players[1].cards[1]);
                gui.SetHandType(players[0].currentHand);
                gui.SetWinningOdds(players[0].oddsMatrix);
            }
        }

        public override Dictionary<QFeature, decimal> GetFeatures(QAction action)
        {
            return QFeature_String.FromStringDecimalDictionary(new Dictionary<string, decimal>() {
                {"HandRank", players[ai_position].handRank},
                {"WinningOdds", (decimal)players[ai_position].oddsMatrix.Sum(x=>x)}
            });
        }

        public override decimal GetValueHeuristic()
        {
            return 0;
        }

        public override object Save()
        {
            return new object[] {};
        }

        public override QState Open(object o)
        {
            object[] oo = (object[])o;
            return new Poker() {  };
        }

        public static Dictionary<ulong, double[]> OpenDictionaryULongDoubleFromFile(string filename)
        {
            Dictionary<ulong, double[]> objectToSerialize;
            using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    
                    objectToSerialize = ProtoBuf.Serializer.Deserialize<Dictionary<ulong, double[]>>(stream);
                    stream.Close();
                    return objectToSerialize;
                }
                catch (Exception e)
                {
                    stream.Close();
                    throw e;
                }
            }
        }

        public static void SaveDictionaryULongDoubleToFile(string filename, Dictionary<ulong, double[]> objectToSerialize)
        {
            using (Stream stream = File.Open(filename + "_new", FileMode.Create))
            {
                    
                    ProtoBuf.Serializer.Serialize<Dictionary<ulong, double[]>>(stream, objectToSerialize.Where(kv => kv.Key != 0).Distinct().ToDictionary(x=>x.Key, x=>x.Value));
                    stream.Close();
                
            }
            if (System.IO.File.Exists(filename+"_backup")) System.IO.File.Delete(filename + "_backup");
            if (System.IO.File.Exists(filename)) System.IO.File.Move(filename, filename+"_backup");
            if (System.IO.File.Exists(filename + "_new")) System.IO.File.Move(filename + "_new", filename);
        }

        

        

        // Rough code for looping and playing a live on-screen poker game
        public void PlayLive()
        {
            PlayerAI ai = new PlayerAI(0, "", 0);

            while (true)
            {
                try
                {
                    
                        Live.LoadImages();
                  
                        Live.LoadScreen();

                        ai.DealCards(Live.card1, Live.card2);

                        if (Live.currentChips > 0) ai.UpdateChips(Live.currentChips);
                            
                        if (!Live.MyTurnIsReady) continue;

                        ai.PlayHand();

                }
                catch (Exception e) { WriteLine("Exception: " + e); Live.Reset(); }
            }
        }
    }
}
