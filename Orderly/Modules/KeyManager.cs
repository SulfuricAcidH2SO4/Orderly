using Orderly.Views.RadialMenu;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using Orderly.Interfaces;
using System.Runtime.InteropServices;

namespace Orderly.Modules
{
    public static class KeyManager
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        public static IKeyboardMouseEvents? Hook { get; private set; }
        public static bool PauseMenuListener { get; set; } = false;
        public static bool PauseClickListener { get; set; } = false;    

        public static Point lastOpenedPoint;
        private static ProgramConfiguration? config;
        private static InputTerminalView menu;

        public static void InitializeHook()
        {
            Hook = Gma.System.MouseKeyHook.Hook.GlobalEvents();
            Hook.KeyDown += OnKeyDown;
            Hook.MouseDown += OnMouseDown;
            config = (ProgramConfiguration)App.GetService<IProgramConfiguration>();
            menu = App.GetService<InputTerminalView>();
            App.Current.Exit += OnAppExit;
        }

        private static void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (!PauseClickListener && !menu.IsVisible) {
                lastOpenedPoint = new Point(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        public static void SendTextAt(string text, Point point)
        {
            if (Control.IsKeyLocked(Keys.CapsLock)) {
                const int KEYEVENTF_EXTENDEDKEY = 0x1;
                const int KEYEVENTF_KEYUP = 0x2;
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY, (nint)(UIntPtr)0);
                keybd_event(0x14, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                (nint)(UIntPtr)0);
            }
            MouseAction.ClickAtPosition(point.X, point.Y);
            SendKeys.SendWait(text);
            SendKeys.Flush();
        }

        private static void OnAppExit(object sender, ExitEventArgs e)
        {
            Hook!.KeyDown -= OnKeyDown;
            Hook.Dispose();
        }

        private static void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (PauseMenuListener) return;
            bool correctInput = e.KeyCode == config!.InputOptions.KeyCode
                                && e.Control == config.InputOptions.UseCtrl
                                && e.Alt == config.InputOptions.UseAlt
                                && e.Shift == config.InputOptions.UseShift;

            if (correctInput) {
                e.SuppressKeyPress = true;
                if (menu.IsVisible) {
                    menu.CloseMenu();
                    return;
                } 
                double xPos = Cursor.Position.X;
                double yPos = Cursor.Position.Y;
                yPos = yPos < 0 ? 0 : yPos;
                yPos = yPos + menu.Height > SystemParameters.PrimaryScreenHeight ? SystemParameters.PrimaryScreenHeight - menu.Height : yPos;
                menu.Top = yPos;
                menu.Left = xPos;
                menu.OpenMenu();
            }
        }

    }
}
