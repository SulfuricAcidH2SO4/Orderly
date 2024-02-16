using Orderly.Views.RadialMenu;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;
using Orderly.Interfaces;

namespace Orderly.Modules
{
    public static class KeyListener
    {
        public static IKeyboardMouseEvents? Hook { get; private set; }
        public static bool PauseMenuListener { get; set; } = false;

        private static ProgramConfiguration config;

        public static void InitializeHook()
        {
            Hook = Gma.System.MouseKeyHook.Hook.GlobalEvents();
            Hook.KeyDown += OnKeyDown;
            config = (ProgramConfiguration)App.GetService<IProgramConfiguration>();
            App.Current.Exit += OnAppExit;
        }

        private static void OnAppExit(object sender, ExitEventArgs e)
        {
            Hook!.KeyDown -= OnKeyDown;
        }

        private static void OnKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (PauseMenuListener) return;
            bool correctInput = e.KeyCode == config.InputOptions.KeyCode
                                && e.Control == config.InputOptions.UseCtrl
                                && e.Alt == config.InputOptions.UseAlt
                                && e.Shift == config.InputOptions.UseShift;

            if (correctInput) {
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
