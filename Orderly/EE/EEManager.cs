using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.EE
{
    internal class EEManager
    {
        public static string GetRandomPhrase()
        {
            string[] phrases = JsonConvert.DeserializeObject<string[]>(File.ReadAllText("EE\\FP.ordf"))!;

            return phrases[new Random().Next(phrases.Length - 1)];
        }

        public static string GetRandomAngryFace()
        {
            string[] faces = {
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

            return faces[new Random().Next(faces.Length - 1)];
        }
    }
}
