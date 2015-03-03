using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

class FallingChars
{
    const int PlayerInfoCordinateY = 2;
    public const int MenuXPosition = 80;
    public const char MenuCHar = '*';
    public const string BestPlayersFile = @"..\..\BestPlayers.txt";
    public const string WorstPlayersFile = @"..\..\WorstPlayers.txt";

    public static List<string> BestPlayersInfo = new List<string>();
    public static List<string> WorstPlayersInfo = new List<string>();

    struct GameObject
    {
        public int x;
        public int y;
        public char c;
        public ConsoleColor color;
    }

    static StringBuilder wordToCheck = new StringBuilder();  // the string that the griffin will gather from the falling letters

    static ConsoleColor[] colors = { ConsoleColor.Gray };

    public static void DrawObjectOnPosition(int x, int y, char c, ConsoleColor color)
    {
        Console.SetCursorPosition(x, y);
        //Console.ForegroundColor = color;
        Console.Write(c);
    }

    public static void DrawStringOnPosition(int x, int y, string s, ConsoleColor color)
    {
        Console.SetCursorPosition(x, y);
        //Console.ForegroundColor = color;
        Console.Write(s);
    }
    static Random randomGen = new Random();
    public static string playerName = string.Empty;

    static void Main()
    {
        //begin Title screen + Welcome
        ConsoleHelper.SetConsoleFont(2);
        TitleScreen();
        playerName = GetPlayerName();
        //end Title screen + Welcome

        //<<<<<<< HEAD
        //Console.SetWindowSize(100, 60); //ako dade nqkakva greshka probvaite da mahnete tozi red
        //=======
        //menu tings run
        Console.BufferWidth = Console.WindowWidth = 110;    //clear the right scrollbar
        Console.BufferHeight = Console.WindowHeight = 45;     //clear the down scrollbar
        Console.Title = "HELP THE GRIFFIN TO MAKE WORDS";

        Player newPlayer = new Player();
        PlayerInfo(newPlayer);
        PrintMenu(newPlayer);
        BestPlayersInfo = ReadInfoFromFile(BestPlayersInfo, BestPlayersFile);
        WorstPlayersInfo = ReadInfoFromFile(WorstPlayersInfo, WorstPlayersFile);
        //end
        //>>>>>>> origin/master

        int playfield = 80;                  //Define the playfield so that it wouldn't crash with the menu 
        int score = 0;
        int level = 3;
        char[] latinAlphabet = {'A', 'B','E', 'C','O', 'D', 'U', 'F', 'I','G', 'E','H', 'I', 'J','A', 'K', 'O','L', 'U','M','A',
                              'N', 'O', 'P','E', 'Q', 'I','R', 'U','S', 'A', 'T', 'U', 'V', 'E', 'W', 'O','X', 'U','Y','I','Z'};


        GameObject griffin = InitializeGriffin(playfield);

        List<GameObject> letters = new List<GameObject>();  // this is the current list of 1-to-4-random letter chains to display falling from the top
        // *******       StringBuilder gotString=new StringBuilder(); // the string got by the griffin from the falling letters
        while (true)
        {


            bool detectCollision = false;
            bool getBonus = false;

            GenerateLetterChain(playfield, latinAlphabet, randomGen, letters);

            griffin = MoveGriffin(playfield, griffin, newPlayer);

            List<GameObject> currentLetters = new List<GameObject>();

            MoveLetters(ref score, ref level, letters, currentLetters);

            letters = currentLetters;

            char gotLetter = CheckForCollision(ref griffin, letters, ref detectCollision, ref getBonus);

            Console.Clear();                                            //Clear the console;

            if (detectCollision)                      //on collision assume that the griffin has got a letter which is to be added to the word to be checked
            {
                newPlayer.PlayerWord += gotLetter.ToString();
                wordToCheck.Append(gotLetter);  //append the letter to the word
            }

            DrawObjectOnPosition(griffin.x - 1, griffin.y, 'G', griffin.color);
            DrawObjectOnPosition(griffin.x, griffin.y, griffin.c, griffin.color);
            DrawObjectOnPosition(griffin.x + 1, griffin.y, 'G', griffin.color);

            foreach (GameObject letter in letters)
            {
                DrawObjectOnPosition(letter.x, letter.y, letter.c, letter.color);
            }

            PrintMenu(newPlayer);
            PrintOldStats(BestPlayersInfo, WorstPlayersInfo);
            Thread.Sleep(300);               //Game speed depends on the level - slow the program on higher level
        }
    }

    //menu thinsg start

    static List<string> ReadInfoFromFile(List<string> players, string path)
    {
        var readerBest = new StreamReader(path);
        string line = readerBest.ReadLine();
        using (readerBest)
        {
            int br = 0;
            while (br < 5)
            {
                char[] invalidChars = { ' ' };
                var playerInfo = line.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries);
                players.Add(playerInfo[0]);
                players.Add(playerInfo[1]);
                line = readerBest.ReadLine();
                br++;
            }
        }
        return players;
    }

    class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public DateTime PlayDate { get; set; }

        public string PlayerWord { get; set; }
    }

    static void PlayerInfo(Player player)
    {
        //Console.WriteLine("Enter your name.");
        player.Name = playerName; //Console.ReadLine();
        char[] invalidChar = { ' ' };
        string[] name = player.Name.Split(invalidChar, StringSplitOptions.RemoveEmptyEntries);
        while (name.Length == 0)
        {
            Console.WriteLine("Enter valid name");
            player.Name = Console.ReadLine();
            name = player.Name.Split(invalidChar, StringSplitOptions.RemoveEmptyEntries);
        }

        player.Score = 0;
        player.PlayDate = DateTime.Now;
        Console.WriteLine(player.Name);

    }

    static void PrintMenu(Player player)
    {

        for (int i = 0; i < Console.WindowHeight; i++)
        {
            Console.SetCursorPosition(MenuXPosition, i);
            Console.Write(MenuCHar);
        }
        Console.SetCursorPosition(MenuXPosition + 3, PlayerInfoCordinateY);
        Console.Write("Name: {0}", player.Name);

        Console.SetCursorPosition(MenuXPosition + 3, PlayerInfoCordinateY + 2);
        Console.Write("Score: {0}", player.Score);

        Console.SetCursorPosition(MenuXPosition + 3, PlayerInfoCordinateY + 4);
        Console.Write("Word: {0}", player.PlayerWord);

        for (int i = MenuXPosition; i < Console.WindowWidth; i++)
        {
            Console.SetCursorPosition(i, PlayerInfoCordinateY + 6);
            Console.Write(MenuCHar);
        }
    }

    static void PrintOldStats(List<string> bestPlayers, List<string> worstPlayers)
    {
        const int yCorection = 10;
        int bestY = yCorection;

        Console.SetCursorPosition(MenuXPosition + 6, PlayerInfoCordinateY + bestY - 2);
        Console.Write("Best Scores");

        for (int i = 0, j = 1; i < bestPlayers.Count; i += 2, j += 2)
        {
            Console.SetCursorPosition(MenuXPosition + 6, PlayerInfoCordinateY + bestY);
            Console.Write("{0}: {1}", bestPlayers[i], bestPlayers[j]);
            bestY++;
        }


        int worstY = bestY + 2;
        Console.SetCursorPosition(MenuXPosition + 6, PlayerInfoCordinateY + worstY);
        Console.Write("Worst Scores");
        worstY += 2;

        for (int i = 0, j = 1; i < worstPlayers.Count; i += 2, j += 2)
        {
            Console.SetCursorPosition(MenuXPosition + 6, PlayerInfoCordinateY + worstY);
            Console.Write("{0}: {1}", worstPlayers[i], worstPlayers[j]);
            worstY++;
        }

    }

    static List<string> CheckIsInClasation(List<string> scores, Player player)
    {
        if (player.Score >= 0)
        {
            for (int i = 1; i < scores.Count; i += 2)
            {
                if (player.Score > int.Parse(scores[i]))
                {
                    scores.Insert(i - 1, player.Name);
                    scores.Insert(i, player.Score.ToString());
                    scores.RemoveRange(scores.Count - 2, 2);
                    break;
                }
            }
        }
        else
        {
            for (int i = 1; i < scores.Count; i += 2)
            {
                if (player.Score < int.Parse(scores[i]))
                {
                    scores.Insert(i - 1, player.Name);
                    scores.Insert(i, player.Score.ToString());
                    scores.RemoveRange(scores.Count - 2, 2);
                    break;
                }
            }
        }
        return scores;
    }


    static void WriteToFiles(List<string> scores, Player player, string path)
    {
        var newScore = CheckIsInClasation(scores, player);
        var writer = new StreamWriter(path);

        for (int i = 0, j = 1; i < scores.Count; i += 2, j += 2)
        {
            writer.WriteLine("{0} {1}", scores[i], scores[j]);
        }
        writer.Close();
    }

    //menu tings end



    private static char CheckForCollision(ref GameObject griffin, List<GameObject> letters, ref bool detectCollision, ref bool getBonus)
    {
        foreach (GameObject letter in letters)                          //Check for collision;
        {
            if (((letter.x == griffin.x - 1) ||
                (letter.x == griffin.x) ||
                (letter.x == griffin.x + 1)) && (letter.y == griffin.y))
            {
                if (letter.c == (char)3)
                {
                    getBonus = true;
                }
                else
                {
                    detectCollision = true;
                }
                return letter.c;
            }
        }
        return Char.MinValue;
    }

    private static void MoveLetters(ref int score, ref int level, List<GameObject> letters, List<GameObject> currentLetters)
    {
        for (int i = 0; i < letters.Count; i++)                  //Move the letters;
        {
            GameObject oldLetter = letters[i];
            GameObject currentLetter = new GameObject();
            currentLetter.x = oldLetter.x;
            currentLetter.y = oldLetter.y + 1;
            currentLetter.c = oldLetter.c;
            currentLetter.color = oldLetter.color;
            if (currentLetter.y < Console.WindowHeight)
            {
                currentLetters.Add(currentLetter);
            }
        }
    }

    private static GameObject MoveGriffin(int playfield, GameObject griffin, Player player)
    {
        while (Console.KeyAvailable)            //Move the griffin if arrow key is pressed;
        {
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            if (pressedKey.Key == ConsoleKey.Spacebar)
            {
                if (player.PlayerWord.Length == null)
                {
                    continue;
                }

                if (FindWord(player.PlayerWord.ToLower()))
                {
                    player.Score += player.PlayerWord.Length * 10;
                }
                else
                {
                    player.Score -= player.PlayerWord.Length * 5;
                }

                if (player.Score >= 0)
                {
                    WriteToFiles(BestPlayersInfo, player, BestPlayersFile);
                }
                else
                {
                    WriteToFiles(WorstPlayersInfo, player, WorstPlayersFile);
                }
                player.PlayerWord = string.Empty;

                //Console.WriteLine(wordToCheck);
                //Console.Read();
            }
            else
            {
                if (pressedKey.Key == ConsoleKey.RightArrow)
                    if ((griffin.x + 2) < playfield)
                    {
                        griffin.x++;
                    }
                if (pressedKey.Key == ConsoleKey.LeftArrow)
                    if ((griffin.x - 2) >= 0)
                    {
                        griffin.x--;
                    }
                if (pressedKey.Key == ConsoleKey.Escape)
                {
                    System.Environment.Exit(0);
                }
            }

        }
        return griffin;
    }

    private static GameObject InitializeGriffin(int playfield)
    {
        GameObject griffin = new GameObject();
        griffin.x = playfield / 2 + 1;
        griffin.y = Console.WindowHeight - 1;
        griffin.c = 'G';
        griffin.color = ConsoleColor.Green;
        return griffin;
    }

    // Generate a 1-to-4-characters chain of a random chosen Latin letter, or a bonus heart and add to the letters list 
    private static void GenerateLetterChain(int playfield, char[] latinAlphabet, Random randomGen, List<GameObject> letters)
    {
        int chance = randomGen.Next(0, 100);
        int letterX = randomGen.Next(0, playfield - 1);
        char letterChar;
        if (chance < 3)
        {
            letterChar = (char)3;
        }
        else
        {
            letterChar = latinAlphabet[randomGen.Next(0, latinAlphabet.Length)];
        }
        ConsoleColor letterCol = colors[randomGen.Next(0, colors.Length)];
        for (int i = 1; i <= randomGen.Next(1, 4); i++)
        {
            GameObject newLetter = new GameObject();
            newLetter.y = 0;

            if (newLetter.x >= playfield) newLetter.x = playfield - 1;
            newLetter.c = letterChar;
            if (newLetter.c == (char)3)
            {
                newLetter.color = ConsoleColor.Red;     // heart is a bonus which gives an extra life
                newLetter.x = letterX;
            }
            else
            {
                newLetter.color = letterCol;
                newLetter.x = letterX + i - 1;
            }
            letters.Add(newLetter);
        }
    }

    // scan the dictionary for the "catched" word
    static bool FindWord(string searchedWord)
    {
        //if (searchedWord.Length > 10)
        //{
        //    throw new ArgumentException("The word is too long!");
        //}
        string line;

        try
        {

            using (System.IO.StreamReader vocabularyFile = new System.IO.StreamReader(@"../../wordlist.txt"))
            {
                while ((line = vocabularyFile.ReadLine()) != null)
                {
                    if (line == searchedWord)
                    {
                        // sb.AppendLine(line.ToString());
                        //   Console.WriteLine("Word found! ");
                        // Console.WriteLine(sb.ToString());

                        return true;
                    }
                }
                return false;
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Access Denied,security or I/O error!\n");
            Environment.Exit(0);
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine("Error: (0)\n", ex.Message);
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine("Can't find the path to the file or directory!\n");
            Environment.Exit(0);
        }
        catch (IOException)
        {
            Console.WriteLine("Error reading file!\n");
            Environment.Exit(0);
        }
        return false;
    }

    /*...Zenix code...*/
    //print Griffin title screen
    static void TitleScreen()
    {
        Console.Clear();

        int conHeight = 60;
        if (conHeight > Console.LargestWindowHeight)
            conHeight = Console.LargestWindowHeight;

        if (Console.BufferHeight < conHeight)
            Console.WindowHeight = Console.BufferHeight = conHeight;
        else
            Console.BufferHeight = Console.WindowHeight = conHeight;

        string titleFile = @"..\..\GriffinGraphic.txt";
        string titleText = File.ReadAllText(titleFile, System.Text.Encoding.UTF8);
        string[] titleImage = titleText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string buffer = null;

        Console.Write("\n" + new string(' ', 10));
        for (int i = 0; i < titleImage.Length; i++, Console.Write("\n" + new string(' ', 10)))
        {
            buffer = titleImage[i];
            for (int j = 0; j < buffer.Length; j++)
            {
                Console.ResetColor();
                if (buffer[j] == '#')
                {
                    Console.Write(" ");
                    continue;
                }
                //set the foreground color
                switch (buffer[j])
                {
                    case 'r': Console.ForegroundColor = ConsoleColor.Red; break;
                    case 'g': Console.ForegroundColor = ConsoleColor.Green; break;
                    case 'b': Console.ForegroundColor = ConsoleColor.Blue; break;
                    case 'y': Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case 'c': Console.ForegroundColor = ConsoleColor.Cyan; break;
                    case 'm': Console.ForegroundColor = ConsoleColor.Magenta; break;
                    case 'w': Console.ForegroundColor = ConsoleColor.White; break;

                    case 'R': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                    case 'G': Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                    case 'B': Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                    case 'Y': Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                    case 'C': Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                    case 'M': Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                }
                j++;
                //set the background color
                switch (buffer[j])
                {
                    case 'r': Console.BackgroundColor = ConsoleColor.Red; break;
                    case 'g': Console.BackgroundColor = ConsoleColor.Green; break;
                    case 'b': Console.BackgroundColor = ConsoleColor.Blue; break;
                    case 'y': Console.BackgroundColor = ConsoleColor.Yellow; break;
                    case 'c': Console.BackgroundColor = ConsoleColor.Cyan; break;
                    case 'm': Console.BackgroundColor = ConsoleColor.Magenta; break;
                    case 'w': Console.BackgroundColor = ConsoleColor.White; break;

                    case 'R': Console.BackgroundColor = ConsoleColor.DarkRed; break;
                    case 'G': Console.BackgroundColor = ConsoleColor.DarkGreen; break;
                    case 'B': Console.BackgroundColor = ConsoleColor.DarkBlue; break;
                    case 'Y': Console.BackgroundColor = ConsoleColor.DarkYellow; break;
                    case 'C': Console.BackgroundColor = ConsoleColor.DarkCyan; break;
                    case 'M': Console.BackgroundColor = ConsoleColor.DarkMagenta; break;
                }
                j++;
                //set the symbol and print
                switch (buffer[j])
                {
                    case 'l': Console.Write('░'); break;
                    case 'm': Console.Write('▒'); break;
                    case 'd': Console.Write('▓'); break;
                    case '#': Console.Write(" "); break;
                }
            }
        }
        //print welcome message
        Console.WriteLine();
        string presents = "...Team 'Griffin' presents 'Falling Letters'...";
        Console.CursorLeft = (Console.BufferWidth - presents.Length) / 2;       //reposition the cursor
        Console.WriteLine(presents);
        Console.ReadKey();
    }
    //pop player's name prompt screen
    static string GetPlayerName()
    {
        Console.Clear();

        int conWidth = 80;
        int conHeight = 30;

        if (Console.BufferWidth < conWidth)
            Console.WindowWidth = Console.BufferWidth = conWidth;
        else
            Console.BufferWidth = Console.WindowWidth = conWidth;

        if (Console.BufferHeight < conHeight)
            Console.WindowHeight = Console.BufferHeight = conHeight;
        else
            Console.BufferHeight = Console.WindowHeight = conHeight;

        ConsoleColor conbgcolor = (ConsoleColor)3;
        ConsoleColor confgcolor = (ConsoleColor)4;

        Console.BackgroundColor = conbgcolor;
        Console.ForegroundColor = confgcolor;
        Console.WriteLine(new string(' ', Console.BufferHeight * Console.BufferWidth));

        int promptWindowWidth = 40;
        int promptWindowHeight = 5;

        if (promptWindowWidth > Console.BufferWidth)
            promptWindowWidth = Console.BufferWidth;
        if (promptWindowHeight > Console.BufferHeight)
            promptWindowHeight = Console.BufferHeight;

        ConsoleColor promptbgcolor = ConsoleColor.DarkBlue;
        ConsoleColor promptfgcolor = (ConsoleColor)10;

        Console.BackgroundColor = promptbgcolor;
        Console.ForegroundColor = promptfgcolor;
        Console.CursorLeft = (Console.BufferWidth - promptWindowWidth) / 2;         //center align the prompt
        Console.CursorTop = (Console.BufferHeight - promptWindowHeight) / 2;        //middle align the prompt

        for (int row = 0; row < promptWindowHeight; row++)
        {
            Console.WriteLine(new string(' ', promptWindowWidth));
            Console.CursorLeft = (Console.BufferWidth - promptWindowWidth) / 2;     //reposition the cursor
        }

        string prompt = "Enter your player alias:";
        int promptRows = 2;

        Console.CursorLeft = (Console.BufferWidth - promptWindowWidth) / 2 + (promptWindowWidth - prompt.Length) / 2;
        Console.CursorTop = (Console.BufferHeight - promptWindowHeight) / 2 + (promptWindowHeight - promptRows) / 2;

        Console.WriteLine(prompt);
        Console.CursorLeft = (Console.BufferWidth - promptWindowWidth) / 2 + (promptWindowWidth - prompt.Length) / 2;
        string playerName = Console.ReadLine();

        Console.ResetColor();
        return playerName;
    }
}

public static class ConsoleHelper
{
    private enum StdHandle
    {
        OutputHandle = -11
    }

    [DllImport("kernel32")]
    private static extern IntPtr GetStdHandle(StdHandle index);

    [DllImport("kernel32")]
    private extern static bool SetConsoleFont(IntPtr hOutput, uint index);

    public static bool SetConsoleFont(uint index)
    {
        return SetConsoleFont(GetStdHandle(StdHandle.OutputHandle), index);
    }
}   /*...end Zenix code...*/