using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private String MenuName { get; set; }
        private int currentSelection;


        public Menu(String menuName)
        {
            this.MenuName = menuName;
        }

        public void AddMenuItem(MenuItem item)
        {
            MenuItems.Add(item);
        }

        public string RunMenu() // need to be of type Func<string> 
        {
            const int startX = 0;
            const int startY = 0;
            const int optionsPerLine = 1;
            const int spacingPerLine = 2000;

            currentSelection = 0;

            ConsoleKey key;

            Console.CursorVisible = false;

            do
            {
                Console.Clear();

                for (int i = 0; i < MenuItems.Count(); i++)
                {
                    Console.SetCursorPosition(startX + (i % optionsPerLine) * spacingPerLine,
                        startY + i / optionsPerLine);

                    if (i == currentSelection)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(MenuItems.ElementAt(i).ToString());

                    Console.ResetColor();
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    {
                        if (currentSelection >= optionsPerLine)
                            currentSelection -= optionsPerLine;
                        break;
                    }
                    case ConsoleKey.DownArrow:
                    {
                        if (currentSelection + optionsPerLine < MenuItems.Count())
                            currentSelection += optionsPerLine;
                        break;
                    }
                    case ConsoleKey.LeftArrow:
                    {
                        if (MenuItems.ElementAt(currentSelection).MenuType.Equals(MenuItem.MenuTypeEnum.MenuSetting))
                        {
                            MenuItems.ElementAt(currentSelection).DecreaseValue();
                        }

                        break;
                    }
                    case ConsoleKey.RightArrow:
                    {
                        if (MenuItems.ElementAt(currentSelection).MenuType.Equals(MenuItem.MenuTypeEnum.MenuSetting))
                        {
                            MenuItems.ElementAt(currentSelection).IncrementValue();
                        }

                        break;
                    }
                    case ConsoleKey.Enter:
                    {
                        if (MenuItems.ElementAt(currentSelection).MenuType.Equals(MenuItem.MenuTypeEnum.MenuSetting))
                        {
                            // if enter has been pressed on a case where its not applicable
                            // then replace the current key value with another key's value
                            key = ConsoleKey.DownArrow;
                        }

                        break;
                    }
                }
            } while (key != ConsoleKey.Enter);

            Console.CursorVisible = true;

            return MenuItems.ElementAt(currentSelection).MethodToExecute();
        }
    }
}