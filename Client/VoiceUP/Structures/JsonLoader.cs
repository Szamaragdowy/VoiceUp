using Newtonsoft.Json;
using System.IO;

namespace VoiceUP.Structures
{
    static class JsonLoader
    {
        public static  T LoadJson<T>(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
