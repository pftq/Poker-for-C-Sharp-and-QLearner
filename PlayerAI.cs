using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QLearner.Resources;
using System.IO;

namespace Poker
{
    public class PlayerAI:Player
    {

        public PlayerAI(int table_pos, string username, int chips):base(table_pos, username, chips)
        {

            
        }


        public override PokerAction BestAction(int currentCall, int minBet, int maxEnemyChips, int minEnemyChips, int averageEnemyChips, int numEnemies)
        {
            return base.BestAction(currentCall, minBet, maxEnemyChips, minEnemyChips, averageEnemyChips, numEnemies);
        }

        public void PlayHand()
        {
        }

        public override void UpdateChips(int newChips)
        {

        }

        public override void WonChips(int won)
        {
            
        }


    }

}
