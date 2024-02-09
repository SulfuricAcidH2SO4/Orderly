using Newtonsoft.Json;
using Orderly.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Interfaces
{
    public interface IProgramConfiguration
    {
        string AbsolutePassword { get; set; }
        bool IsDarkMode {  get; set; }
        bool ShowMinimizeNotification { get; set; }
        bool StartOnStartUp {  get; set; }
        bool StartMinimized { get; set; }
        bool CloseButtonClosesApp { get; set; }  

        void Save();
        static ProgramConfiguration Load()
        {
            if (!File.Exists("CoreConfig.ordcf"))
            {
                ProgramConfiguration config = new();
                config.Save();
            }
            return JsonConvert.DeserializeObject<ProgramConfiguration>(File.ReadAllText("CoreConfig.ordcf"))!;
        }
    }
}
