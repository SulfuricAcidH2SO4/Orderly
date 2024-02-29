using Newtonsoft.Json;
using System.IO;

namespace Orderly.EE
{
    public static class EEManager
    {
        public static string GetRandomPhrase()
        {
            string[] phrases = JsonConvert.DeserializeObject<string[]>(File.ReadAllText("EE\\FP.ordf"))!;

            return phrases[new Random().Next(phrases.Length - 1)];
        }

        private static Random rnd = new();
        private static string[] Faces = {
                "(╥﹏╥)",
                "(＞﹏＜)",
                "(＃￣ω￣)",
                "(ノ°益°)ノ",
                "(｡╯︵╰｡)",
                "＼(º □ º l|l)/",
                "(｡T ω T｡)",
                "ヽ(￣～￣　)ノ",
                "╮(￣ω￣;)╭",
                "(ಡ‸ಡ)"
            };
        private static string face;

        public static string Face
        {
            get => Faces[rnd.Next(Faces.Length - 1)];
            set
            {
                face = value;
            }
        }
    }
}
