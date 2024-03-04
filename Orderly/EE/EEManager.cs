using Newtonsoft.Json;
using System.IO;

namespace Orderly.EE
{
    public static class EEManager
    {
        public static string GetRandomPhrase()
        {
            string[] phrases = {
                "Now with extra sauce!",
                "Written (almost) without ChatGPT",
                "Don't know what to put here...",
                "Also try Minecraft",
                "Also try Terraria",
                "I swear it's not Origin",
                "Touching grass since 2001"
            };

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
