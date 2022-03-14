using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace ConsoleFileManager
{
    public class State
    {
        private int _countItemOnPage = 0;
        private string _selectedPath = string.Empty;
        private int _selectedPage = 0;

        public int CountItemOnPage
        {
            get { return _countItemOnPage; }
            set { _countItemOnPage = value; }
        }
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; }
        }
        public int SelectedPage
        {
            get { return _selectedPage; }
            set { _selectedPage = value; }
        }

        public bool SaveState(string path)
        {
            try
            {
                string jsonState = JsonSerializer.Serialize(this);
                File.WriteAllText(path, jsonState);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return false;
            }
        }
        public State LoadState(string path)
        {
            try
            {
                string jsonState = File.ReadAllText(path);
                State loadState = JsonSerializer.Deserialize<State>(jsonState);

                return loadState;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.WriteException(ex);
                return null;
            }
        }
    }
}
