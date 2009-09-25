using System;
using System.Text;
using System.Text.RegularExpressions;

namespace mudclient
{
    static class Color
    {
        #region Fields

        private static Boolean _bold;
        private enum ANSI_Color
        {
            Black,
            Red,
            Green,
            Yellow,
            Blue,
            Cyan,
            Magenta,
            White
        }

        #endregion Fields

        public static void ParseColor(String Message)
        {
            Message = Message.Replace("\r\n\0", Environment.NewLine);
            Regex RE = new Regex(@"(\e\[\d{1,2}m)|([\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Pc}\p{Zs}\p{Pe}\p{Pd}\p{Ps}\p{Cn}\p{Po}]+)|(\s+)|(\n)", RegexOptions.Compiled);
            MatchCollection matches = RE.Matches(Message);

            foreach (Match match in matches)
            {
                if (match.Value[0] == '\u001B')
                    ParseMatch(match.Value);
                else
                    Console.Write(match.Value);
            }
        }

        static void ParseMatch(String Value)
        {
            Value = Value.Substring(2, Value.Length - 2);
            Value = Value.Substring(0, Value.Length - 1);

            foreach (String value in Value.Split(new Char[] { ';' }))
            {
                switch (value)
                {
                    case @"0":
                        ResetColor();
                        break;
                    case @"1":
                        _bold = true;
                        break;
                    //------------------------- Foreground
                    case @"30":
                        SetForeColor(ANSI_Color.Black);
                        break;
                    case @"31":
                        SetForeColor(ANSI_Color.Red);
                        break;
                    case @"32":
                        SetForeColor(ANSI_Color.Green);
                        break;
                    case @"33":
                        SetForeColor(ANSI_Color.Yellow);
                        break;
                    case @"34":
                        SetForeColor(ANSI_Color.Blue);
                        break;
                    case @"35":
                        SetForeColor(ANSI_Color.Magenta);
                        break;
                    case @"36":
                        SetForeColor(ANSI_Color.Cyan);
                        break;
                    case @"37":
                        SetForeColor(ANSI_Color.White);
                        break;
                    //------------------------- Background
                    case @"40":
                        SetBackColor(ANSI_Color.Black);
                        break;
                    case @"41":
                        SetBackColor(ANSI_Color.Red);
                        break;
                    case @"42":
                        SetBackColor(ANSI_Color.Green);
                        break;
                    case @"43":
                        SetBackColor(ANSI_Color.Yellow);
                        break;
                    case @"44":
                        SetBackColor(ANSI_Color.Blue);
                        break;
                    case @"45":
                        SetBackColor(ANSI_Color.Magenta);
                        break;
                    case @"46":
                        SetBackColor(ANSI_Color.Cyan);
                        break;
                    case @"47":
                        SetBackColor(ANSI_Color.White);
                        break;
                }
            }
        }

        #region Console Manipulation

        static void ResetColor()
        {
            Console.ResetColor();
            _bold = false;
        }
        static void SetForeColor(ANSI_Color Color)
        {
            switch (Color)
            {
                case ANSI_Color.Black:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case ANSI_Color.Red:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case ANSI_Color.Green:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case ANSI_Color.Yellow:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case ANSI_Color.Blue:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case ANSI_Color.Cyan:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case ANSI_Color.Magenta:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case ANSI_Color.White:
                    if (_bold)
                        Console.ForegroundColor = ConsoleColor.White;
                    else
                        Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
        }
        static void SetBackColor(ANSI_Color Color)
        {
            switch (Color)
            {
                case ANSI_Color.Black:
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case ANSI_Color.Red:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.Red;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case ANSI_Color.Green:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.Green;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    break;
                case ANSI_Color.Yellow:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.Yellow;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    break;
                case ANSI_Color.Blue:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.Blue;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                    break;
                case ANSI_Color.Cyan:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.Cyan;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkCyan;
                    break;
                case ANSI_Color.Magenta:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    break;
                case ANSI_Color.White:
                    if (_bold)
                        Console.BackgroundColor = ConsoleColor.White;
                    else
                        Console.BackgroundColor = ConsoleColor.Gray;
                    break;
            }
        }

        #endregion Console Manipulation
    }
}
