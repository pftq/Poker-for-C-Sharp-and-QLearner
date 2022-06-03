If you don't want to download anything and just want to try playing, you can play it online through the browser at https://www.pftq.com/qpoker/

For those downloading to desktop...

Files to run or load are in RELEASE EXE and DLL.  They are also downloadable here in case it's confusing on Github:
https://drive.google.com/drive/folders/1_Q69jIYWln9TLse8-JcQ3clRI6qKAPC8?usp=sharing

Launch the Poker.exe to play as a standalone app for human practice against bots.  You can toggle the "Play Blind" button to practice playing without looking at your cards.

Load the Poker.dll into QLearner as a plugin to let the AI play and learn over time.  The source code is structured in a way you can likely pull it into your own machine learning projects to train algos on.  The GUI can be disabled so it runs in console only using the variables at the top of the Poker.cs file; if the GUI is disabled, it'll also use cache for winning odds calculations, which improves performance 2x.  A precomputed cache for about 3-million hand configurations can be downloaded here: https://drive.google.com/drive/folders/1_z4v5hlkhirGICPQw_u8t5qDTa1SzGtA?usp=sharing

This is also the same code I used to simulate a million games for the stats on winning hands here:
https://www.pftq.com/blabberbox/?page=Frequency_of_Winning_Hands_in_Poker

Lastly, you can run it against live poker games by toggling the useLiveData variable at the top of Poker.cs, which will make it constantly monitor and play poker games on screen.  To avoid getting into trouble, my own code for this is not available here - the PlayerAI class for the AI and the Live.class for reading the game screen are empty.  You must implement and code these yourself with your own screenshots of cards, table, username etc in the images folder though (make sure they are all 24-bit PNG).  The current image set is optimized for 1920x1200 resolution, so your screen should be the same settings for best results.  The table should also be placed in the bottom right of the screen to make it easier for the image recognition.  Make sure to disable DPI settings by right-clicking the exe and choosing to override DPI changes by application.

October 2021
by pftq
https://www.pftq.com/qlearner/

Additional Credits:
- Keith Rule for C# Fast, Texas Hand Evaluator library
- Google protobuf for caching.
- Aforge for image recognition libraries.
- Tesseract2 for text recognition libraries.
- radbyx for mouse click code at https://stackoverflow.com/questions/2416748/how-do-you-simulate-mouse-click-in-c