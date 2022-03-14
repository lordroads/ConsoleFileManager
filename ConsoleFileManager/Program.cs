using System;
using System.IO;

namespace ConsoleFileManager
{
    internal class Program
    {
        static int width = Console.LargestWindowWidth / 2 + 1;
        static int height = Console.LargestWindowHeight;

        static bool _run = true;

        static string nameConfigFile = Properties.Resources.nameConfig;
        static State state;

        public static bool Run
        {
            get { return _run; }
            set { _run = value; }
        }

        static void Main(string[] args)
        {
            string pathToStateFile = Path.Combine(Directory.GetCurrentDirectory(), nameConfigFile);

            Init(pathToStateFile);

            if (args.Length > 0)
            {
                Commands.Run(string.Join(' ', args));
            }
            else
            {
                Commands.Run(@$"page {Commands.SelectPage + 1}");
            }

            while (_run)
            {
                Console.Write("Command: ");

                Commands.Run(Console.ReadLine());

                state.SaveState(pathToStateFile);
            }
        }

        /// <summary>
        /// Инициализация сохраненого состояние или создание default состояние если сохнение отсуствует.
        /// </summary>
        /// <param name="pathState">Путь до файла конфигурации.</param>
        static void Init(string pathState)
        {
            Console.SetWindowSize(width, height);

            state = new State();
            state = state.LoadState(pathState);

            if (state == null)
            {
                state = new State();
                state.SelectedPath = Directory.GetCurrentDirectory();
                state.SelectedPage = 0;
                state.CountItemOnPage = 10;
            }

            Commands.Init(state);
        }
    }
}
