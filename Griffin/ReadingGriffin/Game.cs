﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ReadingGriffin
{
    class Game
    {
        static void Main()
        {
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;
            int position = 0;
            int position2 = 0;
            bool reflect = false;
            bool reflect2 = false;
            while (true)
            {
                Console.SetCursorPosition(2, position);
                Console.Write(
                @"****
  ****
  ****");
                Console.SetCursorPosition(position2, 10);
                Console.Write(
                @"****");
                Console.SetCursorPosition(position2, 11);
                Console.Write(
                @"****");
                Console.SetCursorPosition(position2, 12);
                Console.Write(
                @"****");
                Thread.Sleep(100);
                Console.Clear();
                if (position == Console.WindowHeight - 3)
                {
                    reflect = true;
                }
                if (reflect == false)
                {
                    position++;
                }
                else
                {
                    position--;
                }
                if (position == Console.WindowHeight - Console.WindowHeight)
                {
                    reflect = false;
                }
                if (position2 == Console.WindowWidth - 4)
                {
                    reflect2 = true;
                }
                if (reflect2 == false)
                {
                    position2 += 2;
                }
                else
                {
                    position2 -= 2;
                }
                if (position2 == Console.WindowWidth - Console.WindowWidth)
                {
                    reflect2 = false;
                }
            }
        }
    }
}