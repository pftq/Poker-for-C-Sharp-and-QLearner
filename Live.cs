using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using QLearner.Resources;
using System.Drawing.Imaging;
using AForge.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using tessnet2;
using System.Text.RegularExpressions;

namespace Poker
{
    public class Live 
    {
        // configure
        public static string folderPrefix = "images/";
        private static bool saveScreenshots = true;

        // do not touch
        private static int screenWidth = 1920;
        private static int screenHeight = 1200;
        public static Rectangle? tableArea, smallerTableArea, playerArea, callArea, foldArea, foldPreArea, checkPreArea, raiseArea, minBetArea, quarterpotArea, halfpotArea, potArea, allinArea, allinCallArea, iAmBackArea, noticeArea, checkArea, betArea, flopArea, chipsArea;
        public static Bitmap screen, table, player, call, fold, raise, minBet, quarterpot, halfpot, pot, allin, allinCall, iAmBack, check, bet, flop, chips;
        public static bool gameFound = false, playerFound = false, cardsFound=false, myTurn=false, flopFound=false, enableAlerts = true;
        public static double scale = 1.0;
        public static Card card1, card2, lastCard1, lastCard2, flopCard1, flopCard2, flopCard3, flopCard4, flopCard5, lastFlopCard1, lastFlopCard2, lastFlopCard3, lastFlopCard4, lastFlopCard5;
        public static float matchScore=0;
        public static int prevFlopCt = 0;

        public static Dictionary<string, int> enemyChips = new Dictionary<string, int>();
        public static Dictionary<string, Rectangle> enemyAreas = new Dictionary<string, Rectangle>();
        public static Dictionary<string, int> enemyEncounters = new Dictionary<string, int>();
        public static int maxEnemyChips = 0, minEnemyChips=0, avgEnemyChips=0, enemyCount=0;
        public static DateTime lastEnemyCheck = DateTime.MinValue;
        public static bool MyTurnIsReady { get {return myTurn&&cardsFound;} }

        private static bool expectedChipsDecrease = false; // flag unusual likely misreading of chips

        public static List<Card> flopCards
        {
            get
            {
                List<Card> t = new List<Card>();
                if (flopCard1 != null) t.Add(flopCard1);
                if (flopCard2 != null) t.Add(flopCard2);
                if (flopCard3 != null) t.Add(flopCard3);
                if (flopCard4 != null) t.Add(flopCard4);
                if (flopCard5 != null) t.Add(flopCard5);
                return t;
            }
        }

        public static int currentChips=-1, currentCall=-1, currentRaise = -1, currentBet=-1, currentPot=-1, prevChips = -1, lastMinBet=-1, lastActualMinBet=-1;
        
        public static bool hasRaiseButton = false, hasCallAllInButton = false, hasCallButton=false, hasCheckButton=false, hasBetButton=false;

        public static Tesseract tessnet = new Tesseract();
        public static bool hasDecimals = false;

        private static List<string> imageFiles = new List<string>() // filenames of pngs without the extension. cards automatically added, no need to list here
        {
            "call",
            "fold",
            "fold_pre",
            "raise",
            "halfpot",
            "quarterpot",
            "minbet",
            "pot",
            "allin",
            "allinraise",
            "username",
            "table",
            "iamback",
            "check",
            "check_pre",
            "bet",
            "preflop",
            "enemy",
            "notice",
            "chips_comma",
            "button_comma"
        };

        private static Dictionary<string, Bitmap> images = new Dictionary<string, Bitmap>();

        public static void Reset()
        {
            tableArea = smallerTableArea = playerArea  = foldArea= null;
            gameFound = playerFound = cardsFound = false;
            enableAlerts = true;
        }

        public static void LoadImages() {
            if (images.Any()) return;

            if (!Directory.Exists(folderPrefix + "live/")) Directory.CreateDirectory(folderPrefix + "live/");

            Deck d = new Deck();
            for (int i = 0; i < 52; i++) imageFiles.Add(d.DealCard().ToString());

            WriteLine("Loading image files...");
            List<string> missing = new List<string>();
            foreach (string s in imageFiles)
            {
                string filepath = folderPrefix+s + ".png";
                if (File.Exists(filepath))
                {
                    images[s] = (Bitmap)System.Drawing.Image.FromFile(filepath);
                    
                    //WriteLine("File loaded: " + s);
                }
                else
                {
                    WriteLine("No file found for: " + filepath);
                    missing.Add(s);
                }
            }
            if (missing.Any()) WriteLine("Total missing (" + missing.Count + "): " + string.Join(", ", missing));

            // Tessnet sucks at reading cards, only use for reading chips
            tessnet.Init(folderPrefix+"tessdata/", "eng", true);
            tessnet.SetVariable("tessedit_char_whitelist", "0123456789");
        }

        public static Bitmap GetScreen()
        {
            //return (Bitmap)System.Drawing.Image.FromFile(folderPrefix + "live/live.png"); // test
            
            Bitmap image = new Bitmap(screenWidth, screenHeight, images["table"].PixelFormat);
            var gfx = Graphics.FromImage(image);
            gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, new Size(screenWidth, screenHeight), CopyPixelOperation.SourceCopy);
            return image;
        }

        public static void RefreshScreen()
        {
            screen = GetScreen();
            table = CropImage(screen, tableArea.Value);
        }

        public static void LoadScreen()
        {
            if (!gameFound)
            {
                Thread.Sleep(3000);
                WriteLine("Screencapturing...");
            }
            Thread.Sleep(1000);
            
            card1 = card2 = null;
            flopFound = cardsFound = myTurn = false;
            flopCard1 = flopCard2 = flopCard3 = flopCard4 = flopCard5 = null;
            currentChips = currentCall = currentRaise = currentBet = currentPot = - 1;

            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;
            if(!gameFound) WriteLine("Screen resolution: " + screenWidth + "x" + screenHeight+" (see README if wrong)");

            screen = GetScreen();
            SaveScreenshot(screen, "live");

            
            
            ParseScreen();
        }

        public static void SaveScreenshot(Bitmap img, string filename)
        {
            try { if(saveScreenshots) img.Save(folderPrefix + "live/" + filename + ".png", ImageFormat.Png); }
            catch { }
        }

        public static void ParseScreen()
        {
            if (!gameFound)
            {
                WriteLine("Parsing screen capture...");
                gameFound = HasTable();
            }
            else
            {
                WriteLine("Reusing last table location...", enableAlerts);
                table = CropImage(screen, tableArea.Value);
                SaveScreenshot(table, "live_table");
            }
            if (gameFound)
            {
                

                if (!playerFound) playerFound = HasUser();
                else
                {
                    WriteLine("Reusing last player location...", enableAlerts);
                    player = CropImage(table, playerArea.Value);
                    SaveScreenshot(player, "live_player");
                }

                if (playerFound)
                {
                    CheckChips();
                    myTurn = HasTurnButtons();
                    if (myTurn)
                    {
                        CheckCards();
                        WriteLine("Live cards: " + card1 + " " + card2, enableAlerts);
                        CheckFlop();
                        if(!flopFound) CheckEnemies(true);
                        else CheckEnemies(true);
                        enableAlerts = false;
                    }
                    else
                    {
                        WriteLine("No turn buttons found... ", enableAlerts);
                        if (foldPreArea.HasValue || checkPreArea.HasValue)
                        {
                            //WriteLine("Previewing cards...");
                            CheckCards();
                            if (!flopFound)
                            {
                                CheckFlop();
                                if(flopFound) myTurn = RescanButtons();
                                CheckEnemies(true);
                                if (myTurn) enableAlerts = false; 
                            }
                            //WriteLine("Previewed cards: " + card1 + " " + card2);
                        }
                        /*else
                        {
                            CheckFlop(false);
                            if (flopFound)
                            {
                                CheckEnemies(true);
                            }
                        }*/
                    }
                }
                else
                {
                    WriteLine("No user found.", enableAlerts);
                    SaveScreenshot(CropImage(table, playerSearchArea), "live_player");
                }
            }
            else WriteLine("No live game found.");
        }

        public static void CheckEnemies(bool fullscan = false)
        {
            
            if ((fullscan || !enemyAreas.Any()))
            {
                enemyAreas.Clear();
                WriteLine("Scanning for enemies...", enableAlerts);
                Bitmap haystack = ResizeImage(table, 0.5);
                Bitmap needle = ResizeImage(images["enemy"], 0.5);
                for (int i = 1; i <= 5; i++)
                {
                    ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(.9f);

                    Rectangle searchArea = new Rectangle(0, 0, table.Width, playerArea.Value.Y);

                    if (i == 1) searchArea = new Rectangle(0, playerArea.Value.Y + playerArea.Value.Height - playerArea.Value.Height * 3 / 2, playerArea.Value.X, playerArea.Value.Height * 3 / 2);
                    else if (i == 2) searchArea = new Rectangle(playerArea.Value.X + playerArea.Value.Width, playerArea.Value.Y + playerArea.Value.Height - playerArea.Value.Height * 3 / 2, table.Width - playerArea.Value.X - playerArea.Value.Width, playerArea.Value.Height * 3 / 2);
                    else if (i == 3) searchArea = new Rectangle(0, (table.Height - playerArea.Value.Height * 17/8)/2,  playerArea.Value.Width*3/2, playerArea.Value.Height * 7/4);
                    else if (i == 4) searchArea = new Rectangle(table.Width - playerArea.Value.Width * 3 / 2, (table.Height - playerArea.Value.Height * 17 / 8) / 2, playerArea.Value.Width * 3 / 2, playerArea.Value.Height * 7/4);
                    else if (i == 5) searchArea = new Rectangle(0, 0, table.Width, playerArea.Value.Height * 2);

                    SaveScreenshot(CropImage(table, searchArea), "enemySearch" + i);

                    searchArea = new Rectangle((int)(1.0 * searchArea.Location.X / table.Width * haystack.Width), (int)(1.0 * searchArea.Location.Y / table.Height * haystack.Height), (int)(1.0 * searchArea.Width / table.Width * haystack.Width), (int)(1.0 * searchArea.Height / table.Height * haystack.Height));

                    TemplateMatch[] matchings = tm.ProcessImage(haystack, needle, searchArea);


                    BitmapData data = haystack.LockBits(
                         new Rectangle(0, 0, haystack.Width, haystack.Height),
                         ImageLockMode.ReadWrite, haystack.PixelFormat);


                    int y = 0;
                    List<Rectangle> tempMatches = new List<Rectangle>();
                    foreach (TemplateMatch m in matchings.OrderByDescending(m => m.Similarity))
                    {
                        Rectangle smallerMatch = m.Rectangle;
                        Rectangle rec = new Rectangle((int)(1.0 * smallerMatch.Location.X / haystack.Width * table.Width), (int)(1.0 * smallerMatch.Location.Y / haystack.Height * table.Height), (int)(1.0 * smallerMatch.Width / haystack.Width * table.Width), (int)(1.0 * smallerMatch.Height / haystack.Height * table.Height));

                        if (tempMatches.Where(x => (x.X <= rec.X && x.X + x.Width >= rec.X || x.X <= rec.X + rec.Width && x.X + x.Width >= rec.X + rec.Width) && (x.Y <= rec.Y && x.Y + x.Height >= rec.Y || x.Y <= rec.Y + rec.Height && x.Y + x.Height >= rec.Y + rec.Height)).Any()) continue;
                        tempMatches.Add(rec);
                    }
                    foreach (Rectangle r in tempMatches.OrderBy(m => m.X))
                    {
                        if (i == 5 && r.X > table.Width / 3 && y==0) y ++;
                        if (i == 5 && r.X > table.Width * 2 / 3&&y<=1) y++;

                        enemyAreas[(i + y).ToString()] = r;
                        y++;
                    }


                    haystack.UnlockBits(data);
                }

                lastEnemyCheck = DateTime.Now;
                WriteLine("Enemies found: " + enemyAreas.Count, enableAlerts);
            }

            ReadEnemyChips();
        }

        public static void ReadEnemyChips()
        {
            if (enemyAreas.Any())
            {

                ReadEnemyChipAreas();
                int newMax = enemyChips.Max(e => e.Value);

                if (newMax>=3*maxEnemyChips || newMax<=maxEnemyChips/3)
                {
                    WriteLine("Rechecking enemy chips max = " + newMax);
                    Thread.Sleep(1000);
                    RefreshScreen();
                    ReadEnemyChipAreas(true);
                    newMax = enemyChips.Max(e => e.Value);
                    WriteLine("Confirmed enemy chips max = " + newMax);
                }

                if (!enemyChips.Where(e => e.Value > 0).Any())
                {
                    WriteLine("No enemy chips readable, using last " + maxEnemyChips);
                }
                else if (newMax > maxEnemyChips && flopFound)
                {
                    WriteLine("Impossible increase in enemy chips post-flop to " + newMax + ", using last " + maxEnemyChips, enableAlerts);
                }
                else if (newMax < maxEnemyChips/10 && flopFound)
                {
                    WriteLine("Unlikely drop in enemy chips post-flop to "+newMax+", using last " + maxEnemyChips, enableAlerts);
                }
                else if (enemyChips.Any() )
                {
                    maxEnemyChips = newMax;
                    if (enemyChips.Count > 1 && maxEnemyChips > 100000 && maxEnemyChips.ToString().Reverse().Take(4).Last().ToString() == "1" && maxEnemyChips > enemyChips.OrderByDescending(x => x.Value).Take(2).Min(e => e.Value) * 2) // misread comma as a 1
                    {
                        WriteLine("Excluding " + maxEnemyChips + " as likely misreading of comma.");
                        maxEnemyChips = enemyChips.OrderByDescending(x => x.Value).Take(2).Min(e => e.Value);
                    }
                    else if (enemyChips.Count == 1 && hasRaiseButton && currentRaise > 0 && maxEnemyChips < currentRaise)
                    {
                        WriteLine("Likely misread of enemy chips "+newMax+" less than raise amount.");
                        if (10 * maxEnemyChips > currentRaise)
                        {
                            maxEnemyChips *= 10;
                            enemyChips[enemyChips.Keys.First()] = maxEnemyChips;
                            WriteLine("Guessing enemy chips at " + newMax + ".");
                        }
                        else if (100 * maxEnemyChips > currentRaise)
                        {
                            maxEnemyChips *= 100;
                            enemyChips[enemyChips.Keys.First()] = maxEnemyChips;
                            WriteLine("Guessing enemy chips at " + newMax + ".");
                        }
                        else if (1000 * maxEnemyChips > currentRaise)
                        {
                            maxEnemyChips *= 1000;
                            enemyChips[enemyChips.Keys.First()] = maxEnemyChips;
                            WriteLine("Guessing enemy chips at " + newMax + ".");
                        }
                    }
                    else if (enemyChips.Count == 1 && hasBetButton && currentBet > 0 && maxEnemyChips < currentBet)
                    {
                        WriteLine("Likely misread of enemy chips " + newMax + " less than bet amount.");
                        if (10 * maxEnemyChips > currentBet)
                        {
                            maxEnemyChips *= 10;
                            enemyChips[enemyChips.Keys.First()] = maxEnemyChips;
                            WriteLine("Guessing enemy chips at " + newMax + ".");
                        }
                        else if (100 * maxEnemyChips > currentBet)
                        {
                            maxEnemyChips *= 100;
                            enemyChips[enemyChips.Keys.First()] = maxEnemyChips;
                            WriteLine("Guessing enemy chips at " + newMax + ".");
                        }
                        else if (1000 * maxEnemyChips > currentBet)
                        {
                            maxEnemyChips *= 1000;
                            enemyChips[enemyChips.Keys.First()] = maxEnemyChips;
                            WriteLine("Guessing enemy chips at " + newMax + ".");
                        }
                    }

                    avgEnemyChips = (int)enemyChips.Where(e=>e.Value>0).Select(e => e.Value).DefaultIfEmpty(0).Average();
                    minEnemyChips = enemyChips.Where(e => e.Value > 0).Select(e => e.Value).DefaultIfEmpty(0).Min();

                    if (maxEnemyChips < enemyChips.Average(e => e.Value)) maxEnemyChips = avgEnemyChips;

                    if (currentCall > 0) maxEnemyChips += currentCall;
                    else if (hasCallAllInButton && currentChips > 0) maxEnemyChips += currentChips;

                }
                else minEnemyChips = maxEnemyChips = -1;
            }
        }

        public static void ReadEnemyChipAreas(bool print = false)
        {
            enemyChips.Clear();
            enemyCount = 0;
            foreach (KeyValuePair<string, Rectangle> kv in enemyAreas)
            {
                Rectangle r = kv.Value;
                string id = kv.Key;
                Bitmap enemy = CropImage(table, new Rectangle(r.X, r.Y, r.Width, r.Height));
                SaveScreenshot(enemy, "live_enemy" + id);
                Rectangle enemyNameArea = new Rectangle(0, enemy.Height * 2/5, enemy.Width, enemy.Height / 3);
                string oldEnemyFile = "live/live_enemy" + id + "_name.png";
                bool oldEnemyMatched = false;
                if (enemyEncounters.ContainsKey(id) && File.Exists(oldEnemyFile))
                {
                    Bitmap oldEnemy = (Bitmap)System.Drawing.Image.FromFile(oldEnemyFile);
                    for (int i = 0; i < 1; i++)
                    {
                        if (i > 0)
                        {
                            RefreshScreen();
                            enemy = CropImage(table, new Rectangle(r.X, r.Y, r.Width, r.Height));
                            SaveScreenshot(enemy, "live_enemy" + id);
                        }
                        Rectangle? sameEnemy = PictureContains(enemy, oldEnemy, enemyNameArea, .95f);
                        if (sameEnemy.HasValue)
                        {
                            enemyEncounters[id]++;
                            oldEnemyMatched = true;
                            break;
                        }
                    }
                }
                if(!oldEnemyMatched)
                {
                    ReadText(enemy, enemyNameArea, "enemy" + id + "_name");
                    enemyEncounters[id] = 1;
                }
                Rectangle enemyChipArea = new Rectangle(0, enemy.Height * 2 / 3, enemy.Width, enemy.Height / 3);
                int enemyChipAmt = ReadNumber(RemoveFromImage( CropImage(enemy, enemyChipArea), images["chips_comma"]), Rectangle.Empty, "enemyChips"+id);
                WriteLine("Enemy " + id+" (x"+enemyEncounters[id]+"): " + enemyChipAmt, enableAlerts || print);
                enemyChips[id] = enemyChipAmt;
                enemyCount++;
            }

        }



        public static Bitmap RemoveFromImage(Bitmap image, Bitmap remove)
        {
            int c = 0;
            try
            {
                while (true)
                {
                    c++;
                    Rectangle? results = PictureContains(image, remove);
                    if (results.HasValue)
                    {
                        using (Graphics graphics = Graphics.FromImage(image))
                        {
                            graphics.FillEllipse(Brushes.Black, results.Value);
                            graphics.DrawImage(image, new Rectangle(results.Value.Width - 1, 0, results.Value.X, image.Height), new Rectangle(0, 0, results.Value.X, image.Height), GraphicsUnit.Pixel);
                        }
                    }
                    else break;
                    if (c >= 2) break;
                }
            }
            catch { }
            return image;
        }

        public static void CheckChips()
        {
            WriteLine("Checking chips...", enableAlerts);
            int margin = (int)(-20 * scale);
            chipsArea = new Rectangle((int)(playerArea.Value.X-margin), playerArea.Value.Y + playerArea.Value.Height, (int)(playerArea.Value.Width+margin*2), images["username"].Height * 3/2);
            chips = RemoveFromImage(CropImage(table, chipsArea.Value), images["chips_comma"]);
            SaveScreenshot(AdjustContrast( Blur( MakeGrayscale( chips) )), "live_chips");

            currentChips = ReadNumber(chips, Rectangle.Empty);
            if (currentChips != -1) WriteLine(" - " + currentChips, enableAlerts);
            if (currentChips != 0)
            {
                if (currentChips >= 3 * prevChips || currentChips < -1 || (prevChips > 0 && currentChips > 0 && prevChips >= 3 * currentChips)
                    || (currentChips<=prevChips/3 && prevChips.ToString().Contains(currentChips.ToString()))
                    )
                {
                    SaveScreenshot(AdjustContrast( Blur(MakeGrayscale(chips))), "live_chipsError");
                    WriteLine("Re-reading " + currentChips + " chips to be sure...", enableAlerts);
                    Thread.Sleep(1000);
                    RefreshScreen();
                    player = CropImage(table, playerArea.Value);
                    if (PictureContains(player, images["username"], null, 0.95f).HasValue)
                    {
                        int badChips = currentChips;
                        chips = RemoveFromImage(CropImage(table, chipsArea.Value), images["chips_comma"]);
                        currentChips = ReadNumber(chips, Rectangle.Empty);
                        RescanCallAmt();
                        WriteLine("Re-read to confirm " + Live.currentChips + " chips with "+currentCall+" call value.");
                        SaveScreenshot(AdjustContrast(Blur(MakeGrayscale(chips))), "live_chips");
                        if ((currentChips == badChips || currentChips < prevChips / 3 || (currentChips <= prevChips / 3 && prevChips.ToString().Contains(currentChips.ToString()))) && currentChips < prevChips && prevChips > 0 && !expectedChipsDecrease)
                        {
                            currentChips = prevChips;
                            WriteLine("Re-using previous chips: " + currentChips);
                        }
                        else if(currentChips<prevChips && currentChips<currentCall && prevChips>currentCall)
                        {
                            currentChips = prevChips;
                            WriteLine("Re-using previous chips, impossible call higher than chips read: " + currentChips);
                        }
                        else if (currentCall < 0 && currentChips < prevChips && currentChips < Math.Max(lastActualMinBet, lastMinBet) && prevChips > Math.Max(lastActualMinBet, lastMinBet))
                        {
                            currentChips = prevChips;
                            WriteLine("Re-using previous chips, unlikely less than minbet "+Math.Max(lastActualMinBet, lastMinBet)+": " + currentChips);
                        }
                    }
                    else
                    {
                        
                        WriteLine("Ignoring chips value, unable to confirm player name.", enableAlerts);
                        currentChips = -1;
                    }
                }
                if (currentChips != -1) prevChips = currentChips;
            }

            
        }

        // from https://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale
        public static Bitmap MakeGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][] 
          {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
          });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }

        public static Bitmap Blur(Bitmap image)
        {
            //return image;
            AForge.Imaging.Filters.GaussianBlur blur = new AForge.Imaging.Filters.GaussianBlur(.25, 2);
            blur.ApplyInPlace(image);
            return image;
        }

        // from https://stackoverflow.com/questions/3115076/adjust-the-contrast-of-an-image-in-c-sharp-efficiently
        public static Bitmap AdjustContrast(Bitmap Image, float Value=50)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }

        public static int ReadNumber(Bitmap img) { return ReadNumber(img, new Rectangle(0,0,img.Width,img.Height)); }
        public static int ReadNumber(Bitmap img, Rectangle area, string filename="")
        {
            Rectangle newArea = new Rectangle(area.X, area.Y, area.X + area.Width > img.Width ? img.Width - area.X : area.Width, area.Y + area.Height > img.Height ? img.Height - area.Y : area.Height);
            try
            {
                Bitmap refinedImg = area.IsEmpty? img: CropImage(img, newArea);
                if (!area.IsEmpty) refinedImg = RemoveFromImage(refinedImg, images["button_comma"]);
                refinedImg = AdjustContrast(Blur(MakeGrayscale(refinedImg)));
                try
                {
                    if (filename != "") SaveScreenshot(refinedImg, "live_" + filename);
                }
                catch { }

                int result = -1;

                List<Word> results = tessnet.DoOCR(refinedImg, Rectangle.Empty);
                
                if (results.Any())
                {
                    foreach (Word r in results.OrderByDescending(res => Regex.Replace(res.Text, @"[^0-9]", "").Length))
                    {
                        string number = Regex.Replace(r.Text, @"[^0-9\.]", "");
                        if (number.Contains(".") && Convert.ToDouble(number)<500) hasDecimals = true;
                        if (number != "" )
                        {
                            //result = Convert.ToInt32(Math.Max(1, Convert.ToDouble(number)));
                            if (number.First().ToString() == "0") number = "1" + number;
                            result = Convert.ToInt32(number.Replace(".", ""));
                            break;
                        }
                    }
                }
                // Tessnet memory leak
                tessnet.Dispose();
                tessnet.Clear();
                return result;
            }
            catch (Exception ex)
            {
                try
                {
                    //WriteLine("Error number read: " + ex);
                    SaveScreenshot(CropImage(img, newArea), "live_readNumberError");
                }
                catch {
                    try
                    {
                        SaveScreenshot(img, "live_readNumberError");
                    }
                    catch { }
                }

                return -1;
            }
        }

        public static string ReadText(Bitmap img, Rectangle area, string filename = "")
        {
            Rectangle newArea = new Rectangle(area.X, area.Y, area.X + area.Width > img.Width ? img.Width - area.X : area.Width, area.Y + area.Height > img.Height ? img.Height - area.Y : area.Height);
            try
            {

                try
                {
                    if (filename != "") SaveScreenshot(CropImage(img, newArea), "live_" + filename);
                }
                catch { }

                string result = "";



                List<Word> results = tessnet.DoOCR(img, newArea);

                if (results.Any())
                {
                    foreach (Word r in results.OrderByDescending(res => res.Confidence))
                    {
                        result = r.Text;
                        //Console.WriteLine(" - " + result);
                        break;
                    }
                }
                // Tessnet memory leak
                tessnet.Dispose();
                tessnet.Clear();
                return result;
            }
            catch (Exception ex)
            {
                try
                {
                    SaveScreenshot(CropImage(img, newArea), "live_readTextError");
                }
                catch
                {
                    try
                    {
                        SaveScreenshot(img, "live_readTextError");
                    }
                    catch { }
                }

                return "";
            }
        }

        public static void CheckCards(float accuracy=.97f)
        {
            bool printOutput = false;
            if (lastCard1 != null) printOutput = false;
            WriteLine("Checking cards with accuracy " + accuracy, printOutput);
            Deck d = new Deck();
            HashSet<Card> usedCards = new HashSet<Card>();
            for (int i = 0; i < 52; i++)
            {
                if (!d.deck.Any()) break;
                Card card; bool cached = false;
                if (lastCard1 != null && !usedCards.Contains(lastCard1))
                {
                    card = d.DealCard(lastCard1);
                    lastCard1 = null;
                    cached = true;
                }
                else if (lastCard2 != null && !usedCards.Contains(lastCard2) && !(card1 != null && lastCard2 == card1))
                {
                    card = d.DealCard(lastCard2);
                    lastCard2 = null;
                    cached = true;
                }
                else
                {
                    card = d.DealCard();
                    card.accuracy = accuracy + card.accuracyAdjustment;
                }
                if (card1!=null && card == card1) continue;
                string c = card.ToString();
                Card originalCard = card;
                if (images.ContainsKey(c))
                {
                    //WriteLine("Checking player hand for " + c);
                    Rectangle? match = PictureContains(player, images[c], null, card.accuracy);
                    float accuracyScore = matchScore;
                    if (match.HasValue)
                    {
                        WriteLine("Card found: " + c + " (" + accuracyScore.ToString("N3") + " accuracy, scale=" + (match.Value.Width / images[c].Width).ToString("N2") + ")", printOutput);
                        Bitmap cardText = CropImage(player, new Rectangle(match.Value.X, match.Value.Y, match.Value.Width/2, player.Height/2));
                        SaveScreenshot(cardText, "live_cardText" + (card1 == null ? "1" : "2"));

                        Rectangle? cornerMatch = PictureContains(cardText, CropImage(images[c], new Rectangle(2, 2, cardText.Width-4, cardText.Height-4)), null, .9f + card.accuracyAdjustment);
                        if (cornerMatch.HasValue)
                        {
                            WriteLine(" - Corner text confirmed.", printOutput);
                        }
                        else
                        {
                            WriteLine(" - Failed to confirm corner text.", printOutput);
                            continue;
                        }
                        try
                        {
                            float cornerAccuracy = matchScore;
                            if (card.looksSimilarTo!=null)
                            {
                                bool replaced = false;
                                foreach (Rank similarRank in card.looksSimilarTo)
                                {
                                    bool m = false;
                                    Card otherCard = new Card(similarRank, (similarRank != card.rank ? card.suite : card.suite == Suite.Club ? Suite.Spade : card.suite == Suite.Spade ? Suite.Club : card.suite == Suite.Heart ? Suite.Diamond : Suite.Heart));
                                    if (usedCards.Contains(otherCard) || (card1 != null && otherCard == card1)) continue;

                                    
                                        Rectangle? doubleCheck = PictureContains(player, images[otherCard.ToString()], new Rectangle((int)Math.Max(0, match.Value.X - 10 * scale), 0, match.Value.X <= player.Width / 3 ? (int)(match.Value.Width + 20 * scale) : (int)(player.Width - match.Value.X + 10 * scale), player.Height), .9f);

                                        float replacementScore = matchScore;
                                        if (replacementScore > accuracyScore -.005)
                                        {

                                            m = true;
                                            WriteLine(" - Better match " + otherCard.ToString() + " (" + replacementScore.ToString("N3") + ">" + accuracyScore.ToString("N3") + ")", printOutput);
                                        }
                                        else
                                            WriteLine(" - Double checked against " + otherCard + " (" + replacementScore.ToString("N3") + "<" + accuracyScore.ToString("N3") + ")", printOutput);

                                        if (m)
                                        {
                                            Bitmap corner = CropImage(player, new Rectangle(doubleCheck.Value.X, doubleCheck.Value.Y, doubleCheck.Value.Width / 2, player.Height / 2));
                                            Bitmap cornerTemplate = CropImage(images[otherCard.ToString()], new Rectangle(2, 2, doubleCheck.Value.Width / 2 - 4, player.Height / 2 - 4));
                                            if (saveScreenshots)
                                            {
                                                SaveScreenshot(corner, "live_cardText" + (card1 == null ? "1" : "2")+"_2");
                                                SaveScreenshot(cornerTemplate, "live_cardText" + (card1 == null ? "1" : "2") + "_3");
                                            }
                                            doubleCheck = PictureContains(corner, cornerTemplate, null, .9f);
                                            if (matchScore > cornerAccuracy)
                                            {
                                                WriteLine(" - Replaced by better corner match " + otherCard.ToString() + " (" + matchScore.ToString("N3") + ">" + cornerAccuracy.ToString("N3") + ")", printOutput);
                                                card = otherCard;
                                                cornerAccuracy = matchScore;
                                                accuracyScore = replacementScore;
                                                replaced = true;
                                            }
                                            else WriteLine(" - Double checked against corner " + otherCard + " (" + matchScore.ToString("N3") + "<" + cornerAccuracy.ToString("N3") + ")", printOutput);
                                        }
                                    
                                }
                                if (replaced && d.deck.Contains(card)) d.DealCard(card);
                            }
                        }
                        catch (Exception ex) { WriteLine("Error on double-check: " + ex); }


                        card.accuracy = accuracyScore-.01f;
                        usedCards.Add(card);
                        if (card1 == null)
                        {
                            card1 = card;
                            player = CropImage(player, new Rectangle((int)(match.Value.X <= player.Width / 3? match.Value.X + match.Value.Width:0), 0, player.Width - match.Value.Width, player.Height));
                            SaveScreenshot(player, "live_playerCard2");
                        }
                        else if (card2 == null)
                        {
                           
                                card2 = card;
                                cardsFound = true;
                                lastCard1 = card1;
                                lastCard2 = card2;
                                return;
                        }
                        if (card != originalCard) card = originalCard;
                    }
                }
            }

            if (card2 == null && accuracy > .92f)
            {
                if (printOutput) WriteLine("Rechecking hand cards with lower accuracy...", printOutput);
                CheckCards(accuracy - .02f);
            }
            else
            {
                WriteLine("Failed to read cards.");
            }
        }


        public static void CheckFlop(bool readFlop=true)
        {
            
            if (!flopArea.HasValue) flopArea = new Rectangle((int)(playerArea.Value.X - 230 * scale), playerArea.Value.Y - table.Height / 3, (int)(playerArea.Value.Width + 460 * scale), (int)(table.Height *.15));
            flop = CropImage(table, flopArea.Value);
            SaveScreenshot(flop, "live_flop");
            flopFound = HasFlopCards();

            if (readFlop &&flopFound)
            {
                //WriteLine("Parsing flop cards...");
                CheckFlopCards();
            }
            else
            {
                prevFlopCt = 0;
                lastFlopCard1 = lastFlopCard2 = lastFlopCard3 = lastFlopCard4 = lastFlopCard5 = null;
                //WriteLine("No flop cards.");
            }

            // Pot total
            /*if (flopArea.HasValue)
            {
                Rectangle potArea = new Rectangle(playerArea.Value.X, (int)(flopArea.Value.Y - 20 * scale), playerArea.Value.Width, (int)(50 * scale));
                Bitmap pot = CropImage(table, potArea);
                SaveScreenshot(pot, "live_pot");
                currentPot = ReadNumber(pot, Rectangle.Empty);
                //WriteLine("Pot: " + currentPot);
                currentPot = -1; // not reliable
            }*/
        }

        public static bool HasFlopCards()
        {
            bool tableBlocked = !PictureContains(flop, images["preflop"], new Rectangle(flop.Width / 3, 0, flop.Width / 3, flop.Height)).HasValue;
            if (tableBlocked)
            {
                bool cardShape = HasFlopCardInPartitionNumber(1);
                //WriteLine("Flop cards existence detected with " + matchScore.ToString("N3") + " accuracy.");
                if (cardShape) return true;
            }
            return false;
        }
        public static bool ImageContainsCard(Bitmap img)
        {
            return PictureContains(img, images["Ad"], null, .7f).HasValue;
        }
        public static bool HasFlopCardInPartitionNumber(int num=1)
        {
            return ImageContainsCard(GetFlopCardPartitionNumber(num));
        }

        public static Bitmap GetFlopCardPartitionNumber(int num = 1)
        {
            return ResizeImage(CropImage(flop, new Rectangle(num == 1 ? 0 : num == 2 ? flop.Width / 5 : num == 3 ? flop.Width / 3 : num == 4 ? flop.Width / 2 : flop.Width * 3 / 5, 0, num == 5 ? flop.Width / 3 : flop.Width / 4, flop.Height * 9 / 10)), .8);
        }

        public static Dictionary<int, Bitmap> GetFlopPartitions()
        {
            Dictionary<int, Bitmap> images = new Dictionary<int, Bitmap>();
            for (int num = 1; num <= 5; num++)
            {
                Bitmap img = ResizeImage(CropImage(flop, new Rectangle(num == 1 ? 0 : num == 2 ? flop.Width / 5 : num == 3 ? flop.Width / 3 : num == 4 ? flop.Width / 2 : flop.Width * 3 / 5, 0,  num==5? flop.Width/3:flop.Width / 4, flop.Height * 9 / 10)), .8);
                SaveScreenshot(img, "live_flopPartition" + num);
                if (ImageContainsCard(img)) images[num] = img;
                else break;
            }
            return images;
        }

        public static void CheckFlopCards(float accuracy = .97f)
        {
            bool printOutput = false;
            //if (lastFlopCard3 != null) printOutput = false;
            WriteLine("Checking flop cards with accuracy " + accuracy+", expecting >="+prevFlopCt, true || printOutput);
            Deck d = new Deck();
            HashSet<Card> usedCards = new HashSet<Card>();
            if (card1 != null)
            {
                d.DealCard(card1);
                usedCards.Add(card1);
            }
            if (card2 != null)
            {
                d.DealCard(card2);
                usedCards.Add(card2);
            }
            Dictionary<int, Bitmap> flopPartitions = GetFlopPartitions();
            int flopCt = 0; int highestCardPosition = 0;
            while (d.deck.Any())
            {
                if (!d.deck.Any()) break;
                Card card; bool cached = false;
                if (lastFlopCard1 != null && !usedCards.Contains(lastFlopCard1))
                {
                    card = d.DealCard(lastFlopCard1);
                    lastFlopCard1 = null;
                    cached = true;
                }
                else if (lastFlopCard2 != null && !usedCards.Contains(lastFlopCard2))
                {
                    card = d.DealCard(lastFlopCard2);
                    lastFlopCard2 = null;
                    cached = true;
                }
                else if (lastFlopCard3 != null && !usedCards.Contains(lastFlopCard3))
                {
                    card = d.DealCard(lastFlopCard3);
                    lastFlopCard3 = null;
                    cached = true;
                }
                else if (lastFlopCard4 != null && !usedCards.Contains(lastFlopCard4))
                {
                    card = d.DealCard(lastFlopCard4);
                    lastFlopCard4 = null;
                    cached = true;
                }
                else if (lastFlopCard5 != null && !usedCards.Contains(lastFlopCard5))
                {
                    card = d.DealCard(lastFlopCard5);
                    lastFlopCard5 = null;
                    cached = true;
                }
                else
                {
                    card = d.DealCard();
                    card.accuracy = accuracy + card.accuracyAdjustment;
                }
                
                string c = card.ToString();

                Card originalCard = card;

                
                if (images.ContainsKey(c))
                {
                    bool matchFound = false;
                    foreach (KeyValuePair<int, Bitmap> kv in flopPartitions.ToDictionary(x=>x.Key, x=>x.Value))
                    {
                        if (flopCard1 != null && card == flopCard1) continue;
                        if (flopCard2 != null && card == flopCard2) continue;
                        if (flopCard3 != null && card == flopCard3) continue;
                        if (flopCard4 != null && card == flopCard4) continue;
                        if (flopCard5 != null && card == flopCard5) continue;
                        if (card1 != null && card == card1) continue;
                        if (card2 != null && card == card2) continue;

                        Bitmap flopPartition = kv.Value;
                        Rectangle? match = PictureContains(flopPartition, images[c], null, card.accuracy);
                        float accuracyScore=matchScore;
                        if (match.HasValue)
                        {
                            WriteLine("Flop card found: " + c + " (" + accuracyScore.ToString("N3") + " accuracy, scale=" + (match.Value.Width / images[c].Width).ToString("N2") + ")", printOutput);
                            Bitmap cardText = CropImage(flopPartition, new Rectangle(match.Value.X, match.Value.Y, match.Value.Width / 2, flopPartition.Height / 2));
                            SaveScreenshot(cardText, "live_flopCardText" + kv.Key);

                            Rectangle? cornerMatch = PictureContains(cardText, CropImage(images[c], new Rectangle(2, 2, cardText.Width-4, cardText.Height-4)), null, .9f + card.accuracyAdjustment);
                            if (cornerMatch.HasValue)
                            {
                                WriteLine(" - Corner text confirmed.", printOutput);
                            }
                            else
                            {
                                WriteLine(" - Failed to confirm corner text.", printOutput);
                                continue;
                            }

                            try
                            {
                                float cornerAccuracy = matchScore;
                                if (card.looksSimilarTo != null)
                                {
                                    bool replaced = false;
                                    foreach (Rank similarRank in card.looksSimilarTo)
                                    {
                                        bool m = false;
                                        Card otherCard = new Card(similarRank, (similarRank!=card.rank? card.suite: card.suite==Suite.Club? Suite.Spade: card.suite==Suite.Spade? Suite.Club: card.suite==Suite.Heart? Suite.Diamond: Suite.Heart));
                                        if (usedCards.Contains(otherCard)) continue;

                                        
                                            Rectangle? doubleCheck = PictureContains(flopPartition, images[otherCard.ToString()], new Rectangle((int)Math.Max(0, match.Value.X - 10 * scale), 0, match.Value.X + match.Value.X + 20 * scale > flopPartition.Width ? (int)(match.Value.Width + 20 * scale) : (int)(flopPartition.Width - match.Value.X + 10 * scale), flopPartition.Height), .9f);

                                            float replacementScore = matchScore;
                                            if (replacementScore > accuracyScore -.005)
                                            {

                                                m = true;
                                                WriteLine(" - Better match " + otherCard.ToString() + " (" + matchScore.ToString("N3") + ">" + accuracyScore.ToString("N3") + ")", printOutput);
                                            }
                                            else
                                                WriteLine(" - Double checked against " + otherCard + " (" + matchScore.ToString("N3") + "<" + accuracyScore.ToString("N3") + ")", printOutput);

                                            if (m)
                                            {
                                                Bitmap corner = CropImage(flopPartition, new Rectangle(doubleCheck.Value.X, doubleCheck.Value.Y, doubleCheck.Value.Width / 2, flopPartition.Height / 2));
                                                Bitmap cornerTemplate = CropImage(images[otherCard.ToString()], new Rectangle(2, 2, doubleCheck.Value.Width / 2 - 4, flopPartition.Height / 2 - 4));
                                                if (saveScreenshots)
                                                {
                                                    SaveScreenshot(corner, "live_flopCardText" + kv.Key + "_2");
                                                    SaveScreenshot(cornerTemplate, "live_flopCardText" + kv.Key + "_3");
                                                }
                                                doubleCheck = PictureContains(corner, cornerTemplate, null, .9f);
                                                if (matchScore > cornerAccuracy)
                                                {
                                                    WriteLine(" - Replaced by better corner match " + otherCard.ToString() + " (" + matchScore.ToString("N3") + ">" + cornerAccuracy.ToString("N2") + ")", printOutput);
                                                    card = otherCard;
                                                    cornerAccuracy = matchScore;
                                                    accuracyScore = replacementScore;
                                                    replaced = true;
                                                }
                                                else WriteLine(" - Double checked against corner " + otherCard + " (" + matchScore.ToString("N3") + "<" + cornerAccuracy.ToString("N3") + ")", printOutput);
                                            }
                                       
                                    }
                                    if (replaced && d.deck.Contains(card))
                                    {
                                        d.DealCard(card);
                                    }
                                }
                            }
                            catch (Exception ex) { WriteLine("Error on flop double-check: " + ex); }

                           

                            flopPartitions.Remove(kv.Key);
                            flopCt++;
                            card.accuracy = accuracyScore - .01f;
                            highestCardPosition = Math.Max(highestCardPosition, kv.Key);
                            usedCards.Add(card);
                            matchFound = true;
                            if (flopCard1 == null)
                            {
                                flopCard1 = card;
                                lastFlopCard1 = flopCard1;
                            }
                            else if (flopCard2 == null)
                            {
                                flopCard2 = card;
                                lastFlopCard1 = flopCard1;
                                lastFlopCard2 = flopCard2;
                            }
                            else if (flopCard3 == null)
                            {

                                flopCard3 = card;
                                lastFlopCard1 = flopCard1;
                                lastFlopCard2 = flopCard2;
                                lastFlopCard3 = flopCard3;
                            }
                            else if (flopCard4 == null)
                            {

                                flopCard4 = card;
                                lastFlopCard1 = flopCard1;
                                lastFlopCard2 = flopCard2;
                                lastFlopCard3 = flopCard3;
                                lastFlopCard4 = flopCard4;
                            }
                            else if (flopCard5 == null)
                            {

                                flopCard5 = card;
                                flopFound = true;
                                lastFlopCard1 = flopCard1;
                                lastFlopCard2 = flopCard2;
                                lastFlopCard3 = flopCard3;
                                lastFlopCard4 = flopCard4;
                                lastFlopCard5 = flopCard5;
                                prevFlopCt = 5;
                                WriteLine("5 Flop cards read: " + flopCard1 + " | " + flopCard2 + " | " + flopCard3 + " | " + flopCard4 + " | " + flopCard5, printOutput);
                                return;
                            }
                            if (card != originalCard) card = originalCard;
                            else break;
                        }
                    }
                    if (!matchFound && cached)
                    {
                        WriteLine("Unexpected cache miss: " + card + " at " + card.accuracy.ToString("N3") + " accuracy.");
                        printOutput = true;
                    }
                }
            }
            if (flopCard3 != null && flopCt < 5 && HasFlopCardInPartitionNumber(flopCt + 1)) highestCardPosition = flopCt + 1;
            if (prevFlopCt + 1 < highestCardPosition) highestCardPosition = prevFlopCt + 1;
            if (flopCard3 != null && flopCt >= prevFlopCt && flopCt<highestCardPosition && prevFlopCt < highestCardPosition && !HasFlopCardInPartitionNumber(highestCardPosition)) highestCardPosition = prevFlopCt;
            string fail = ""; Card backup1 = flopCard1, backup2 = flopCard2, backup3 = flopCard3, backup4 = flopCard4, backup5 = flopCard5;
            if (flopCard3 != null && flopCt >= prevFlopCt && flopCt>=highestCardPosition)
            {
                lastFlopCard1 = flopCard1;
                lastFlopCard2 = flopCard2;
                lastFlopCard3 = flopCard3;
                lastFlopCard4 = flopCard4;
                lastFlopCard5 = flopCard5;
                flopFound = true;
                prevFlopCt = flopCt;
                WriteLine("Flop cards read: " + flopCard1 + " | " + flopCard2 + " | " + flopCard3 + " | " + flopCard4 + " | " + flopCard5, printOutput);
                return; // success
            }
            else
            {
                fail = "Found " + flopCt + " flop card(s), expecting >=" + Math.Max(highestCardPosition, Math.Max(3, prevFlopCt))+"\n "+
                    " - " + flopCard1 + " | " + flopCard2 + " | " + flopCard3 + " | " + flopCard4 + " | " + flopCard5;
                WriteLine(fail, printOutput);
                flopCard1 = flopCard2 = flopCard3 = flopCard4 = flopCard5 = null;
                flopFound = false;
            }

            if (!flopFound && accuracy > .93f)
            {
                WriteLine("Rechecking flop cards with lower accuracy...", printOutput);
                flop = CropImage(CropImage(GetScreen(), tableArea.Value), flopArea.Value);
                CheckFlopCards(accuracy - .02f);
            }
            else
            {
                WriteLine("Failed to read flop cards.");
                WriteLine(fail);
                if (backup3 != null)
                {
                    flopCard1 = backup1;
                    flopCard2 = backup2;
                    flopCard3 = backup3;
                    flopCard4 = backup4;
                    flopCard5 = backup5;
                    WriteLine("Still use " + flopCt + " flop cards read...");
                    flopFound = true;
                }
            }
        }

        

        public static bool HasTable()
        {
            if (screen != null)
            {
                double scale = 0.25;
                Bitmap smallerScreen = ResizeImage(screen, scale);
                Bitmap smallerTableTemplate = images["table"];

                if (smallerTableTemplate.Width > smallerScreen.Width) smallerTableTemplate = new AForge.Imaging.Filters.ResizeBicubic((int)(smallerScreen.Width*.5), (int)(smallerScreen.Width*.5 / smallerTableTemplate.Width * smallerTableTemplate.Height)).Apply(smallerTableTemplate);
                if (smallerTableTemplate.Height > smallerScreen.Height) smallerTableTemplate = new AForge.Imaging.Filters.ResizeBicubic((int)(smallerScreen.Height * .5 / smallerTableTemplate.Height * smallerTableTemplate.Width), (int)(smallerScreen.Height * .5)).Apply(smallerTableTemplate);

                Rectangle? smallerMatch = PictureContains(smallerScreen, smallerTableTemplate, null, 0.75f);
                if (smallerMatch.HasValue)
                {
                    smallerTableArea = smallerMatch;
                    tableArea = new Rectangle((int)(1.0*smallerMatch.Value.Location.X / smallerScreen.Width * screen.Width), (int)(1.0*smallerMatch.Value.Location.Y / smallerScreen.Height * screen.Height-50*scale), (int)(1.0*smallerMatch.Value.Width / smallerScreen.Width * screen.Width*1.2), (int)(1.0*smallerMatch.Value.Height / smallerScreen.Height * screen.Height*1.2));

                    table = CropImage(screen, tableArea.Value);
                    SaveScreenshot(table, "live_table");
                    WriteLine("Live game found at: " + tableArea.Value.Location);
                }
                else table = null;
            }
            return tableArea.HasValue;
        }

        public static Rectangle playerSearchArea { get { return new Rectangle((table.Width - (table.Width / 2)) / 2, table.Height /2, table.Width / 2, table.Height / 2); } }
        public static bool HasUser()
        {
            if (table != null && tableArea != null)
            {
                playerArea = PictureContains(table, images["username"], playerSearchArea, 0.95f);
                if (playerArea.HasValue)
                {
                    scale = 1.0 * playerArea.Value.Width / images["username"].Width;
                    WriteLine("Live game scale: " + scale);
                    playerArea = new Rectangle((int)(playerArea.Value.Location.X - 15 * scale), (int)(playerArea.Value.Location.Y - 100 * scale), (int)(playerArea.Value.Width + 30 * scale), (int)(playerArea.Value.Height + 100 * scale));
                    player = CropImage(table, playerArea.Value);
                    SaveScreenshot(player, "live_player");
                    WriteLine("Live user found at: " + playerArea.Value.Location);

                    if (playerArea.Value.X + playerArea.Value.Width / 2 < table.Width / 2)
                    {
                        WriteLine("Extending left side of table area based on player center...");
                        int extraWidth = table.Width / 2 - playerArea.Value.X + playerArea.Value.Width / 2;
                        int extraHeight = (int)(.85*table.Height- playerArea.Value.Y - playerArea.Value.Height);
                        int oldTableY = tableArea.Value.Y;
                        tableArea = new Rectangle(tableArea.Value.Location.X - extraWidth, Math.Max(0, tableArea.Value.Y-extraHeight), tableArea.Value.Width + extraWidth, tableArea.Value.Height);
                        table = CropImage(screen, tableArea.Value);
                        SaveScreenshot(table, "live_table");
                        WriteLine("Live game recentered at: " + tableArea.Value.Location);
                        playerArea = new Rectangle(playerArea.Value.Location.X + extraWidth, playerArea.Value.Y + (oldTableY- tableArea.Value.Y), playerArea.Value.Width, playerArea.Value.Height);
                        player = CropImage(table, playerArea.Value);
                        SaveScreenshot(player, "live_player");
                    }
                }
                else player = null;
            }
            return playerArea.HasValue;
        }
        
        public static bool HasTurnButtons()
        {
            bool buttonsFound = false; 
            if (table != null && tableArea != null)
            {
                float accuracy = .92f;

                Rectangle buttonsArea = new Rectangle(table.Width / 4, (int)(playerArea.Value.Y+100*scale), table.Width * 3 / 4, (int)(table.Height - playerArea.Value.Y-100*scale));
                SaveScreenshot(CropImage(table, buttonsArea), "live_buttons");

                if (!buttonsFound || !foldArea.HasValue)
                {
                    Rectangle? newFoldArea = PictureContains(table, images["fold"], foldArea.HasValue ? foldArea : buttonsArea, accuracy-(foldArea.HasValue? .02f:0));
                    if (newFoldArea.HasValue)
                    {
                        foldArea = newFoldArea;
                        fold = CropImage(table, foldArea.Value);
                        SaveScreenshot(fold, "live_fold");
                        WriteLine("Fold button found.", enableAlerts);
                        buttonsFound = true;
                        hasCheckButton = hasBetButton = false;
                    }
                    else
                    {
                        WriteLine("No fold button found.", enableAlerts);
                        hasCallAllInButton = hasRaiseButton = hasCallButton = false;
                    }
                }

                if (buttonsFound )
                {
                    callArea = PictureContains(table, images["call"], callArea.HasValue ? callArea : buttonsArea, accuracy - (callArea.HasValue ? .02f : 0));
                    if (callArea.HasValue)
                    {
                        call = CropImage(table, callArea.Value);
                        SaveScreenshot(call, "live_call");
                        WriteLine("Call button found.", enableAlerts);
                        buttonsFound = true;
                        hasCallButton = true;
                        currentCall = ReadNumber(table, new Rectangle((int)(callArea.Value.X - 40 * scale), callArea.Value.Y + callArea.Value.Height, (int)(callArea.Value.Width + 80 * scale), (int)(callArea.Value.Height*2)), "callAmt");
                        if(currentCall!=-1) WriteLine("Current call = " + currentCall, enableAlerts);
                        
                        if (currentChips>0&&(currentCall > currentChips||currentCall*1000<currentChips))
                        {
                            WriteLine(" - Current call "+currentCall+" ignored as misread.");
                            RescanCallAmt();
                            if (currentCall != -1)
                            {
                                WriteLine("Current call = " + currentCall);
                                if (currentCall > currentChips && currentCall / 10 <= currentChips && currentCall >= 11000 && currentCall / 3 >= lastMinBet && currentCall.ToString().Reverse().Take(4).Last().ToString() == "1")
                                {
                                    int tempCall = (int)(currentCall / 10000) * 1000 + (currentCall % 1000);
                                    if (tempCall > lastMinBet) currentCall = tempCall;
                                    else WriteLine("Current call corrected to: " + currentCall);
                                }
                                else if (currentChips > 0 && (currentCall > currentChips || currentCall * 1000 < currentChips))
                                {
                                    currentCall = lastMinBet;
                                    if (flopFound) currentCall = currentChips;
                                    WriteLine("Current call adjusted to: " + currentCall);
                                }
                            }
                            else if (flopFound) currentCall = currentChips;
                        }
                        else if (currentCall / 3 >= Math.Max(lastMinBet, lastActualMinBet) && currentCall >= 11000  && currentCall / 3 >= lastMinBet && currentCall.ToString().Reverse().Take(4).Last().ToString() == "1" && !(currentChips>=20*lastMinBet && currentCall>=currentChips/3 && currentCall<=currentChips))
                        {
                            WriteLine("Current call " + currentCall + " likely misreading of comma.");
                            bool usingHalfRaise = false;
                            if (raiseArea.HasValue)
                            {
                                Rectangle? newRaiseArea = PictureContains(table, images["raise"], raiseArea, accuracy - .02f);
                                if (newRaiseArea.HasValue)
                                {
                                    raiseArea = newRaiseArea;
                                    currentRaise = ReadNumber(table, new Rectangle((int)(raiseArea.Value.X - 40 * scale), raiseArea.Value.Y + raiseArea.Value.Height, (int)(raiseArea.Value.Width + 80 * scale), (int)(raiseArea.Value.Height * 2)), "raiseAmt");
                                    if (currentRaise >= 2 * lastActualMinBet && currentRaise < currentCall && !(currentRaise >= Math.Max(lastMinBet, lastActualMinBet) * 3 && currentRaise >= 11000  && currentCall.ToString().Reverse().Take(4).Last().ToString() == "1"))
                                    {
                                        currentCall = currentRaise / 2;
                                        WriteLine("Current call adjusted to half of raise: " + currentCall / 2);
                                        usingHalfRaise = true;
                                    }
                                    if (!usingHalfRaise)
                                    {

                                        int tempCall = (int)(currentCall / 10000) * 1000 + (currentCall % 1000);
                                        if (tempCall > lastMinBet) currentCall = tempCall;
                                        else WriteLine("Current call corrected to: " + currentCall);
                                    }
                                }
                                else WriteLine("Avoiding correction down out of caution from lack of raise button.");
                            }
                            else WriteLine("Avoiding correction down out of caution from lack of raise button.");
                        }

                        if (false && currentCall < lastMinBet / 2 && currentCall * 10 < currentChips )
                        {
                            WriteLine(" - Current call " + currentCall + " too low misread, re-reading...");
                            RescanCallAmt();
                            if (currentCall != -1)
                            {
                                WriteLine("Current call = " + currentCall);
                                if (currentCall < lastMinBet / 2 && currentCall * 10 < currentChips)
                                {
                                    currentCall = lastMinBet;
                                    if (flopFound) currentCall = currentChips;
                                    WriteLine("Current call adjusted to: " + currentCall);
                                }
                            }
                            else if (flopFound) currentCall = currentChips;
                        }
                        if (currentCall < lastActualMinBet/2)
                        {
                            WriteLine(" - Current call " + currentCall + " lower than minbet " + lastActualMinBet + " misread, re-reading...");
                            RescanCallAmt();
                            if (currentCall != -1) WriteLine("Current call = " + currentCall);
                            if (currentCall < lastActualMinBet)
                            {
                                bool usingHalfRaise = false;
                                if (raiseArea.HasValue)
                                {
                                    Rectangle? newRaiseArea = PictureContains(table, images["raise"], raiseArea , accuracy -  .02f );
                                    if (newRaiseArea.HasValue)
                                    {
                                        raiseArea = newRaiseArea;
                                        currentRaise = ReadNumber(table, new Rectangle((int)(raiseArea.Value.X - 40 * scale), raiseArea.Value.Y + raiseArea.Value.Height, (int)(raiseArea.Value.Width + 80 * scale), (int)(raiseArea.Value.Height * 2)), "raiseAmt");
                                        if (currentRaise >= 2 * lastActualMinBet && currentRaise < currentChips && !(currentRaise >= Math.Max(lastMinBet, lastActualMinBet) * 3 && currentRaise >= 11000 && currentCall.ToString().Reverse().Take(4).Last().ToString() == "1"))
                                        {
                                            currentCall = currentRaise / 2;
                                            WriteLine("Current call adjusted to half of raise: " + currentCall);
                                            usingHalfRaise = true;
                                        }
                                    }
                                }
                                if(!usingHalfRaise)
                                {
                                    if (10 * currentCall < currentChips) currentCall = (int)(currentCall / 10000) * 1000 +( currentCall%1000);
                                    else currentCall = lastActualMinBet;
                                    if (currentCall < lastMinBet) currentCall = lastMinBet;
                                    if (flopFound || prevFlopCt>=3) currentCall = currentChips;
                                    WriteLine("Current call adjusted to: " + currentCall);
                                }
                            }
                        }
                    }
                    else
                    {
                        WriteLine("No call button found.", enableAlerts);
                        hasCallButton = false;
                    }
                }
                
                

                if (!buttonsFound)
                {
                    Rectangle? underPlayerArea = new Rectangle(playerArea.Value.X, playerArea.Value.Y + playerArea.Value.Height, playerArea.Value.Width, tableArea.Value.Height - (playerArea.Value.Y + playerArea.Value.Height));
                    SaveScreenshot(CropImage(table, underPlayerArea.Value), "live_underPlayerArea");
                    WriteLine("No raise/fold buttons found.", enableAlerts);
                    foldPreArea = PictureContains(table, images["fold_pre"], foldPreArea.HasValue ? foldPreArea : underPlayerArea, accuracy - (foldPreArea.HasValue ? .02f : 0));
                    if (foldPreArea.HasValue)
                    {
                        WriteLine("Pre-fold button found.", enableAlerts);
                        if (callArea.HasValue) currentCall = ReadNumber(table, new Rectangle((int)(callArea.Value.X - 40 * scale), callArea.Value.Y + callArea.Value.Height, (int)(callArea.Value.Width + 80 * scale), (int)(callArea.Value.Height * 2)), "callAmt");
                        return false;
                    }
                    else 
                    {
                        checkPreArea = PictureContains(table, images["check_pre"], checkPreArea.HasValue ? checkPreArea : underPlayerArea, accuracy - (checkPreArea.HasValue ? .02f : 0));
                        if (checkPreArea.HasValue)
                        {
                            WriteLine("Pre-check button found.", enableAlerts);
                            return false;
                        }
                        else
                        {
                            iAmBackArea = PictureContains(table, images["iamback"], iAmBackArea.HasValue ? iAmBackArea : underPlayerArea, .9f);
                            if (iAmBackArea.HasValue)
                            {
                                WriteLine("I-am-back button found. Re-entering game...");
                                Click(iAmBackArea);
                                return false;
                            }
                            else if(playerArea.HasValue)
                            {
                                noticeArea = PictureContains(table, images["notice"], noticeArea.HasValue? noticeArea: new Rectangle(table.Width / 3, Math.Max(playerArea.Value.Y - table.Height / 3, 0), table.Width / 3, table.Height / 3), .94f);
                                if (noticeArea.HasValue)
                                {
                                    WriteLine("Notice found. Clicking okay...");
                                    Click(noticeArea.Value);
                                    return false;
                                }
                            }
                        }
                    }
                }



                // check last, least likely
                if (!buttonsFound)
                {
                    //WriteLine("No pre-fold / I-am-back buttons found.");
                    Rectangle? newCheckArea = PictureContains(table, images["check"], checkArea.HasValue ? checkArea : buttonsArea, accuracy-.01f - (checkArea.HasValue ? .02f : 0));
                    if (newCheckArea.HasValue)
                    {
                        checkArea = newCheckArea;
                        check = CropImage(table, checkArea.Value);
                        SaveScreenshot(check, "live_check");
                        WriteLine("Check button found.", enableAlerts);
                        hasRaiseButton = hasCallAllInButton = false;
                        hasCheckButton = true;
                        buttonsFound = true;

                        // bet and raise fundamentally the same
                        Rectangle? newBetArea = PictureContains(table, images["bet"], betArea.HasValue ? betArea : buttonsArea, accuracy-.01f - (betArea.HasValue ? .02f : 0));
                        if (newBetArea.HasValue)
                        {
                            betArea = newBetArea;
                            bet = CropImage(table, betArea.Value);
                            SaveScreenshot(bet, "live_bet");
                            WriteLine("Bet button found.", enableAlerts);
                            buttonsFound = true;
                            hasBetButton = true;

                            currentBet = ReadNumber(table, new Rectangle((int)(betArea.Value.X - 40 * scale), betArea.Value.Y + betArea.Value.Height, (int)(betArea.Value.Width + 80 * scale), (int)(betArea.Value.Height * 2)), "betAmt");
                            if (currentBet != -1) WriteLine("Current bet = " + currentBet, enableAlerts);
                            if (currentBet > currentChips)
                            {
                                WriteLine(" - Current bet " + currentBet + " ignored as misread.");
                                RescanBetAmt();
                                if (currentBet != -1) WriteLine("Current bet = " + currentBet, enableAlerts);
                                if (currentBet > currentChips)
                                {
                                    WriteLine(" - Current bet " + currentBet + " ignored as misread.");
                                    currentBet = -1;
                                }
                                if (currentBet < lastActualMinBet)
                                {
                                    WriteLine(" - Current bet " + currentBet + " lower than minbet " + lastActualMinBet + " misread, re-reading...");
                                    RescanBetAmt();
                                    if (currentBet != -1) WriteLine("Current bet = " + currentBet);
                                    if (currentBet < lastActualMinBet)
                                    {
                                        currentBet = lastActualMinBet;
                                        WriteLine("Current bet adjusted to: " + currentBet);
                                    }
                                }
                            }
                            if (currentBet > 3 * Math.Max(lastMinBet, lastActualMinBet) && lastMinBet > 0 && currentBet >= 11000 && currentBet / 3 >= lastMinBet && currentBet.ToString().Reverse().Take(4).Last().ToString() == "1")
                            {

                                currentBet = (int)(currentBet / 10000) * 1000 + (currentBet % 1000);
                                WriteLine("Current bet corrected to: " + currentBet);

                            }
                            if (currentBet != -1) lastActualMinBet = lastMinBet = currentBet;
                        }
                    }
                    else WriteLine("No check / bet buttons found.", enableAlerts);
 
                }

                if (buttonsFound&&!hasBetButton)
                {
                    Rectangle? newRaiseArea = PictureContains(table, images["raise"], raiseArea.HasValue ? raiseArea : buttonsArea, accuracy - (raiseArea.HasValue ? .02f : 0));
                    if (newRaiseArea.HasValue)
                    {
                        raiseArea = newRaiseArea;
                        raise = CropImage(table, raiseArea.Value);
                        SaveScreenshot(raise, "live_raise");
                        WriteLine("Raise button found.", enableAlerts);
                        buttonsFound = true;
                        hasRaiseButton = true;
                        hasCallAllInButton = false;
                        currentRaise = ReadNumber(table, new Rectangle((int)(raiseArea.Value.X - 40 * scale), raiseArea.Value.Y + raiseArea.Value.Height, (int)(raiseArea.Value.Width + 80 * scale), (int)(raiseArea.Value.Height * 2)), "raiseAmt");
                        if (currentRaise != -1) WriteLine("Current raise = " + currentRaise, enableAlerts);
                        if (currentRaise / 10 >= Math.Max(lastMinBet, lastActualMinBet) && currentCall > 0 && currentRaise >= currentCall * 4 && currentRaise >= 11000 && currentRaise / 10 >= lastMinBet && currentRaise.ToString().Reverse().Take(4).Last().ToString() == "1")
                        {
                            WriteLine("Current raise " + currentRaise + " likely misreading of comma.");
                            if (currentCall > 0 && currentCall >= Math.Max(lastMinBet, lastActualMinBet) && currentCall <= currentChips && currentRaise/11<=currentCall*2)
                            {
                                currentRaise = currentCall * 2;
                                WriteLine("Current raise adjusted 2x call to: " + currentRaise);
                            }
                            else
                            {

                                currentRaise = (int)(currentRaise / 10000) * 1000 + (currentRaise % 1000);
                                WriteLine("Current raise corrected to: " + currentRaise);
                            }
                        }
                        if (currentRaise > currentChips || (currentRaise < currentCall && !(currentCall >= 11000 && currentCall>=3*lastMinBet && currentRaise > lastMinBet && currentCall.ToString().Reverse().Take(4).Last().ToString() == "1")))
                        {
                            WriteLine(" - Current raise "+currentRaise+" ignored as misread.");
                            currentRaise = -1;
                        }
                        if (currentRaise == -1 && currentCall > 0)
                        {
                            WriteLine(" - Current raise "+currentRaise+" misread, assumed 2x call " + currentCall + ".");
                            currentRaise = 2 * currentCall;
                        }
                        if (currentRaise>0 && currentRaise < currentChips && (currentCall < currentRaise / 5 || (currentCall < currentChips / 100 && currentRaise > currentChips / 25)))
                        {
                            WriteLine(" - Current call "+currentCall+" misread, assumed half of raise " + currentRaise + ".");
                            currentCall = currentRaise / 2;
                        }
                    }
                    else
                    {
                        WriteLine("No raise button found.", enableAlerts);
                        hasRaiseButton = false;
                    }
                }


                if (buttonsFound && !hasRaiseButton&&!hasBetButton)
                {

                    Rectangle? NewAllinRaiseArea = PictureContains(table, images["allinraise"], allinCallArea.HasValue ? allinCallArea : buttonsArea, .95f - (allinCallArea.HasValue ? .02f : 0));
                    if (NewAllinRaiseArea.HasValue)
                    {
                        allinCallArea = NewAllinRaiseArea;
                        allinCall = CropImage(table, allinCallArea.Value);
                        SaveScreenshot(allinCall, "live_allinraise");
                        WriteLine("All-in raise button found.", enableAlerts);
                        buttonsFound = true;
                        hasCallAllInButton = true;
                        //if (!hasCallButton) currentCall = currentChips;
                    }
                    else
                    {
                        WriteLine("No all-in raise button found.", enableAlerts);
                        hasCallAllInButton = false;
                    }
                }

                if (buttonsFound && (hasRaiseButton||hasBetButton))
                {
                    if (!halfpotArea.HasValue)
                    {
                        halfpotArea = PictureContains(table, images["halfpot"], halfpotArea.HasValue ? halfpotArea : buttonsArea, accuracy);
                        if (halfpotArea.HasValue)
                        {
                            halfpot = CropImage(table, halfpotArea.Value);
                            SaveScreenshot(halfpot, "live_halfpot");
                            WriteLine("Halfpot button found.", enableAlerts);
                            buttonsFound = true;


                            if (hasRaiseButton && !minBetArea.HasValue)
                            {
                                minBetArea = PictureContains(table, images["minbet"], minBetArea.HasValue ? minBetArea : new Rectangle(buttonsArea.X, buttonsArea.Y, halfpotArea.Value.X, buttonsArea.Height), accuracy);
                                if (minBetArea.HasValue)
                                {
                                    minBet = CropImage(table, minBetArea.Value);
                                    SaveScreenshot(minBet, "live_minRaise");
                                    WriteLine("Min raise button found.", enableAlerts);
                                    buttonsFound = true;
                                }
                            }

                            if (hasBetButton && !quarterpotArea.HasValue)
                            {
                                quarterpotArea = PictureContains(table, images["quarterpot"], quarterpotArea.HasValue ? quarterpotArea : new Rectangle(buttonsArea.X, buttonsArea.Y, halfpotArea.Value.X, buttonsArea.Height), accuracy);
                                if (quarterpotArea.HasValue)
                                {
                                    quarterpot = CropImage(table, quarterpotArea.Value);
                                    SaveScreenshot(quarterpot, "live_quarterpot");
                                    WriteLine("Quarterpot button found.", enableAlerts);
                                    buttonsFound = true;
                                }
                            }
                            
                        }
                    }

                    if (!potArea.HasValue)
                    {
                        potArea = PictureContains(table, images["pot"], potArea.HasValue ? potArea : buttonsArea, accuracy);
                        if (potArea.HasValue)
                        {
                            pot = CropImage(table, potArea.Value);
                            SaveScreenshot(pot, "live_pot");
                            WriteLine("Pot button found.", enableAlerts);
                            buttonsFound = true;
                        }
                    }

                    

                    if (!allinArea.HasValue)
                    {
                        allinArea = PictureContains(table, images["allin"], allinArea.HasValue ? allinArea : buttonsArea, accuracy);
                        if (allinArea.HasValue)
                        {
                            allin = CropImage(table, allinArea.Value);
                            SaveScreenshot(allin, "live_allin");
                            WriteLine("All-in button found.", enableAlerts);
                            buttonsFound = true;
                        }
                    }
                }
                

            }
            

            return buttonsFound;
        }

        public static bool RescanButtons()
        {
            if (tableArea.HasValue)
            {
               // WriteLine("Rescanning buttons...");
                RefreshScreen();
                return HasTurnButtons();
            }
            return false;
        }

        public static void RescanRaiseAmt()
        {
            if (raiseArea.HasValue)
            {
                // WriteLine("Rescanning raise amt...");
                RefreshScreen();
                int oldRaise = currentRaise;
                currentRaise = ReadNumber(table, new Rectangle((int)(raiseArea.Value.X - 40 * scale), raiseArea.Value.Y + raiseArea.Value.Height, (int)(raiseArea.Value.Width + 80 * scale), (int)(raiseArea.Value.Height * 2)), "raiseAmt");
                if (currentRaise / 10 > oldRaise && oldRaise > 0 && currentRaise >= 11000 && currentRaise / 10 >= lastMinBet && currentRaise.ToString().Reverse().Take(4).Last().ToString() == "1")
                {
                    WriteLine("Pot raise " + currentRaise + " likely misreading of comma.");
                    
                        currentRaise = (int)(currentRaise / 10000) * 1000 + (currentRaise % 1000);
                        WriteLine("Pot raise corrected to: " + currentRaise);
                    
                }
            }
        }

        public static void RescanBetAmt()
        {
            if (betArea.HasValue)
            {
                // WriteLine("Rescanning bet amt...");
                RefreshScreen();
                currentBet = ReadNumber(table, new Rectangle((int)(betArea.Value.X - 40 * scale), betArea.Value.Y + betArea.Value.Height, (int)(betArea.Value.Width + 80 * scale), (int)(betArea.Value.Height * 2)), "betAmt");
                if (currentBet / 10 > Math.Max(lastActualMinBet, lastMinBet) && currentBet >= 11000 && currentBet / 10 >= lastMinBet && currentBet.ToString().Reverse().Take(4).Last().ToString() == "1")
                {
                    WriteLine("Pot bet " + currentBet + " likely misreading of comma.");

                    currentBet = (int)(currentBet / 10000) * 1000 + (currentBet % 1000);
                    WriteLine("Pot bet corrected to: " + currentBet);

                }
            }
        }
        public static void RescanCallAmt()
        {
            if (callArea.HasValue)
            {
                // WriteLine("Rescanning bet amt...");
                RefreshScreen();
                currentCall = ReadNumber(table, new Rectangle((int)(callArea.Value.X - 40 * scale), callArea.Value.Y + callArea.Value.Height, (int)(callArea.Value.Width + 80 * scale), (int)(callArea.Value.Height * 2)), "callAmt");
            }
        }


        public static Bitmap CropImage(Bitmap image, Rectangle cropArea)
        {
            if (cropArea.X < 0) cropArea.X = 0;
            if (cropArea.Y < 0) cropArea.Y = 0;
            if (cropArea.X + cropArea.Width > image.Width) cropArea.Width = image.Width - cropArea.X;
            if (cropArea.Y + cropArea.Height > image.Height) cropArea.Height = image.Height - cropArea.Y;
            return image.Clone(cropArea, image.PixelFormat);
        }

        public static Bitmap ResizeImage(Bitmap img, double scale)
        {
            if (scale == 1) return img;
            return new AForge.Imaging.Filters.ResizeBicubic((int)(img.Width * scale), (int)(img.Height * scale)).Apply(img);
        }

        public static Rectangle? PictureContains(Bitmap haystack, Bitmap needle, Rectangle? searchArea=null, float accuracy=0.95f)
        {
            //WriteLine("Searching picture...");
            Rectangle? bestMatch = null;

            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(accuracy);

            TemplateMatch[] matchings = (searchArea.HasValue? tm.ProcessImage(haystack, needle, searchArea.Value):tm.ProcessImage(haystack, needle));
            

            BitmapData data = haystack.LockBits(
                 new Rectangle(0, 0, haystack.Width, haystack.Height),
                 ImageLockMode.ReadWrite, haystack.PixelFormat);

            float bestFit = 0;
            foreach (TemplateMatch m in matchings)
            {
                if (m.Similarity > bestFit)
                {
                    bestFit = m.Similarity;
                    bestMatch = m.Rectangle;
                }

            }

            haystack.UnlockBits(data);

            matchScore = bestFit;
            return bestMatch;
        }

        public static void ClickFold()
        {
            expectedChipsDecrease = false;
            if (foldArea.HasValue) Click(foldArea.Value);
            else ClickCheck();
        }

        public static void ClickCheck()
        {
            expectedChipsDecrease = false;
            if (hasCheckButton && checkArea.HasValue) Click(checkArea.Value);
        }

        public static void ClickBet()
        {
            expectedChipsDecrease = true;
            if (hasBetButton && betArea.HasValue) Click(betArea.Value);
            else if (raiseArea.HasValue) ClickRaise();
        }

        public static void ClickCall()
        {
            expectedChipsDecrease = true;
            if (hasCallButton && callArea.HasValue) Click(callArea.Value);
            else ClickCheck();
        }

        public static void ClickRaise()
        {
            expectedChipsDecrease = true;
            if (hasRaiseButton && raiseArea.HasValue) Click(raiseArea.Value);
            else ClickBet();
        }

        public static void ClickCallAllIn()
        {
            expectedChipsDecrease = true;
            if (hasCallAllInButton && allinCallArea.HasValue) Click(allinCallArea.Value);
            else ClickCall();
        }

        public static void Click(Rectangle? area, bool moveAway = true)
        {
            Bitmap expectedButton = CropImage(table, area.Value);
            Rectangle screenArea = new Rectangle(Live.tableArea.Value.X + area.Value.X, Live.tableArea.Value.Y + area.Value.Y, expectedButton.Width, expectedButton.Height);
            Bitmap newestScreen = GetScreen();
            if (!PictureContains(newestScreen, expectedButton, screenArea, .95f).HasValue)
            {
                WriteLine("Oops, buttons changed while I was thinking. Avoid clicking...");
                if (saveScreenshots)
                {
                    SaveScreenshot(expectedButton, "live_expected");
                    SaveScreenshot(CropImage(newestScreen, screenArea), "live_expectedButGot");
                }
                return;
            }

            LiveControl.SetCursorPosition(screenArea.X + new Random().Next(0, area.Value.Width / 2), screenArea.Y + new Random().Next(0, area.Value.Height / 2));
            Thread.Sleep(100 + new Random().Next(0, 200));
            LiveControl.MouseEvent(LiveControl.MouseEventFlags.LeftDown);
            Thread.Sleep(10 + new Random().Next(0, 40));
            LiveControl.MouseEvent(LiveControl.MouseEventFlags.LeftUp);
            if (moveAway)
            {
                Thread.Sleep(100 + new Random().Next(0, 200));
                LiveControl.SetCursorPosition(Live.tableArea.Value.X + new Random().Next(0, (new Random().Next(0, 2) <= 1 ? tableArea.Value.Width /10 : tableArea.Value.Width * 9/10)), Live.tableArea.Value.Y + new Random().Next(Live.tableArea.Value.Height*7/10, Live.tableArea.Value.Height*8/10));
            }
        }

        public static bool WriteLine(string s, bool showOnScreen=true)
        {
            if (showOnScreen) Console.WriteLine(s);
            try
            {
                //File.AppendAllLines("Poker.txt", new string[] { DateTime.Now + ": " + s });
            }
            catch (Exception e) { Console.WriteLine("Log failed: " + e); }
            return true;
        }

    }

    
}
