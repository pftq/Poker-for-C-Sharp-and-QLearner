using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QLearner.Resources;
using System.Runtime.Serialization;
using ProtoBuf;
using System.IO;

namespace Poker
{
    [Serializable]
    [ProtoContract]
    public class Stats : ISerializable
    {
        [ProtoMember(1)]
        public Dictionary<string, int> handStatsWon = new Dictionary<string, int>();
        [ProtoMember(2)]
        public Dictionary<string, int> handStatsTotal = new Dictionary<string, int>();
        [ProtoMember(3)]
        public Dictionary<string, int> handStatsChips = new Dictionary<string, int>();
        [ProtoMember(4)]
        public Dictionary<string, int> actionStatsWon = new Dictionary<string, int>();
        [ProtoMember(5)]
        public Dictionary<string, int> actionStatsTotal = new Dictionary<string, int>();
        [ProtoMember(6)]
        public Dictionary<string, int> actionStatsChips = new Dictionary<string, int>();

        [ProtoMember(7)]
        public int handsPlayed = 0;
        [ProtoMember(8)]
        public int handsSkipped = 0;
        [ProtoMember(9)]
        public int handsFolded = 0;
        [ProtoMember(10)]
        public int chipsWon = 0;
        [ProtoMember(11)]
        public int chipsLost = 0;

        [ProtoMember(12)]
        public Dictionary<string, int> positionStatsWon = new Dictionary<string, int>();
        [ProtoMember(13)]
        public Dictionary<string, int> positionStatsTotal = new Dictionary<string, int>();
        [ProtoMember(14)]
        public Dictionary<string, int> positionStatsChips = new Dictionary<string, int>();

        [ProtoMember(15)]
        public Dictionary<string, int> enemyStatsWon = new Dictionary<string, int>();
        [ProtoMember(16)]
        public Dictionary<string, int> enemyStatsTotal = new Dictionary<string, int>();
        [ProtoMember(17)]
        public Dictionary<string, int> enemyStatsChips = new Dictionary<string, int>();

        public Stats()
        {

        }

        public Stats(SerializationInfo info, StreamingContext context)
        {
            handStatsWon = (Dictionary<string, int>)info.GetValue("handStatsWon", typeof(Dictionary<string, int>));
            handStatsTotal = (Dictionary<string, int>)info.GetValue("handStatsTotal", typeof(Dictionary<string, int>));
            handStatsChips = (Dictionary<string, int>)info.GetValue("handStatsChips", typeof(Dictionary<string, int>));
            actionStatsWon = (Dictionary<string, int>)info.GetValue("actionStatsWon", typeof(Dictionary<string, int>));
            actionStatsTotal = (Dictionary<string, int>)info.GetValue("actionStatsTotal", typeof(Dictionary<string, int>));
            actionStatsChips = (Dictionary<string, int>)info.GetValue("actionStatsChips", typeof(Dictionary<string, int>));
            chipsWon = (int)info.GetValue("chipsWon", typeof(int));
            chipsLost = (int)info.GetValue("chipsLost", typeof(int));
            handsPlayed = (int)info.GetValue("handsPlayed", typeof(int));
            handsSkipped = (int)info.GetValue("handsSkipped", typeof(int));
            handsFolded = (int)info.GetValue("handsFolded", typeof(int));
            positionStatsWon = (Dictionary<string, int>)info.GetValue("positionStatsWon", typeof(Dictionary<string, int>));
            positionStatsTotal = (Dictionary<string, int>)info.GetValue("positionStatsTotal", typeof(Dictionary<string, int>));
            positionStatsChips = (Dictionary<string, int>)info.GetValue("positionStatsChips", typeof(Dictionary<string, int>));
            enemyStatsWon = (Dictionary<string, int>)info.GetValue("enemyStatsWon", typeof(Dictionary<string, int>));
            enemyStatsTotal = (Dictionary<string, int>)info.GetValue("enemyStatsTotal", typeof(Dictionary<string, int>));
            enemyStatsChips = (Dictionary<string, int>)info.GetValue("enemyStatsChips", typeof(Dictionary<string, int>));
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("handStatsWon", handStatsWon);
            info.AddValue("handStatsTotal", handStatsTotal);
            info.AddValue("handStatsChips", handStatsChips);
            info.AddValue("actionStatsWon", actionStatsWon);
            info.AddValue("actionStatsTotal", actionStatsTotal);
            info.AddValue("actionStatsChips", actionStatsChips);
            info.AddValue("chipsWon", chipsWon);
            info.AddValue("chipsLost", chipsLost);
            info.AddValue("handsPlayed", handsPlayed);
            info.AddValue("handsSkipped", handsSkipped);
            info.AddValue("handsFolded", handsFolded);
            info.AddValue("positionStatsWon", positionStatsWon);
            info.AddValue("positionStatsTotal", positionStatsTotal);
            info.AddValue("positionStatsChips", positionStatsChips);
            info.AddValue("enemyStatsWon", enemyStatsWon);
            info.AddValue("enemyStatsTotal", enemyStatsTotal);
            info.AddValue("enemyStatsChips", enemyStatsChips);
        }

        public static Stats OpenStatsFromFile(string filename)
        {
            Stats objectToSerialize;
            using (Stream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {

                    objectToSerialize = ProtoBuf.Serializer.Deserialize<Stats>(stream);
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

        public static void SaveStatsToFile(string filename, Stats objectToSerialize)
        {
            using (Stream stream = File.Open(filename + "_new", FileMode.Create))
            {

                ProtoBuf.Serializer.Serialize<Stats>(stream, objectToSerialize);
                stream.Close();

            }
            if (System.IO.File.Exists(filename + "_backup")) System.IO.File.Delete(filename + "_backup");
            if (System.IO.File.Exists(filename)) System.IO.File.Move(filename, filename + "_backup");
            if (System.IO.File.Exists(filename + "_new")) System.IO.File.Move(filename + "_new", filename);
        }
    }

}
