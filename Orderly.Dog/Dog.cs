using System.Diagnostics;

namespace Orderly.Dog
{
    public class Dog
    {
        static void Main(string[] args)
        {
            if(args.Length == 0) {
                Console.WriteLine("Dude, you can't run this");
                return;
            }
            switch (args[0]) {
                case "restore":
                    string original = args[1];
                    string backup = args[2];
                    bool restart = bool.Parse(args[3]);
                    File.Move(original, $"{original}.old");
                    File.Move(backup, original);
                    StartOrderly();
                    break;
            }
        }

        private static void StartOrderly()
        {
            try {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = "Orderly.exe",
                    UseShellExecute = false,
                };

                Process.Start(startInfo);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error starting the process: {ex.Message}");
            }
        }
    }
}
