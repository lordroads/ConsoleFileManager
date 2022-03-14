using System;
using System.Diagnostics;
using System.IO;

namespace ConsoleFileManager
{
    public static class Helper
    {
        /// <summary>
        /// Добавляет элемент в массив.
        /// </summary>
        /// <param name="array">Массив в котором необходимо сделать разширение.</param>
        /// <param name="item">Элемент который необходимо добавить.</param>
        /// <returns>Возвращает значение типа string[]</returns>
        public static string[] AddItemInArray(string[] array, string item)
        {
            string[] copy = array;
            string[] result = new string[copy.Length + 1];

            for (int i = 0; i < copy.Length; i++)
            {
                result[i] = copy[i];
            }
            result[copy.Length] = item;

            return result;
        }
        /// <summary>
        /// Возвращает общее количество байт в запрашиваемой директории.
        /// </summary>
        /// <param name="path">Путь к директории.</param>
        /// <returns>Возвращает значение типа long</returns>
        public static long GetTotalLength(string path)
        {
            long totalLength = 0;

            try
            {
                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = new FileInfo(files[i]);
                    if (fileInfo.Exists)
                    {
                        totalLength += fileInfo.Length;
                    }
                }
                for (int i = 0; i < dirs.Length; i++)
                {
                    totalLength += GetTotalLength(dirs[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
            }

            return totalLength;
        }
        /// <summary>
        /// Возвращает общее количество файлов и директорий по запрашиваемому пути.
        /// </summary>
        /// <param name="path">>Путь к директории.</param>
        /// <returns>Возращает кортеж типа (long, long), количество файлов и директорий.</returns>
        public static (long, long) GetTotalItem(string path)
        {
            long countFiles = 0;
            long countDirs = 0;

            try
            {
                string[] files = Directory.GetFiles(path);
                string[] dirs = Directory.GetDirectories(path);
                countFiles += files.Length;
                countDirs += dirs.Length;

                for (int i = 0; i < dirs.Length; i++)
                {
                    (long, long) counter = GetTotalItem(dirs[i]);
                    countFiles += counter.Item1;
                    countDirs += counter.Item2;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
            }

            return (countFiles, countDirs);
        }
    }
}
