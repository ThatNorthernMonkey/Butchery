using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using StardewValley;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using System.Dynamic;

namespace MonkeySaveGameSerializer
{
    public class MonkeySaveGame
    {
        private string _rootPath;
        private string _fileName;

        private string _fullFileSavePath;
        private string _finalPath;
        private string _finalDirectoryName;

        private List<string> _saveAttrs;

        public MonkeySaveGame(string rootPath, string fileName)
        {
            _rootPath = rootPath;
            _fileName = fileName;
            _saveAttrs = new List<string>();
            
        }

        public void CreateUniqueIdForThisSave(string name, int eyeColour, int hairColour, int hair, string farmName)
        {

            _saveAttrs.Add(eyeColour.ToString());
            _saveAttrs.Add(hairColour.ToString());
            _saveAttrs.Add(hair.ToString());
            _saveAttrs.Add(name);
            _saveAttrs.Add(farmName);

            StringBuilder fileName = new StringBuilder();

            foreach (var l in _saveAttrs)
            {
                fileName.Append(l);
            }

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var temp = rgx.Replace(fileName.ToString(), "");
            temp = temp.Replace(" ", string.Empty);

            _finalDirectoryName = temp.ToLower();

            _fileName += ".json";

            _finalPath = _rootPath + "\\SavedData\\" + _finalDirectoryName + "\\";         
            _fullFileSavePath = _finalPath + _fileName;

            CreateDirectory();

        }

        public void Save(object obj)
        {

            try
            {
                File.WriteAllBytes(_fullFileSavePath, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }

        public dynamic Load<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(File.ReadAllBytes(_fullFileSavePath)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new object();
            }
        }

        private void CreateDirectory()
        {
            if (!Directory.Exists(_finalPath))
            {
                Directory.CreateDirectory(_finalPath);
            }

            if (!File.Exists(Path.Combine(_fullFileSavePath)))
            {
                File.Create(Path.Combine(_fullFileSavePath));
            }
        }

        public string GetPath()
        {
            return _rootPath;
        }
    }
}
