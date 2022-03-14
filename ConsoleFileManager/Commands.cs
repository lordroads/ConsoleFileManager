using System;
using System.IO;
using System.Diagnostics;

namespace ConsoleFileManager
{
    public static class Commands
    {
        static string[] _dataDirs;
        static string[] _dataFiles;
        static string[] dataInfo;
        static string _currentPath = string.Empty;
        static string _currentInfoPath = string.Empty;
        static int _selectPage = 0;
        static int _countItemOnPage;
        static int _countPages = 1;
        static State _state;

        public static string CurrentPath
        {
            get { return _currentPath; }
            set { _currentPath = value; }
        }
        public static int CountItemOnPage
        {
            get { return _countItemOnPage; }
            set { _countItemOnPage = value; }
        }
        public static int SelectPage
        {
            get { return _selectPage; }
            set { _selectPage = value; }
        }
        public static int CountPages
        {
            get { return _countPages; }
        }
        /// <summary>
        /// Инициализация сохраненого состояния.
        /// </summary>
        /// <param name="state"></param>
        public static void Init(State state)
        {
            _state = state;

            _currentPath = state.SelectedPath;
            _selectPage = state.SelectedPage;
            _countItemOnPage = state.CountItemOnPage;
        }

        /// <summary>
        /// Обработчик команд от пользователя
        /// </summary>
        /// <param name="userEnter">Получает на вход строку от пользователя, в порядке - первым передается название команды, вторым атрибуты для команды если такие имеются.</param>
        public static void Run(string userEnter)
        {
            string[] command = ParseCommand(userEnter);
            Viewer.ClearConsole();
            try
            {
                switch (command[0].ToLower())
                {
                    case "P":
                    case "page":
                        PageCommand(command[1]);
                        break;
                    case "v":
                    case "view":
                        if (command.Length > 2)
                        {
                            ViewCommand(command[1], command[2] == "r");
                        }
                        else if (command.Length > 1)
                        {
                            ViewCommand(command[1]);
                        }
                        else
                        {
                            ViewCommand();
                        }
                        break;
                    case "fi":
                    case "fileinfo":
                        FileInfoCommand(command[1]);
                        break;
                    case "c":
                    case "copy":
                        CopyCommand(command[1], command[2]);
                        break;
                    case "d":
                    case "delete":
                        DeleteCommand(command[1]);
                        break;
                    case "q":
                    case "quit":
                    case "exit":
                        QuitCommand();
                        break;
                    case "h":
                    case "help":
                        HelpCommand();
                        break;
                    default:
                        DefaultCommand(command[0]);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
            }
            Viewer.View();
        }
        /// <summary>
        /// Разделение string на string[] по указаному разделителю.
        /// </summary>
        /// <param name="commandLine">Строка для разделения содержащая команду и атрибуты к ней.</param>
        /// <param name="separator">Char по которому необходимо разделить строку. Если не указывать это значение, по умолчанию это пробел.</param>
        /// <param name="ignoreSpace">Дополнительный char, с помощью которого происходит игнорирование основного разделителя. Если не указывать это значение, по умолчанию это '"'.</param>
        /// <returns>String[] - первым идет команда, далее атрибуты для нее.</returns>
        private static string[] ParseCommand(string commandLine, char separator = ' ', char ignoreSpace = '"')
        {
            string command = string.Empty;
            string[] args = new string[0];
            bool flag = true;

            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return new[] { "Command is null or white space!" };
            }

            for (int i = 0; i < commandLine.Length; i++)
            {
                if (commandLine[i] == separator & flag)
                {
                    args = Helper.AddItemInArray(args, command);
                    command = string.Empty;
                }
                else
                {
                    if (commandLine[i] == ignoreSpace)
                    {
                        flag = !flag;
                    }
                    else
                    {
                        command += commandLine[i];
                    }
                }
                if (i == commandLine.Length - 1)
                {
                    args = Helper.AddItemInArray(args, command);
                    command = string.Empty;
                }
            }

            return args;
        }

        private static void PageCommand(string numberPage)
        {
            int enterNumberPage = int.Parse(numberPage) - 1;

            if (enterNumberPage >= 0)
            {
                if (CountPages >= enterNumberPage)
                {
                    _selectPage = enterNumberPage;

                    _state.SelectedPage = _selectPage;

                    Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
                    Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, _selectPage));
                    Viewer.AddView(FormatMessage.END_BOX, GetInfo(_currentPath));
                }
                else
                {
                    Viewer.AddView(FormatMessage.MESSAGE, $"Total of \"{CountPages + 1}\" pages");
                }
            }
            else
            {
                Viewer.AddView(FormatMessage.MESSAGE, "The countdown starts from \"1\"");
            }
        }
        private static void ViewCommand(string enterPath = null, bool reload = false)
        {
            if (String.IsNullOrWhiteSpace(enterPath))
            {
                Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
                Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, 0, reload));
                Viewer.AddView(FormatMessage.END_BOX, GetInfo(_currentPath));
            }
            else
            {
                _state.SelectedPage = 0;
                _state.SelectedPath = enterPath;

                Viewer.AddView(FormatMessage.LONG, enterPath, DateTime.Now.ToString());
                Viewer.AddView(FormatMessage.START_BOX, GetPage(enterPath, 0, reload));
                Viewer.AddView(FormatMessage.END_BOX, GetInfo(enterPath));
            }
        }

        private static void FileInfoCommand(string pathFile)
        {
            Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
            Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, _selectPage, true));
            Viewer.AddView(FormatMessage.END_BOX, GetInfo(pathFile));
        }

        private static void CopyCommand(string pathFile, string pathCopyFile)
        {
            bool status = Copy(pathFile, pathCopyFile);

            Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
            Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, _selectPage, true));
            Viewer.AddView(FormatMessage.END_BOX, GetInfo(_currentPath));

            Viewer.AddView(FormatMessage.MESSAGE, status ? "Copy - OK!" : "Copy - BAD!");
        }

        private static void DeleteCommand(string path)
        {
            bool status = Delete(path);

            if (path == CurrentPath)
            {
                _currentPath = _currentPath.Replace(Path.GetFileName(_currentPath), string.Empty).TrimEnd('\\');

                _state.SelectedPage = 0;
                _state.SelectedPath = _currentPath;

                Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
                Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, 0, true));
                Viewer.AddView(FormatMessage.END_BOX, GetInfo(_currentPath));
            }
            else
            {
                Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
                Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, _selectPage, true));
                Viewer.AddView(FormatMessage.END_BOX, GetInfo(_currentPath));
            }

            Viewer.AddView(FormatMessage.MESSAGE, status ? "Delete - OK!" : "Delete - BAD!");
        }

        private static void HelpCommand()
        {
            Viewer.AddView(FormatMessage.DEFAULT, Properties.Resources.HelpText);
        }

        private static void QuitCommand()
        {
            Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
            Viewer.AddView(FormatMessage.LONG, " ", "До свидания!", " ");

            Program.Run = false;
        }

        private static void DefaultCommand(string userEnter)
        {
            Viewer.AddView(FormatMessage.LONG, _currentPath, DateTime.Now.ToString());
            Viewer.AddView(FormatMessage.START_BOX, GetPage(_currentPath, _selectPage));
            Viewer.AddView(FormatMessage.END_BOX, GetInfo(_currentPath));

            Viewer.AddView(FormatMessage.MESSAGE, $"Not Found Command - {userEnter}");
        }
        /// <summary>
        /// Загружает данные по запрашиваемой директории, если данные запрашиваются повторно по этой директории, то загрузка не производится.
        /// </summary>
        /// <param name="enterPath">Путь до директории.</param>
        /// <param name="raload">Флаг для пригудительной загрузки.</param>
        private static void LoadData(string enterPath, bool raload)
        {
            if (Directory.Exists(enterPath))
            {
                if (enterPath != _currentPath | raload | _dataDirs == null | _dataFiles == null)
                {
                    _dataDirs = Directory.GetDirectories(enterPath);
                    _dataFiles = Directory.GetFiles(enterPath);
                    _currentPath = enterPath;
                    _countPages = (_dataDirs.Length + _dataFiles.Length) / _countItemOnPage;
                }
            }
            else if (_dataDirs == null | _dataFiles == null)
            {
                _dataDirs = Directory.GetDirectories(Directory.GetCurrentDirectory());
                _dataFiles = Directory.GetFiles(Directory.GetCurrentDirectory());
                _currentPath = Directory.GetCurrentDirectory();
                _countPages = (_dataDirs.Length + _dataFiles.Length) / _countItemOnPage;
            }
            else
            {
                Viewer.AddView(FormatMessage.MESSAGE, $"Не корректный путь - \"{enterPath}\"");
                Debug.WriteLine($"Нет такого пути - {enterPath}");
                Logger.WriteException(new Exception($"Нет такого пути - {enterPath}"));
            }
        }

        /// <summary>
        /// Показывает файлы и директории в указаном катологе, по странично.
        /// </summary>
        /// <param name="path">Путь запрашиного каталога</param>
        private static string[] GetPage(string path, int page, bool reload = false)
        {
            string[] resultPage = new string[0];
            _selectPage = page;

            int startIndex = _selectPage * _countItemOnPage;
            int lastIndex = startIndex + _countItemOnPage;

            LoadData(path, reload);

            int allItem = _dataDirs.Length + _dataFiles.Length;

            for (int i = startIndex; i < lastIndex; i++)
            {
                if (i >= allItem)
                {
                    break;
                }
                else if (i < _dataDirs.Length)
                {
                    string dir = _dataDirs[i];
                    resultPage = Helper.AddItemInArray(resultPage, $"[]{Path.GetFileName(dir)}");
                }
                else if (i < allItem)
                {
                    string file = _dataFiles[i - _dataDirs.Length];
                    resultPage = Helper.AddItemInArray(resultPage, $"  {Path.GetFileName(file)}");
                }
            }

            resultPage = Helper.AddItemInArray(resultPage, "\n");
            string strPageNumbers = GetStringNumberPage(_selectPage);
            resultPage = Helper.AddItemInArray(resultPage, strPageNumbers);

            return resultPage;
        }
        /// <summary>
        /// Формирует строку с количеством страниц и выбранной страницей.
        /// </summary>
        /// <param name="selectedPage">Выбранная страница</param>
        /// <returns>Сформированная строка.</returns>
        private static string GetStringNumberPage(int selectedPage)
        {
            string result = "Page - ";

            for (int i = 0; i <= _countPages; i++)
            {
                if (i == selectedPage)
                {
                    result += $"[{i + 1}] ";
                }
                else
                {
                    result += $"{i + 1} ";
                }
            }

            return result;
        }
        /// <summary>
        /// Возвращает информацию о файле или директории.
        /// </summary>
        /// <param name="path">Путь к файлу или директории.</param>
        /// <returns>Массив строк с информацией о файле или директории.</returns>
        private static string[] GetInfo(string path)
        {
            if (_currentInfoPath == path & dataInfo != null)
            {
                return dataInfo;
            }
            else
            {
                _currentInfoPath = path;

                DirectoryInfo infoDir;
                FileInfo infoFile;
                try
                {
                    infoDir = new DirectoryInfo(path);
                    infoFile = new FileInfo(path);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Logger.WriteException(ex);
                    return new string[0];
                }

                if (infoDir.Exists)
                {
                    dataInfo = new string[0];

                    dataInfo = Helper.AddItemInArray(dataInfo, $"Directory info:");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Creation Time - {infoDir.CreationTime}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Last Access Time - {infoDir.LastAccessTime}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Last Write Time - {infoDir.LastWriteTime}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Attributes - {infoDir.Attributes}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Files and Dirs - {Helper.GetTotalItem(path)}");

                    long length = Helper.GetTotalLength(path);
                    string stringLength = length == 0 ? "0 Bytes" : length.ToString("#,# Bytes");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Length - {stringLength}");

                    return dataInfo;
                }
                else if (infoFile.Exists)
                {
                    dataInfo = new string[0];

                    dataInfo = Helper.AddItemInArray(dataInfo, $"File info:");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Creation Time - {infoFile.CreationTime}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Last Access Time - {infoFile.LastAccessTime}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Last Write Time - {infoFile.LastWriteTime}");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Attributes - {infoFile.Attributes}");

                    long length = infoFile.Length;
                    string stringLength = length == 0 ? "0 Bytes" : length.ToString("#,# Bytes");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Length - {stringLength}");

                    return dataInfo;
                }
                else
                {
                    dataInfo = new string[0];

                    dataInfo = Helper.AddItemInArray(dataInfo, $"Directory info: NOT FOUND!");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Creation Time - ");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Last Access Time - ");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Last Write Time - ");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Attributes - ");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Files and Dirs - ");
                    dataInfo = Helper.AddItemInArray(dataInfo, $"Length - ");

                    return dataInfo;
                }
            }
        }
        /// <summary>
        /// Копирование файлов и каталогов
        /// </summary>
        /// <param name="filePath">Путь откуда скопировать файл</param>
        /// <param name="newPath">Путь куда спопировать</param>
        /// <returns>При удачном копировании возращает true, в противном случае false.</returns>
        private static bool Copy(string filePath, string newPath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    return CopyFile(fileInfo, newPath);
                }
                else
                {
                    if (Directory.Exists(filePath))
                    {
                        if (Directory.Exists(newPath))
                        {
                            return CopyDir(filePath, newPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(newPath);

                            return CopyDir(filePath, newPath);
                        }
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// Копирование файлов.
        /// </summary>
        /// <param name="fileInfo">Путь к файлу</param>
        /// <param name="newPath">Путь куда копировать</param>
        /// <returns>Возвращает bool, true если все копирование прошло успешно, false если произошла ошибка при копировании или файл отсуствует.</returns>
        private static bool CopyFile(FileInfo fileInfo, string newPath)
        {
            try
            {
                if (fileInfo.Exists)
                {
                    if (Directory.Exists(newPath))
                    {
                        fileInfo.CopyTo(Path.Combine(newPath, fileInfo.Name));

                        return true;
                    }
                    else
                    {
                        Directory.CreateDirectory(newPath);
                        fileInfo.CopyTo(Path.Combine(newPath, fileInfo.Name));

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// Копирование каталогов с файлами.
        /// </summary>
        /// <param name="pathDir">Путь откуда копировать</param>
        /// <param name="newPath">Путь куда копировать</param>
        /// <returns>Возвращает bool, true если все копирование прошло успешно, false если произошла ошибка при копировании.</returns>
        private static bool CopyDir(string pathDir, string newPath)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(pathDir, "*", SearchOption.AllDirectories);
                string[] files = Directory.GetFiles(pathDir, "*.*", SearchOption.AllDirectories);

                for (int i = 0; i < dirs.Length; i++)
                {
                    Directory.CreateDirectory(dirs[i].Replace(pathDir, newPath));
                }
                for (int j = 0; j < files.Length; j++)
                {
                    FileInfo fileInfo = new FileInfo(files[j]);
                    CopyFile(fileInfo, files[j].Replace(pathDir, newPath).Replace(fileInfo.Name, String.Empty));
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// Удаление файлов и катологов
        /// </summary>
        /// <param name="file">Путь к файлу или каталогу</param>
        /// <returns>При удачном удалении возращает true, в противном случае false.</returns>
        private static bool Delete(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Attributes == FileAttributes.Directory)
            {
                return DeleteDir(path);
            }

            return DeleteFile(path);
        }
        /// <summary>
        /// Удаление каталогов с файлами.
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <returns>Возвращает bool, true если все удаление прошло успешно, false если произошла ошибка при удалении.</returns>
        private static bool DeleteDir(string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
                string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(files[i]);
                }
                for (int j = dirs.Length - 1; j >= 0; j--)
                {
                    Directory.Delete(dirs[j]);
                }
                Directory.Delete(path);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return false;
            }
        }
        /// <summary>
        /// Удаление файлов.
        /// </summary>
        /// <param name="pathFile">Путь к файлу.</param>
        /// <returns>Возвращает bool, true если все удаление прошло успешно, false если произошла ошибка при удалении.</returns>
        private static bool DeleteFile(string pathFile)
        {
            try
            {
                if (File.Exists(pathFile))
                {
                    File.Delete(pathFile);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return false;
            }
        }
    }
}
