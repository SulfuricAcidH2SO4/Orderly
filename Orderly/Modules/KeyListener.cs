using Orderly.Views.RadialMenu;
using SharpHook;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace Orderly.Modules
{
    public static class KeyListener
    {
        [STAThread]
        public static void InitializeHook()
        {
            Task.Factory.StartNew(() =>
            {
                var hook = new TaskPoolGlobalHook();
                hook.KeyPressed += OnKeyPressed;
                hook.Run();
            });
        }

        private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            if((e.RawEvent.Mask & (SharpHook.Native.ModifierMask.LeftAlt | SharpHook.Native.ModifierMask.LeftCtrl)) == (SharpHook.Native.ModifierMask.LeftAlt | SharpHook.Native.ModifierMask.LeftCtrl) && e.Data.KeyCode == SharpHook.Native.KeyCode.VcP)
            {
                
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RadialMenuView menu = App.GetService<RadialMenuView>();
                    double xPos = System.Windows.Forms.Cursor.Position.X - (menu.Width / 2);
                    double yPos = System.Windows.Forms.Cursor.Position.Y - (menu.Height / 2);
                    yPos = yPos < 0 ? 0 : yPos;
                    menu.Top = yPos;
                    menu.Left = xPos;
                    menu.OpenMenu();
                });
            }
        }
    }
}
