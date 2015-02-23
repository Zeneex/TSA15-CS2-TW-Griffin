using System;
using System.Text;

namespace Words
{
    class WordFinder
    {
        static void Main()
        {
            string searchedWord = string.Empty;
            Console.WriteLine("Please type a word: ");
            searchedWord = Console.ReadLine();

            if (FindWord(searchedWord) == true)
            {
                Console.WriteLine("Word found!");
            }
            else
            {
                Console.WriteLine("Word not found!");
            }
/*
            string result=FindWord("dgfhjkll;kjhgf")?"Word exists":"Word does not exist";
            Console.WriteLine(result);
 */
        }


		
        static bool FindWord(string searchedWord)
        {
            string line;


            using (System.IO.StreamReader file = new System.IO.StreamReader(@"../../wordlist.txt"))
            {
                while ((line = file.ReadLine()) != null)
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
    }
}
