using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QLearner.QStates;

namespace Poker
{
    public partial class PokerGUI : Form
    {
        private Dictionary<string, DataGridViewRow> playerRow = new Dictionary<string, DataGridViewRow>();
        private bool bet, fold, check, call, allin;
        private QLearner.QStates.Poker master;
        public PokerGUI(QLearner.QStates.Poker p)
        {
            master = p;
            InitializeComponent();

            DisableActionsButtons();
            DisableRedeal();

            checkBtn.Click += new EventHandler(check_Click);
            callBtn.Click += new EventHandler(callBtn_Click);
            allinBtn.Click += new EventHandler(allinBtn_Click);
            foldBtn.Click += new EventHandler(foldBtn_Click);
            betBtn.Click += new EventHandler(betBtn_Click);
            redeal.Click += new EventHandler(redeal_Click);
            settings.Click += new EventHandler(settings_Click);
            blindCards.Click += new EventHandler(blindCards_Click);
        }

        public void blindCards_Click(object sender, EventArgs e)
        {
            master.BlindCards();
        }

        public void settings_Click(object sender, EventArgs e)
        {
            master.Settings();
        }

        

        public void redeal_Click(object sender, EventArgs e)
        {
            DisableRedeal();
            DisableActionsButtons();
            master.StartHand();
            master.Play();
        }
        public PokerAction getPlayerAction(Player player, bool allowCheck, int minBet)
        {
            if (player.folded || player.all_in)
            {

                DisableActionsButtons();
                return PokerAction.Skip;
            }

            EnableActionsButtons(true, allowCheck, minBet, player.chips);

            for (int i = 0; i < 30 * 1000; i++)
            {
                if (isActionClicked())
                {
                    break;
                }
                System.Threading.Thread.Sleep(1);
                Application.DoEvents();
            }


            if (fold)
            {
                resetActions();
                return PokerAction.Fold;
            }
            if (allin)
            {
                resetActions();
                return PokerAction.Allin;
            }
            if (check)
            {
                resetActions();
                return PokerAction.Check;
            }
            if (call)
            {
                resetActions();
                return PokerAction.Call;
            }
            if (bet)
            {
                resetActions();
                player.bet = (int)betAmt.Value;
                return PokerAction.Bet;
            }
            resetActions();
            return PokerAction.Skip;
        }


        void resetActions()
        {
            fold = false;
            check = false;
            allin = false;
            call = false;
            bet = false;
        }

        bool isActionClicked()
        {
            return fold || allin || check || call || bet;
        }

        void check_Click(object sender, EventArgs e)
        {
            check = true;
            DisableActionsButtons();
        }
        void callBtn_Click(object sender, EventArgs e)
        {
            call = true;
            DisableActionsButtons();
        }

        void allinBtn_Click(object sender, EventArgs e)
        {
            allin = true;
            DisableActionsButtons();
        }

        void betBtn_Click(object sender, EventArgs e)
        {
            bet = true;
            DisableActionsButtons();
        }

        void foldBtn_Click(object sender, EventArgs e)
        {
            fold = true;
            DisableActionsButtons();
        }

        void DisableActionsButtons()
        {
            EnableActionsButtons(false);
        }

        private delegate void EnableActionsButtonsD(bool enable = true, bool check = true, int minBet=0, int chipsLeft=0);
        public void EnableActionsButtons(bool enable = true, bool check = true, int minBet = 0, int chipsLeft = 0)
        {
            if (InvokeRequired)
            {
                Invoke(new EnableActionsButtonsD(EnableActionsButtons), new object[] { enable, check, minBet, chipsLeft });
            }
            else
            {
                allinBtn.Enabled =
                foldBtn.Enabled = enable;
                
                if (enable)
                {
                    if (check) checkBtn.Enabled = enable;
                    else callBtn.Enabled = enable;

                    betAmt.Enabled =
                    betBtn.Enabled = (chipsLeft > minBet);

                    betAmt.Value = betAmt.Minimum = Math.Max(0, minBet);
                    betAmt.Maximum = Math.Max(0, chipsLeft - minBet);
                }
                else checkBtn.Enabled = 
                    betBtn.Enabled =
                    betAmt.Enabled = 
                    callBtn.Enabled = false;

                
            }
        }

        private delegate void ToggleBetRaiseD(bool raise);
        public void ToggleBetRaise(bool raise)
        {
            if (InvokeRequired)
            {
                Invoke(new ToggleBetRaiseD(ToggleBetRaise), new object[] { raise });
            }
            else
            {
                if (raise) betBtn.Text = "Raise";
                else betBtn.Text = "Bet";

            }
        }

        private delegate void EnableRedealD();
        public void EnableRedeal()
        {
            if (InvokeRequired)
            {
                Invoke(new EnableRedealD(EnableRedeal), new object[] { });
            }
            else
            {
                redeal.Enabled = true;
            }
        }
        private delegate void DisableRedealD();
        public void DisableRedeal()
        {
            if (InvokeRequired)
            {
                Invoke(new DisableRedealD(DisableRedeal), new object[] { });
            }
            else
            {
                redeal.Enabled = false;
            }
        }

        private delegate void QuitD();
        public void Quit()
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                Invoke(new QuitD(Quit));
            }
            else
            {
                Close();
            }
        }

      

        private delegate void SetHandD(Card card1, Card card2);
        public void SetHand(Card card1, Card card2)
        {
            if (InvokeRequired)
            {
                Invoke(new SetHandD(SetHand), new object[] { card1, card2 });
            }
            else
            {
                hand1.Text = card1.NiceName();
                hand2.Text = card2.NiceName();
                this.hand1.ForeColor = GetCardColor(card1);
                this.hand2.ForeColor = GetCardColor(card2);
            }
        }
        private delegate void ClearHandD();
        public void ClearHand()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearHandD(ClearHand), new object[] { });
            }
            else
            {
                hand1.Text = "";
                hand2.Text = "";
                HandType.Text = "";
            }
        }

        private delegate void SetHandTypeD(string description);
        public void SetHandType(string description)
        {
            if (InvokeRequired)
            {
                Invoke(new SetHandTypeD(SetHandType), new object[] { description });
            }
            else
            {
                HandType.Text = description;
            }
        }

        private delegate void SetPotD(int amount);
        public void SetPot(int amount)
        {
            if (InvokeRequired)
            {
                Invoke(new SetPotD(SetPot), new object[] { amount });
            }
            else
            {
                pot.Value = amount;
            }
        }

        private delegate void ClearTableCardsD();
        public void ClearTableCards()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearTableCardsD(ClearTableCards), new object[] { });
            }
            else
            {
                this.card1.Text =
                this.card2.Text =
                this.card3.Text =
                this.card4.Text =
                this.card5.Text = "";

                card1.Visible =
                    card2.Visible =
                    card3.Visible =
                    card4.Visible =
                    card5.Visible = false;
                Application.DoEvents();
            }
        }

        private delegate void SetFlopCardsD(Card card1, Card card2, Card card3);
        public void SetFlopCards(Card card1, Card card2, Card card3)
        {
            if (InvokeRequired)
            {
                Invoke(new SetFlopCardsD(SetFlopCards), new object[] { card1, card2, card3 });
            }
            else
            {
                this.card1.Text = card1.NiceName();
                this.card2.Text = card2.NiceName();
                this.card3.Text = card3.NiceName();
                this.card1.Visible = this.card2.Visible = this.card3.Visible = true;
                this.card1.ForeColor = GetCardColor(card1);
                this.card2.ForeColor = GetCardColor(card2);
                this.card3.ForeColor = GetCardColor(card3);
                Application.DoEvents();
            }
        }

        private delegate void SetTurnCardD(Card card4);
        public void SetTurnCard(Card card4)
        {
            if (InvokeRequired)
            {
                Invoke(new SetTurnCardD(SetTurnCard), new object[] { card4 });
            }
            else
            {
                this.card4.Text = card4.NiceName();
                this.card4.Visible = true;
                this.card4.ForeColor = GetCardColor(card4);
                Application.DoEvents();
            }
        }

        private delegate void SetRiverCardD(Card card5);
        public void SetRiverCard(Card card5)
        {
            if (InvokeRequired)
            {
                Invoke(new SetRiverCardD(SetRiverCard), new object[] { card5 });
            }
            else
            {
                this.card5.Text = card5.NiceName();
                this.card5.Visible = true;
                this.card5.ForeColor = GetCardColor(card5);
                Application.DoEvents();
            }
        }

        private Color GetCardColor(Card card)
        {
            return (card.suite == Suite.Heart || card.suite == Suite.Diamond ? Color.Red : Color.Black);
        }

        private delegate void RefreshPlayerD(Player player);
        public void RefreshPlayer(Player player)
        {
            if (InvokeRequired)
            {
                Invoke(new RefreshPlayerD(RefreshPlayer), new object[] { player });
            }
            else
            {
                try
                {

                    if (!player.isPlayable)
                    {

                        if (!playerRow.ContainsKey(player.name))
                            playerRow[player.name] = players.Rows[players.Rows.Add(false, player.name, player.chips, "", "", "")];
                        else playerRow[player.name].Cells["PlayerChips"].Value = player.chips;

                        if (player.bet > 0) playerRow[player.name].Cells["PlayerBet"].Value = player.bet;
                        else playerRow[player.name].Cells["PlayerBet"].Value = "";

                        playerRow[player.name].Cells["PlayerAction"].Value = !player.isAlive ? "Dead" : player.folded ? "Fold" : player.all_in ? "All-In" : player.raise ? "Raise" : player.call ? "Call" : player.bet > 0 ? "Bet" : player.check ? "Check" : "";
                        if (player.folded || !player.isAlive) playerRow[player.name].DefaultCellStyle.BackColor = Color.OrangeRed;
                    }
                    else
                    {
                        chips.Value = player.chips;
                        betTotal.Value = player.bet;
                        username.Text = player.name;
                    }
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(1);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    master.WriteLine("GUI update error: " + ex);
                }
            }
        }

        private delegate void SetDealerD(string playerName);
        public void SetDealer(string playerName)
        {
            if (InvokeRequired)
            {
                Invoke(new SetDealerD(SetDealer), new object[] { playerName });
            }
            else
            {
                foreach (DataGridViewRow r in players.Rows)
                {
                    if (r.Cells["PlayerName"].Value.ToString() == playerName)
                    {
                        r.Cells["PlayerDealer"].Value = true;
                    }
                    else r.Cells["PlayerDealer"].Value = false;
                }
                if (playerName == "Player1") dealer.Checked = true;
                else dealer.Checked = false;
                Application.DoEvents();
            }
        }

        private delegate void ShowPlayerHandD(Player player);
        public void ShowPlayerHand(Player player)
        {
            if (InvokeRequired)
            {
                Invoke(new ShowPlayerHandD(ShowPlayerHand), new object[] { player});
            }
            else
            {
                try
                {
                    playerRow[player.name].Cells["PlayerHand"].Value = player.cards[0].NiceName() + " " + player.cards[1].NiceName();
                    playerRow[player.name].Cells["PlayerHandType"].Value = player.currentHand;
                    playerRow[player.name].Cells["Odds"].Value = FormatPercent(player.oddsMatrix.Sum(x => x));
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    master.WriteLine("GUI error: " + ex);
                }
            }
        }
        private delegate void ClearPlayerHandD(Player player);
        public void ClearPlayerHand(Player player)
        {
            if (InvokeRequired)
            {
                Invoke(new ClearPlayerHandD(ClearPlayerHand), new object[] { player });
            }
            else
            {
                try
                {
                    playerRow[player.name].Cells["PlayerHand"].Value = "";
                    playerRow[player.name].Cells["PlayerHandType"].Value = "";
                    playerRow[player.name].Cells["Odds"].Value = "";
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    master.WriteLine("GUI exception: " + ex);
                }
            }
        }
        private delegate void SetPlayerSmallBlindD(Player player);
        public void SetPlayerSmallBlind(Player player)
        {
            if (InvokeRequired)
            {
                Invoke(new SetPlayerSmallBlindD(SetPlayerSmallBlind), new object[] { player });
            }
            else
            {
                RefreshPlayer(player);
                if (player.isPlayable)
                {
                    
                }
                else
                {
                    try
                    {
                        playerRow[player.name].Cells["PlayerAction"].Value = "Small Blind";
                    }
                    catch (Exception e)
                    {
                        master.WriteLine("GUI Exception: " + e);
                    }
                }
                
            }
        }
        private delegate void SetPlayerBigBlindD(Player player);
        public void SetPlayerBigBlind(Player player)
        {
            if (InvokeRequired)
            {
                Invoke(new SetPlayerBigBlindD(SetPlayerBigBlind), new object[] { player });
            }
            else
            {
                RefreshPlayer(player);
                if (player.isPlayable)
                {

                }
                else
                {
                    try
                    {
                        playerRow[player.name].Cells["PlayerAction"].Value = "Big Blind";
                    }
                    catch (Exception ex)
                    {
                        master.WriteLine("GUI ex: " + ex);
                    }
                }
                
            }
        }

        private delegate void SetPlayerOddsD(Player player, double[] odds);
        public void SetPlayerOdds(Player player, double[] odds)
        {
            if (InvokeRequired)
            {
                Invoke(new SetPlayerOddsD(SetPlayerOdds), new object[] { player, odds });
            }
            else
            {
                if (player.isPlayable)
                {
                    SetWinningOdds(odds);
                }
                else
                {
                    try
                    {
                        playerRow[player.name].Cells["Odds"].Value = FormatPercent(odds.Sum(x => x));
                    }
                    catch (Exception ex)
                    {
                        master.WriteLine("GUI exception: " + ex);
                    }
                }
            }
        }
        private delegate void SetPlayerHandTypeD(Player player, string hand);
        public void SetPlayerHandType(Player player, string hand)
        {
            if (InvokeRequired)
            {
                Invoke(new SetPlayerHandTypeD(SetPlayerHandType), new object[] { player, hand });
            }
            else
            {
                if (player.isPlayable)
                {
                    SetHandType(hand);
                }
                else
                {
                    try
                    {
                        playerRow[player.name].Cells["PlayerHandType"].Value = hand;
                    }
                    catch (Exception ex)
                    {
                        master.WriteLine("GUI err: " + ex);
                    }
                }
            }
        }
        private delegate void SetWinningPlayersD(List<Player> winners);
        public void SetWinningPlayers(List<Player> winners)
        {
            if (InvokeRequired)
            {
                Invoke(new SetWinningPlayersD(SetWinningPlayers), new object[] { winners });
            }
            else
            {
                foreach (DataGridViewRow r in players.Rows)
                {
                    if (winners.Select(p=>p.name).Contains(r.Cells["PlayerName"].Value.ToString()))
                    {
                        r.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                    else r.DefaultCellStyle.BackColor = Color.OrangeRed;
                }
                if (winners.Where(p=>p.isPlayable).Any())
                {
                    HandType.BackColor = Color.Yellow;
                }
                else HandType.BackColor = Color.Transparent;

            }
        }
        private delegate void ClearWinningPlayerD();
        public void ClearWinningPlayer()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearWinningPlayerD(ClearWinningPlayer), new object[] {  });
            }
            else
            {
                foreach (DataGridViewRow r in players.Rows)
                {
                    r.DefaultCellStyle.BackColor = Color.White;
                }
                HandType.BackColor = Color.Transparent;

            }
        }

        private delegate void ClearPlayerActionsD();
        public void ClearPlayerActions()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearPlayerActionsD(ClearPlayerActions), new object[] { });
            }
            else
            {
                foreach (DataGridViewRow r in players.Rows)
                {
                    r.Cells["PlayerAction"].Value = "";
                    r.Cells["PlayerBet"].Value = "";
                }
                betTotal.Value = 0;
                Application.DoEvents();
                System.Threading.Thread.Sleep(1);
            }
        }

        private delegate void SetWinningOddsD(double[] player);
        public void SetWinningOdds(double[] player)
        {
            if (InvokeRequired)
            {
                Invoke(new SetWinningOddsD(SetWinningOdds), new object[] { player });
            }
            else
            {
                double playerwins = 0.0;

                for (int i = 0; i < 9; i++)
                {
                    switch ((HoldemHand.Hand.HandTypes)i)
                    {
                        case HoldemHand.Hand.HandTypes.HighCard:
                            PlayerHighCard.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.Pair:
                            PlayerPair.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.TwoPair:
                            PlayerTwoPair.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.Trips:
                            Player3ofaKind.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.Straight:
                            PlayerStraight.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.Flush:
                            PlayerFlush.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.FullHouse:
                            PlayerFullhouse.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.FourOfAKind:
                            Player4ofaKind.Text = FormatPercent(player[i]);
                            break;
                        case HoldemHand.Hand.HandTypes.StraightFlush:
                            PlayerStraightFlush.Text = FormatPercent(player[i]);
                            break;
                    }
                    playerwins += player[i];
                }

                winningOdds.Text = FormatPercent(playerwins);
            }
        }

        private string FormatPercent(double d)
        {
            return (100*d).ToString("N2") + "%";
        }

        private delegate void ClearOddsGridD();
        public void ClearOddsGrid()
        {
            if (InvokeRequired)
            {
                Invoke(new ClearOddsGridD(ClearOddsGrid), new object[] { });
            }
            else
            {
                PlayerHighCard.Text = "";
                PlayerPair.Text = "";
                PlayerTwoPair.Text = "";
                Player3ofaKind.Text = "";
                PlayerStraight.Text = "";
                PlayerFlush.Text = "";
                PlayerFullhouse.Text = "";
                Player4ofaKind.Text = "";
                PlayerStraightFlush.Text = "";
                winningOdds.Text = "";
            }
        }
    }
}
