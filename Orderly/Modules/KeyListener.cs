using Orderly.Views.RadialMenu;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace Orderly.Modules
{
    public static class KeyListener
    {
        private static IKeyboardMouseEvents? hook;

        public static void InitializeHook()
        {
            hook = Hook.GlobalEvents();
            hook.KeyDown += OnKeyDown;
            App.Current.Exit += OnAppExit;
        }

        private static void OnAppExit(object sender, ExitEventArgs e)
        {
            hook!.KeyDown -= OnKeyDown;
        }

        private static void OnKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.P && e.Control && e.Alt) {
                RadialMenuView menu = App.GetService<RadialMenuView>();
                if (menu.IsVisible) return;
                double xPos = Cursor.Position.X - (menu.Width / 2);
                double yPos = Cursor.Position.Y - (menu.Height / 2);
                yPos = yPos < 0 ? 0 : yPos;
                menu.Top = yPos;
                menu.Left = xPos;
                menu.OpenMenu();
            }
        }
    }
}
