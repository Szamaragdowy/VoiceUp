using Newtonsoft.Json;
using System.IO;

namespace VoiceUP.Structures
{
    static class JsonLoader
    {
        public static  T LoadJson<T>(string path)
        {
            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            else
            {
                File.Create(path).Close();
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            
        }
    }
}
