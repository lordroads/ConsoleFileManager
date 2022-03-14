using System;
using System.IO;

namespace ConsoleFileManager
{
    public static class Logger
    {
        /// <summary>
        /// Запись ошибок в файлы.
        /// </summary>
        /// <param name="ex">Принимает Exception</param>
        public static void WriteException(Exception ex)
        {
            try
            {
                string pathDirLog = Path.Combine(Directory.GetCurrentDirectory(), "errors");
                if (Directory.Exists(pathDirLog))
                {
                    string nameException = DateTime.Now.ToString("dd.MM.yy HH-mm-ss");
                    string pathFile = Path.Combine(pathDirLog, nameException + ".txt");

                    File.AppendAllText(pathFile, ex.ToString());
                }
                else
                {
                    Directory.CreateDirectory(pathDirLog);
                    WriteException(ex);
                }
            }
            catch (Exception criticalEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(criticalEx.ToString());
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
