using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
 
class FallingRocks
{
    struct GameObject
    {
        public int x;
        public int y;
        public char c;
        public ConsoleColor color;
    }

    static StringBuilder wordToCheck = new StringBuilder();  // the string that the griffin will gather from the falling letters

    static ConsoleColor[] colors = { ConsoleColor.Gray, ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Yellow, ConsoleColor.Yellow };
 
    public static void DrawObjectOnPosition (int x, int y, char c, ConsoleColor color)
    {
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = color;
        Console.Write(c);
    }
 
    public static void DrawStringOnPosition(int x, int y, string s, ConsoleColor color)
    {
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = color;
        Console.Write(s);
    }
 
    static void Main()                      
    {
        Console.BufferWidth = Console.WindowWidth=130;    //clear the right scrollbar
        Console.BufferHeight = Console.WindowHeight=30;     //clear the down scrollbar
        Console.Title = "HELP THE GRIFFIN TO MAKE WORDS";
        int playfield = 80;                  //Define the playfield so that it wouldn't crash with the menu 
        int score = 0;
        int level = 0;
        char[] latinAlphabet = {'A', 'B','E', 'C','O', 'D', 'U', 'F', 'I','G', 'E','H', 'I', 'J','A', 'K', 'O','L', 'U','M','A',
                              'N', 'O', 'P','E', 'Q', 'I','R', 'U','S', 'A', 'T', 'U', 'V', 'E', 'W', 'O','X', 'U','Y','I','Z'};


        Random randomGen = new Random();

        GameObject griffin = InitializeGriffin(playfield);

        List<GameObject> letters = new List<GameObject>();  // this is the current list of 1-to-4-random letter chains to display falling from the top
 // *******       StringBuilder gotString=new StringBuilder(); // the string got by the griffin from the falling letters
        while (true)
        {


            bool detectCollision = false;
            bool getBonus = false;

            GenerateLetterChain(playfield, latinAlphabet, randomGen, letters);

            griffin = MoveGriffin(playfield, griffin);

            List<GameObject> currentLetters = new List<GameObject>();

            MoveLetters(ref score, ref level, letters, currentLetters);

            letters = currentLetters;

            char gotLetter=CheckForCollision(ref griffin, letters, ref detectCollision, ref getBonus);
            
            Console.Clear();                                            //Clear the console;

            if (detectCollision)                      //on collision assume that the griffin has got a letter which is to be added to the word to be checked
            {
                wordToCheck.Append(gotLetter);  //append the letter to the word
            }

            DrawObjectOnPosition(griffin.x - 1, griffin.y, 'G', griffin.color);
            DrawObjectOnPosition(griffin.x, griffin.y, griffin.c, griffin.color);
            DrawObjectOnPosition(griffin.x + 1, griffin.y, 'G', griffin.color);

            foreach (GameObject letter in letters)
            {
                DrawObjectOnPosition(letter.x, letter.y, letter.c, letter.color);
            }

            Thread.Sleep(300 - level*100);               //Game speed depends on the level - slow the program on higher level
        }
    }

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
            else
            {
                score++;
                if (level < 5) level = score / 50;
            }
        }
    }

    private static GameObject MoveGriffin(int playfield, GameObject griffin)
    {
        while (Console.KeyAvailable)            //Move the griffin if arrow key is pressed;
        {
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);
            if (pressedKey.Key==ConsoleKey.Spacebar)
            {
                Console.WriteLine(wordToCheck);
                Console.Read();
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
            if (searchedWord.Length > 10)
            {
                throw new ArgumentException("The word is too long!");
            }
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
        
//      private static void CheckWordInVocabulary()
//      {
//          Console.WriteLine(FindWord("spider") ? "Word exists" : "Word does not exist");
//          Console.WriteLine(FindWord("sdfsdf") ? "Word exists" : "Word does not exist");
//
//          Console.Read();
//      }
}