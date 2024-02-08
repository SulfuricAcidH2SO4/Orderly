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
    }
}
