using System;
using System.Collections.Generic;
using System.Linq;

namespace GameBrain
{
    public static class Utils
    {
        
        public static Panel At(this List<Panel> panels, int row, int column)
        {
            return panels.First(x => x.Coordinates.Row == row && x.Coordinates.Column == column);
        }

        public static List<Panel> Range(this List<Panel> panels, int startRow, int startColumn, int endRow,
            int endColumn)
        {
            return panels.Where(x => x.Coordinates.Row >= startRow
                                     && x.Coordinates.Column >= startColumn
                                     && x.Coordinates.Row <= endRow
                                     && x.Coordinates.Column <= endColumn).ToList();
        }

        public static T? GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T) attributes[0] : null;
        }
        
        // checks if the placeable ship will be out of bounds
        public static bool WillBeOutOfBounds(int newStartRow, int newStartColumn, int newEndRow, int newEndColumn, Game game)
        {
            if (newStartRow < 0 || newStartRow > game.Height - 1 ||
                newEndRow < 0 || newEndRow > game.Height - 1 ||
                newStartColumn < 0 || newStartColumn > game.Width - 1 ||
                newEndColumn < 0 || newEndColumn > game.Width - 1)
            {
                return true;
            }

            return false;
        }

        // helper method for finding the ends for a ship based on orientation
        public static (int endRow, int endColumn) getEndsBasedOnOrientationAndShip(
            bool orientation, Ship ship,
            int height, int width)
        {
            // if horizontal, then x-coordinate changes
            var endrow = height;
            var endcolumn = width + ship.Width - 1;
            if (orientation) // if vertical, then y-coordinate changes
            {
                endrow = height + ship.Width - 1;
                endcolumn = width;
            }

            return (endrow, endcolumn);
        }
        
        public static void WritePanelMessage(int row, int ownColumn, string letter, Player controllingPlayer)
        {
            if (controllingPlayer.SelectedHeight == row && controllingPlayer.SelectedWidth == ownColumn)
            {
                Console.Write(" ");
                WriteInColour(letter, controllingPlayer.Color);
                Console.Write(" |");
            }
            else
            {
                Console.Write(" " + letter + " |");
            }
        }

        public static void WriteSecondaryEdges(int width, int row, Player controllingPlayer)
        {
            if (controllingPlayer.SelectedHeight == row)
            {
                Console.Write(" ");
                WriteInColour(row, controllingPlayer.Color);
            }
            else
            {
                Console.Write(" " + row);
            }

            Console.WriteLine(Environment.NewLine + "   |" +
                              string.Concat(Enumerable.Repeat("___+", width))
                                  .Substring(0, width * 4));
        }

        public static void WriteFirstEdges(int row, Player controllingPlayer)
        {
            if (controllingPlayer.SelectedHeight == row)
            {
                if (row >= 10)
                {
                    WriteInColour(row, controllingPlayer.Color);
                    Console.Write(" |");
                }
                else
                {
                    Console.Write(" ");
                    WriteInColour(row, controllingPlayer.Color);
                    Console.Write(" |");
                }
            }
            else
            {
                if (row >= 10)
                {
                    Console.Write(row + " |");
                }
                else
                {
                    Console.Write(" " + row + " |");
                }
            }
        }

        public static void WriteLettersForBoard(int width, Player controllingPlayer)
        {
            Console.Write("    ");
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int i = 0; i < width; i++)
            {
                if (controllingPlayer.SelectedWidth == i)
                {
                    Console.Write(" ");
                    WriteInColour(alphabet[i].ToString(), controllingPlayer.Color);
                    Console.Write("  ");
                }
                else
                {
                    Console.Write(" " + alphabet[i] + "  ");
                }
            }

            Console.Write(Environment.NewLine);
        }

        public static void WriteInColour(int message, ConsoleColor color)
        {
            WriteInColour(message.ToString(), color);
        }

        public static void WriteInColour(string message, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }
    }
}