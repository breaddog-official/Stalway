using UnityEngine;
using System;

namespace Breaddog.SaveManagement
{
    [Serializable]
    public class SaverPlayerPrefs : IStringSaver
    {
        public void Save(string path, string value)
        {
            PlayerPrefs.SetString(ProcessPath(path), value);
            PlayerPrefs.Save();
        }

        public string Load(string path)
        {
            return PlayerPrefs.GetString(ProcessPath(path));
        }

        public bool Exists(string path) => PlayerPrefs.HasKey(ProcessPath(path));
        private string ProcessPath(string path) => path.GetHashCode().ToString();
    }
}
