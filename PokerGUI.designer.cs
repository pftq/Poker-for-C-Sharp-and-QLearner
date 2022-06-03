namespace Poker
{
    partial class PokerGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.allinBtn = new System.Windows.Forms.Button();
            this.foldBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chips = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.hand1 = new System.Windows.Forms.Button();
            this.hand2 = new System.Windows.Forms.Button();
            this.card2 = new System.Windows.Forms.Button();
            this.card1 = new System.Windows.Forms.Button();
            this.card4 = new System.Windows.Forms.Button();
            this.card3 = new System.Windows.Forms.Button();
            this.card5 = new System.Windows.Forms.Button();
            this.checkBtn = new System.Windows.Forms.Button();
            this.pot = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.players = new System.Windows.Forms.DataGridView();
            this.PlayerDealer = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PlayerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerChips = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerHand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerHandType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Odds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlayerBet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dealer = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.betTotal = new System.Windows.Forms.NumericUpDown();
            this.betBtn = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.PlayerHighCard = new System.Windows.Forms.Label();
            this.PlayerPair = new System.Windows.Forms.Label();
            this.PlayerTwoPair = new System.Windows.Forms.Label();
            this.Player3ofaKind = new System.Windows.Forms.Label();
            this.PlayerStraight = new System.Windows.Forms.Label();
            this.PlayerFlush = new System.Windows.Forms.Label();
            this.PlayerFullhouse = new System.Windows.Forms.Label();
            this.Player4ofaKind = new System.Windows.Forms.Label();
            this.PlayerStraightFlush = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.winningOdds = new System.Windows.Forms.Label();
            this.HandType = new System.Windows.Forms.Label();
            this.redeal = new System.Windows.Forms.Button();
            this.callBtn = new System.Windows.Forms.Button();
            this.betAmt = new System.Windows.Forms.NumericUpDown();
            this.settings = new System.Windows.Forms.Button();
            this.blindCards = new System.Windows.Forms.Button();
            this.username = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chips)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.players)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.betTotal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.betAmt)).BeginInit();
            this.SuspendLayout();
            // 
            // allinBtn
            // 
            this.allinBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.allinBtn.Location = new System.Drawing.Point(690, 513);
            this.allinBtn.Margin = new System.Windows.Forms.Padding(4);
            this.allinBtn.Name = "allinBtn";
            this.allinBtn.Size = new System.Drawing.Size(79, 27);
            this.allinBtn.TabIndex = 3;
            this.allinBtn.Text = "All-In";
            this.allinBtn.UseVisualStyleBackColor = true;
            // 
            // foldBtn
            // 
            this.foldBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.foldBtn.Location = new System.Drawing.Point(605, 513);
            this.foldBtn.Margin = new System.Windows.Forms.Padding(4);
            this.foldBtn.Name = "foldBtn";
            this.foldBtn.Size = new System.Drawing.Size(77, 27);
            this.foldBtn.TabIndex = 4;
            this.foldBtn.Text = "Fold";
            this.foldBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(273, 493);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Chips: ";
            // 
            // chips
            // 
            this.chips.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.chips.Enabled = false;
            this.chips.Location = new System.Drawing.Point(331, 493);
            this.chips.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.chips.Name = "chips";
            this.chips.ReadOnly = true;
            this.chips.Size = new System.Drawing.Size(120, 22);
            this.chips.TabIndex = 6;
            this.chips.ThousandsSeparator = true;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(281, 454);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Hand: ";
            // 
            // hand1
            // 
            this.hand1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.hand1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.hand1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hand1.Location = new System.Drawing.Point(337, 449);
            this.hand1.Name = "hand1";
            this.hand1.Size = new System.Drawing.Size(50, 38);
            this.hand1.TabIndex = 8;
            this.hand1.UseVisualStyleBackColor = false;
            // 
            // hand2
            // 
            this.hand2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.hand2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.hand2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hand2.Location = new System.Drawing.Point(393, 449);
            this.hand2.Name = "hand2";
            this.hand2.Size = new System.Drawing.Size(50, 38);
            this.hand2.TabIndex = 9;
            this.hand2.UseVisualStyleBackColor = false;
            // 
            // card2
            // 
            this.card2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.card2.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.card2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.card2.Location = new System.Drawing.Point(311, 240);
            this.card2.Name = "card2";
            this.card2.Size = new System.Drawing.Size(50, 54);
            this.card2.TabIndex = 11;
            this.card2.UseVisualStyleBackColor = false;
            // 
            // card1
            // 
            this.card1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.card1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.card1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.card1.Location = new System.Drawing.Point(255, 240);
            this.card1.Name = "card1";
            this.card1.Size = new System.Drawing.Size(50, 54);
            this.card1.TabIndex = 10;
            this.card1.UseVisualStyleBackColor = false;
            // 
            // card4
            // 
            this.card4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.card4.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.card4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.card4.Location = new System.Drawing.Point(423, 240);
            this.card4.Name = "card4";
            this.card4.Size = new System.Drawing.Size(50, 54);
            this.card4.TabIndex = 13;
            this.card4.UseVisualStyleBackColor = false;
            // 
            // card3
            // 
            this.card3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.card3.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.card3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.card3.Location = new System.Drawing.Point(367, 240);
            this.card3.Name = "card3";
            this.card3.Size = new System.Drawing.Size(50, 54);
            this.card3.TabIndex = 12;
            this.card3.UseVisualStyleBackColor = false;
            // 
            // card5
            // 
            this.card5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.card5.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.card5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.card5.Location = new System.Drawing.Point(479, 240);
            this.card5.Name = "card5";
            this.card5.Size = new System.Drawing.Size(50, 54);
            this.card5.TabIndex = 14;
            this.card5.UseVisualStyleBackColor = false;
            // 
            // checkBtn
            // 
            this.checkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBtn.Location = new System.Drawing.Point(520, 513);
            this.checkBtn.Margin = new System.Windows.Forms.Padding(4);
            this.checkBtn.Name = "checkBtn";
            this.checkBtn.Size = new System.Drawing.Size(77, 27);
            this.checkBtn.TabIndex = 15;
            this.checkBtn.Text = "Check";
            this.checkBtn.UseVisualStyleBackColor = true;
            // 
            // pot
            // 
            this.pot.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pot.Enabled = false;
            this.pot.Location = new System.Drawing.Point(331, 299);
            this.pot.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.pot.Name = "pot";
            this.pot.ReadOnly = true;
            this.pot.Size = new System.Drawing.Size(138, 22);
            this.pot.TabIndex = 17;
            this.pot.ThousandsSeparator = true;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(292, 301);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 17);
            this.label3.TabIndex = 16;
            this.label3.Text = "Pot:";
            // 
            // players
            // 
            this.players.AllowUserToAddRows = false;
            this.players.AllowUserToDeleteRows = false;
            this.players.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.players.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.players.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PlayerDealer,
            this.PlayerName,
            this.PlayerChips,
            this.PlayerHand,
            this.PlayerHandType,
            this.Odds,
            this.PlayerAction,
            this.PlayerBet});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.players.DefaultCellStyle = dataGridViewCellStyle1;
            this.players.Location = new System.Drawing.Point(13, 13);
            this.players.MultiSelect = false;
            this.players.Name = "players";
            this.players.ReadOnly = true;
            this.players.RowHeadersVisible = false;
            this.players.RowTemplate.Height = 24;
            this.players.Size = new System.Drawing.Size(756, 221);
            this.players.TabIndex = 18;
            // 
            // PlayerDealer
            // 
            this.PlayerDealer.HeaderText = "Dealer?";
            this.PlayerDealer.MinimumWidth = 3;
            this.PlayerDealer.Name = "PlayerDealer";
            this.PlayerDealer.ReadOnly = true;
            this.PlayerDealer.Width = 50;
            // 
            // PlayerName
            // 
            this.PlayerName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PlayerName.HeaderText = "Player";
            this.PlayerName.Name = "PlayerName";
            this.PlayerName.ReadOnly = true;
            // 
            // PlayerChips
            // 
            this.PlayerChips.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PlayerChips.HeaderText = "Chips";
            this.PlayerChips.Name = "PlayerChips";
            this.PlayerChips.ReadOnly = true;
            // 
            // PlayerHand
            // 
            this.PlayerHand.HeaderText = "Hand";
            this.PlayerHand.Name = "PlayerHand";
            this.PlayerHand.ReadOnly = true;
            this.PlayerHand.Width = 70;
            // 
            // PlayerHandType
            // 
            this.PlayerHandType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PlayerHandType.HeaderText = "HandType";
            this.PlayerHandType.Name = "PlayerHandType";
            this.PlayerHandType.ReadOnly = true;
            // 
            // Odds
            // 
            this.Odds.HeaderText = "Odds";
            this.Odds.Name = "Odds";
            this.Odds.ReadOnly = true;
            this.Odds.Width = 70;
            // 
            // PlayerAction
            // 
            this.PlayerAction.HeaderText = "Action";
            this.PlayerAction.Name = "PlayerAction";
            this.PlayerAction.ReadOnly = true;
            this.PlayerAction.Width = 75;
            // 
            // PlayerBet
            // 
            this.PlayerBet.HeaderText = "Bet";
            this.PlayerBet.Name = "PlayerBet";
            this.PlayerBet.ReadOnly = true;
            this.PlayerBet.Width = 70;
            // 
            // dealer
            // 
            this.dealer.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.dealer.AutoSize = true;
            this.dealer.Enabled = false;
            this.dealer.Location = new System.Drawing.Point(350, 354);
            this.dealer.Name = "dealer";
            this.dealer.Size = new System.Drawing.Size(80, 21);
            this.dealer.TabIndex = 19;
            this.dealer.Text = "Dealer?";
            this.dealer.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(292, 383);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 17);
            this.label4.TabIndex = 20;
            this.label4.Text = "Bet:";
            // 
            // betTotal
            // 
            this.betTotal.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.betTotal.Enabled = false;
            this.betTotal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.betTotal.Location = new System.Drawing.Point(331, 381);
            this.betTotal.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.betTotal.Name = "betTotal";
            this.betTotal.ReadOnly = true;
            this.betTotal.Size = new System.Drawing.Size(138, 22);
            this.betTotal.TabIndex = 21;
            this.betTotal.ThousandsSeparator = true;
            // 
            // betBtn
            // 
            this.betBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.betBtn.Location = new System.Drawing.Point(520, 474);
            this.betBtn.Margin = new System.Windows.Forms.Padding(4);
            this.betBtn.Name = "betBtn";
            this.betBtn.Size = new System.Drawing.Size(77, 27);
            this.betBtn.TabIndex = 22;
            this.betBtn.Text = "Bet/Raise";
            this.betBtn.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoEllipsis = true;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 342);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 19);
            this.label7.TabIndex = 27;
            this.label7.Text = "High Card";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoEllipsis = true;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 361);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 19);
            this.label8.TabIndex = 28;
            this.label8.Text = "Pair";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label9.AutoEllipsis = true;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(12, 382);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 19);
            this.label9.TabIndex = 29;
            this.label9.Text = "Two Pair";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoEllipsis = true;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(12, 401);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 19);
            this.label10.TabIndex = 30;
            this.label10.Text = "3 of a Kind";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoEllipsis = true;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(12, 420);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(72, 19);
            this.label11.TabIndex = 31;
            this.label11.Text = "Straight";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label12.AutoEllipsis = true;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(12, 439);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(52, 19);
            this.label12.TabIndex = 32;
            this.label12.Text = "Flush";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoEllipsis = true;
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(13, 458);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(92, 19);
            this.label13.TabIndex = 33;
            this.label13.Text = "Full House";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoEllipsis = true;
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(13, 477);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 19);
            this.label14.TabIndex = 34;
            this.label14.Text = "4 of a Kind";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label15.AutoEllipsis = true;
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(13, 496);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(120, 19);
            this.label15.TabIndex = 35;
            this.label15.Text = "Straight Flush";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerHighCard
            // 
            this.PlayerHighCard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerHighCard.AutoSize = true;
            this.PlayerHighCard.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerHighCard.Location = new System.Drawing.Point(140, 342);
            this.PlayerHighCard.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerHighCard.Name = "PlayerHighCard";
            this.PlayerHighCard.Size = new System.Drawing.Size(32, 19);
            this.PlayerHighCard.TabIndex = 37;
            this.PlayerHighCard.Text = "0.0";
            this.PlayerHighCard.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerPair
            // 
            this.PlayerPair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerPair.AutoSize = true;
            this.PlayerPair.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerPair.Location = new System.Drawing.Point(140, 361);
            this.PlayerPair.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerPair.Name = "PlayerPair";
            this.PlayerPair.Size = new System.Drawing.Size(32, 19);
            this.PlayerPair.TabIndex = 38;
            this.PlayerPair.Text = "0.0";
            this.PlayerPair.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerTwoPair
            // 
            this.PlayerTwoPair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerTwoPair.AutoSize = true;
            this.PlayerTwoPair.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerTwoPair.Location = new System.Drawing.Point(140, 382);
            this.PlayerTwoPair.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerTwoPair.Name = "PlayerTwoPair";
            this.PlayerTwoPair.Size = new System.Drawing.Size(32, 19);
            this.PlayerTwoPair.TabIndex = 39;
            this.PlayerTwoPair.Text = "0.0";
            this.PlayerTwoPair.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Player3ofaKind
            // 
            this.Player3ofaKind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Player3ofaKind.AutoSize = true;
            this.Player3ofaKind.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Player3ofaKind.Location = new System.Drawing.Point(140, 401);
            this.Player3ofaKind.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Player3ofaKind.Name = "Player3ofaKind";
            this.Player3ofaKind.Size = new System.Drawing.Size(32, 19);
            this.Player3ofaKind.TabIndex = 40;
            this.Player3ofaKind.Text = "0.0";
            this.Player3ofaKind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerStraight
            // 
            this.PlayerStraight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerStraight.AutoSize = true;
            this.PlayerStraight.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerStraight.Location = new System.Drawing.Point(140, 420);
            this.PlayerStraight.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerStraight.Name = "PlayerStraight";
            this.PlayerStraight.Size = new System.Drawing.Size(32, 19);
            this.PlayerStraight.TabIndex = 41;
            this.PlayerStraight.Text = "0.0";
            this.PlayerStraight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerFlush
            // 
            this.PlayerFlush.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerFlush.AutoSize = true;
            this.PlayerFlush.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerFlush.Location = new System.Drawing.Point(140, 439);
            this.PlayerFlush.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerFlush.Name = "PlayerFlush";
            this.PlayerFlush.Size = new System.Drawing.Size(32, 19);
            this.PlayerFlush.TabIndex = 42;
            this.PlayerFlush.Text = "0.0";
            this.PlayerFlush.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerFullhouse
            // 
            this.PlayerFullhouse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerFullhouse.AutoSize = true;
            this.PlayerFullhouse.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerFullhouse.Location = new System.Drawing.Point(141, 458);
            this.PlayerFullhouse.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerFullhouse.Name = "PlayerFullhouse";
            this.PlayerFullhouse.Size = new System.Drawing.Size(32, 19);
            this.PlayerFullhouse.TabIndex = 43;
            this.PlayerFullhouse.Text = "0.0";
            this.PlayerFullhouse.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Player4ofaKind
            // 
            this.Player4ofaKind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Player4ofaKind.AutoSize = true;
            this.Player4ofaKind.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Player4ofaKind.Location = new System.Drawing.Point(141, 477);
            this.Player4ofaKind.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Player4ofaKind.Name = "Player4ofaKind";
            this.Player4ofaKind.Size = new System.Drawing.Size(32, 19);
            this.Player4ofaKind.TabIndex = 44;
            this.Player4ofaKind.Text = "0.0";
            this.Player4ofaKind.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PlayerStraightFlush
            // 
            this.PlayerStraightFlush.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PlayerStraightFlush.AutoSize = true;
            this.PlayerStraightFlush.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayerStraightFlush.Location = new System.Drawing.Point(141, 496);
            this.PlayerStraightFlush.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PlayerStraightFlush.Name = "PlayerStraightFlush";
            this.PlayerStraightFlush.Size = new System.Drawing.Size(32, 19);
            this.PlayerStraightFlush.TabIndex = 45;
            this.PlayerStraightFlush.Text = "0.0";
            this.PlayerStraightFlush.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 314);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 17);
            this.label6.TabIndex = 55;
            this.label6.Text = "Odds";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoEllipsis = true;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 518);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 19);
            this.label5.TabIndex = 56;
            this.label5.Text = "Winning Odds";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // winningOdds
            // 
            this.winningOdds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.winningOdds.AutoSize = true;
            this.winningOdds.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.winningOdds.Location = new System.Drawing.Point(141, 518);
            this.winningOdds.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.winningOdds.Name = "winningOdds";
            this.winningOdds.Size = new System.Drawing.Size(32, 19);
            this.winningOdds.TabIndex = 57;
            this.winningOdds.Text = "0.0";
            this.winningOdds.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // HandType
            // 
            this.HandType.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.HandType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HandType.Location = new System.Drawing.Point(255, 419);
            this.HandType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HandType.Name = "HandType";
            this.HandType.Size = new System.Drawing.Size(274, 19);
            this.HandType.TabIndex = 58;
            this.HandType.Text = "Hand Description";
            this.HandType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // redeal
            // 
            this.redeal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.redeal.Location = new System.Drawing.Point(680, 240);
            this.redeal.Margin = new System.Windows.Forms.Padding(4);
            this.redeal.Name = "redeal";
            this.redeal.Size = new System.Drawing.Size(89, 27);
            this.redeal.TabIndex = 59;
            this.redeal.Text = "Redeal";
            this.redeal.UseVisualStyleBackColor = true;
            // 
            // callBtn
            // 
            this.callBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.callBtn.Location = new System.Drawing.Point(520, 439);
            this.callBtn.Margin = new System.Windows.Forms.Padding(4);
            this.callBtn.Name = "callBtn";
            this.callBtn.Size = new System.Drawing.Size(77, 27);
            this.callBtn.TabIndex = 60;
            this.callBtn.Text = "Call";
            this.callBtn.UseVisualStyleBackColor = true;
            // 
            // betAmt
            // 
            this.betAmt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.betAmt.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.betAmt.Location = new System.Drawing.Point(606, 477);
            this.betAmt.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.betAmt.Name = "betAmt";
            this.betAmt.Size = new System.Drawing.Size(164, 22);
            this.betAmt.TabIndex = 61;
            this.betAmt.ThousandsSeparator = true;
            // 
            // settings
            // 
            this.settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.settings.Location = new System.Drawing.Point(13, 241);
            this.settings.Margin = new System.Windows.Forms.Padding(4);
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(92, 27);
            this.settings.TabIndex = 62;
            this.settings.Text = "Settings";
            this.settings.UseVisualStyleBackColor = true;
            // 
            // blindCards
            // 
            this.blindCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.blindCards.Location = new System.Drawing.Point(680, 275);
            this.blindCards.Margin = new System.Windows.Forms.Padding(4);
            this.blindCards.Name = "blindCards";
            this.blindCards.Size = new System.Drawing.Size(90, 27);
            this.blindCards.TabIndex = 63;
            this.blindCards.Text = "Play Blind";
            this.blindCards.UseVisualStyleBackColor = true;
            // 
            // username
            // 
            this.username.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.username.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.username.Location = new System.Drawing.Point(295, 521);
            this.username.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(189, 19);
            this.username.TabIndex = 64;
            this.username.Text = "Player Name";
            this.username.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PokerGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.username);
            this.Controls.Add(this.blindCards);
            this.Controls.Add(this.settings);
            this.Controls.Add(this.betAmt);
            this.Controls.Add(this.callBtn);
            this.Controls.Add(this.redeal);
            this.Controls.Add(this.HandType);
            this.Controls.Add(this.winningOdds);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.PlayerHighCard);
            this.Controls.Add(this.PlayerPair);
            this.Controls.Add(this.PlayerTwoPair);
            this.Controls.Add(this.Player3ofaKind);
            this.Controls.Add(this.PlayerStraight);
            this.Controls.Add(this.PlayerFlush);
            this.Controls.Add(this.PlayerFullhouse);
            this.Controls.Add(this.Player4ofaKind);
            this.Controls.Add(this.PlayerStraightFlush);
            this.Controls.Add(this.betBtn);
            this.Controls.Add(this.betTotal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dealer);
            this.Controls.Add(this.players);
            this.Controls.Add(this.pot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBtn);
            this.Controls.Add(this.card5);
            this.Controls.Add(this.card4);
            this.Controls.Add(this.card3);
            this.Controls.Add(this.card2);
            this.Controls.Add(this.card1);
            this.Controls.Add(this.hand2);
            this.Controls.Add(this.hand1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chips);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.foldBtn);
            this.Controls.Add(this.allinBtn);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PokerGUI";
            this.Text = "Poker for C# and QLearner";
            ((System.ComponentModel.ISupportInitialize)(this.chips)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.players)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.betTotal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.betAmt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button allinBtn;
        private System.Windows.Forms.Button foldBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown chips;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button hand1;
        private System.Windows.Forms.Button hand2;
        private System.Windows.Forms.Button card2;
        private System.Windows.Forms.Button card1;
        private System.Windows.Forms.Button card4;
        private System.Windows.Forms.Button card3;
        private System.Windows.Forms.Button card5;
        private System.Windows.Forms.Button checkBtn;
        private System.Windows.Forms.NumericUpDown pot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView players;
        private System.Windows.Forms.CheckBox dealer;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown betTotal;
        private System.Windows.Forms.Button betBtn;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label PlayerHighCard;
        private System.Windows.Forms.Label PlayerPair;
        private System.Windows.Forms.Label PlayerTwoPair;
        private System.Windows.Forms.Label Player3ofaKind;
        private System.Windows.Forms.Label PlayerStraight;
        private System.Windows.Forms.Label PlayerFlush;
        private System.Windows.Forms.Label PlayerFullhouse;
        private System.Windows.Forms.Label Player4ofaKind;
        private System.Windows.Forms.Label PlayerStraightFlush;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label winningOdds;
        private System.Windows.Forms.Label HandType;
        private System.Windows.Forms.Button redeal;
        private System.Windows.Forms.DataGridViewCheckBoxColumn PlayerDealer;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerChips;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerHand;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerHandType;
        private System.Windows.Forms.DataGridViewTextBoxColumn Odds;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlayerBet;
        private System.Windows.Forms.Button callBtn;
        private System.Windows.Forms.NumericUpDown betAmt;
        private System.Windows.Forms.Button settings;
        private System.Windows.Forms.Button blindCards;
        private System.Windows.Forms.Label username;
    }
}