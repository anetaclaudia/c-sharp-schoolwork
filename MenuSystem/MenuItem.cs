using System;
using Domain;
using GameBrain;

namespace MenuSystem
{
    public class MenuItem
    {
        public enum MenuTypeEnum
        {
            MenuOption,
            MenuSetting,
            MenuSavedGame
        }

        public enum MenuSubTypeEnum
        {
            MapHeight,
            MapWidth,
            RandomBoard
        }

        public MenuTypeEnum MenuType { get; set; }
        public MenuSubTypeEnum MenuSubType { get; set; }
        public string Label { get; set; }
        public Func<string> MethodToExecute { get; set; } = null!;
        private int MinValue { get; set; }
        private int MaxValue { get; set; }
        private Game? Game { get; set; }
        private SavedGame? MenuSavedGame { get; set; }

        public MenuItem(string label, Func<string> methodToExecute)
        {
            Label = label.Trim();
            MenuType = MenuTypeEnum.MenuOption;
            MethodToExecute = methodToExecute;
        }

        public MenuItem(string label, MenuSubTypeEnum subType, Game game)
        {
            Label = label.Trim();
            MenuType = MenuTypeEnum.MenuSetting;
            MenuSubType = subType;
            Game = game;
            DetermineMinMaxValue();
        }

        private void DetermineMinMaxValue()
        {
            switch (MenuSubType)
            {
                case MenuSubTypeEnum.MapHeight:
                {
                    MinValue = 5;
                    MaxValue = 26;
                    break;
                }
                case MenuSubTypeEnum.MapWidth:
                {
                    MinValue = 5;
                    MaxValue = 26;
                    break;
                }
                case MenuSubTypeEnum.RandomBoard:
                {
                    MinValue = 0;
                    MaxValue = 1;
                    break;
                }
            }
        }

        public void IncrementValue()
        {
            switch (MenuSubType)
            {
                case MenuSubTypeEnum.MapHeight:
                {
                    if (Game!.Height < MaxValue)
                    {
                        Game.Height += 1;
                    }

                    break;
                }
                case MenuSubTypeEnum.MapWidth:
                {
                    if (Game!.Width < MaxValue)
                    {
                        Game.Width += 1;
                    }

                    break;
                }
                case MenuSubTypeEnum.RandomBoard:
                {
                    if (Game!.RandomBoard < MaxValue)
                    {
                        Game.RandomBoard += 1;
                    }

                    break;
                }
            }
        }

        public void DecreaseValue()
        {
            switch (MenuSubType)
            {
                case MenuSubTypeEnum.MapHeight:
                {
                    if (Game!.Height > MinValue)
                    {
                        Game.Height -= 1;
                    }

                    break;
                }
                case MenuSubTypeEnum.MapWidth:
                {
                    if (Game!.Width > MinValue)
                    {
                        Game.Width -= 1;
                    }

                    break;
                }
                case MenuSubTypeEnum.RandomBoard:
                {
                    if (Game!.RandomBoard > MinValue)
                    {
                        Game.RandomBoard -= 1;
                    }

                    break;
                }
            }
        }

        public override string ToString()
        {
            if (MenuType.Equals(MenuTypeEnum.MenuSetting))
            {
                switch (MenuSubType)
                {
                    case MenuSubTypeEnum.MapHeight:
                    {
                        if (Game!.Height == MaxValue)
                        {
                            return Label + " <- " + Game.Height;
                        }

                        if (Game!.Height == MinValue)
                        {
                            return Label + "    " + Game.Height + " ->";
                        }

                        return Label + " <- " + Game.Height + " ->";
                    }
                    case MenuSubTypeEnum.MapWidth:
                    {
                        if (Game!.Width == MaxValue)
                        {
                            return Label + " <- " + Game.Width;
                        }

                        if (Game!.Width == MinValue)
                        {
                            return Label + "    " + Game.Width + " ->";
                        }
                        return Label + " <- " + Game.Width + " ->";
                    }
                    case MenuSubTypeEnum.RandomBoard:
                    {
                        return Label + " = " + Convert.ToBoolean(Game!.RandomBoard);
                    }
                }
            }

            if (MenuType.Equals(MenuTypeEnum.MenuSavedGame))
            {
                return MenuSavedGame!.ToString();
            }

            return Label;
        }
    }
}