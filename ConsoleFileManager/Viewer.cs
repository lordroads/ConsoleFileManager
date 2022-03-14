using System;
using System.Text;

namespace ConsoleFileManager
{
    public static class Viewer
    {
        static char[] UI = new char[] { '\u250C', '\u2510', '\u2514', '\u2518', '\u251C', '\u2524', '\u2500', '\u252C', '\u2534', '\u2502' }; // 0┌ 1┐ 2└ 3┘ 4├ 5┤ 6─ 7┬ 8┴ 9│
        static int width = Console.LargestWindowWidth / 2;
        static StringBuilder viewConsole = new StringBuilder();
        static StringBuilder backup = new StringBuilder();
        static string[] _tempMessage = new string[0];

        public static void ClearConsole()
        {
            Console.Clear();
            viewConsole = new StringBuilder();
        }
        /// <summary>
        /// Добавление информации для последующего вывода на Консоль.
        /// </summary>
        /// <param name="format">Формат вывода информации.</param>
        /// <param name="message">Информация которую необходимо вывести на консоль.</param>
        public static void AddView(FormatMessage format, params string[] message)
        {
            switch (format)
            {
                case FormatMessage.LONG:
                    Long(message);
                    break;
                case FormatMessage.START_BOX:
                    StartBox(message);
                    break;
                case FormatMessage.END_BOX:
                    EndBox(message);
                    break;
                case FormatMessage.MESSAGE:
                    Message(message);
                    break;
                case FormatMessage.DEFAULT:
                default:
                    Default(message);
                    break;
            }
        }
        /// <summary>
        /// Команда вывода укомплектованной информации на Консоль.
        /// </summary>
        public static void View()
        {
            if (viewConsole == null)
            {
                viewConsole = new StringBuilder();
            }

            Console.WriteLine(viewConsole);

            if (_tempMessage.Length > 0)
            {
                for (int i = 0; i < _tempMessage.Length; i++)
                {
                    viewConsole.Replace(_tempMessage[i] + "\n", string.Empty);
                }
            }

            backup = viewConsole;
        }

        private static void Long(params string[] message)
        {
            int space = width;
            StringBuilder blockMessage = new StringBuilder();

            for (int i = 0; i < message.Length; i++)
            {
                if (space < 0)
                {
                    space = 1;
                    break;
                }
                space -= message[i].Length;
            }

            space = space / (message.Length - 1);

            blockMessage.Append(String.Join(new String(' ', space), message));
            blockMessage.Append('\n');

            viewConsole.Append(blockMessage.ToString());
        }
        private static void Message(params string[] message)
        {
            viewConsole = backup;
            _tempMessage = message;

            for (int i = 0; i < message.Length; i++)
            {
                viewConsole.Append(message[i]);
                viewConsole.Append('\n');
            }
        }
        private static void Default(params string[] message)
        {
            StringBuilder blockMessage = new StringBuilder();

            for (int i = 0; i < message.Length; i++)
            {
                blockMessage.AppendLine(message[i]);
            }

            viewConsole.Append(blockMessage.ToString());
        }
        private static void StartBox(params string[] message)
        {
            StringBuilder blockMessage = new StringBuilder();

            int needMoreRow = Commands.CountItemOnPage - (message.Length - 2);

            blockMessage.Append(UI[0]);
            blockMessage.Append(new String(UI[6], width - 2));
            blockMessage.Append(UI[1]);
            blockMessage.Append('\n');

            for (int i = 0; i < message.Length;)
            {
                blockMessage.Append(UI[9]);

                blockMessage.Append(new String(' ', 2));

                if (message.Length - 2 < Commands.CountItemOnPage)
                {
                    if (i < message.Length - 2)
                    {
                        if (message[i].Length > width)
                        {
                            blockMessage.Append(message[i].Substring(0, width - 4));
                        }
                        else
                        {
                            blockMessage.Append(message[i]);
                            blockMessage.Append(new String(' ', width - message[i].Length - 4));
                        }
                        i++;
                    }
                    else
                    {
                        if (needMoreRow > 0)
                        {
                            blockMessage.Append(new String(' ', width - 4));
                            needMoreRow--;
                        }
                        else
                        {
                            if (message[i] == "\n")
                            {
                                blockMessage.Append(new String(' ', width - 4));
                                i++;
                            }
                            else
                            {
                                if (message[i].Length > width)
                                {
                                    blockMessage.Append(message[i].Substring(0, width - 4));
                                }
                                else
                                {
                                    blockMessage.Append(message[i]);
                                    blockMessage.Append(new String(' ', width - message[i].Length - 4));
                                }
                                i++;
                            }
                        }
                    }
                }
                else
                {
                    if (message[i] == "\n")
                    {
                        blockMessage.Append(new String(' ', width - 4));
                        i++;
                    }
                    else
                    {
                        if (message[i].Length > width)
                        {
                            blockMessage.Append(message[i].Substring(0, width - 4));
                        }
                        else
                        {
                            blockMessage.Append(message[i]);
                            blockMessage.Append(new String(' ', width - message[i].Length - 4));
                        }
                        i++;
                    }
                }

                blockMessage.Append(UI[9]);
                blockMessage.Append('\n');
            }

            blockMessage.Append(UI[4]);
            blockMessage.Append(new String(UI[6], width - 2));
            blockMessage.Append(UI[5]);

            blockMessage.Append('\n');

            viewConsole.Append(blockMessage.ToString());
        }
        private static void EndBox(params string[] message)
        {
            StringBuilder blockMessage = new StringBuilder();

            //Header info
            blockMessage.Append(UI[9]);

            int spaceCount = (width - 2 - message[0].Length) / 2;
            if (spaceCount % 2 == 0)
            {
                string header = string.Concat(new String(' ', spaceCount), message[0], new String(' ', spaceCount));
                blockMessage.Append(header);
            }
            else
            {
                string header = string.Concat(new String(' ', spaceCount), message[0], new String(' ', spaceCount + 1));
                blockMessage.Append(header);
            }
            blockMessage.Append(UI[9]);
            blockMessage.Append('\n');

            //cicle content
            for (int i = 1; i <= message.Length / 2; i++)
            {
                blockMessage.Append(UI[9]);

                string first = string.Empty;
                string second = string.Empty;

                if (i + 3 > message.Length - 1)
                {
                    first = message[i];
                    second = string.Empty;
                }
                else
                {
                    first = message[i];
                    second = message[i + 3];
                }

                int space = width - 2 - 4 - first.Length - second.Length;
                string content = string.Concat(new String(' ', 2), first, new String(' ', space), second, new String(' ', 2));
                blockMessage.Append(content);

                blockMessage.Append(UI[9]);
                blockMessage.Append('\n');
            }

            //finish line
            blockMessage.Append(UI[2]);
            blockMessage.Append(new String(UI[6], width - 2));
            blockMessage.Append(UI[3]);

            blockMessage.Append('\n');

            viewConsole.Append(blockMessage.ToString());
        }
    }
}
