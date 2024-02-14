using Orderly.Views.RadialMenu;
using SharpHook;

namespace Orderly.Modules
{
    public static class KeyListener
    {
        public static Action? KeyCombinationPressed;

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
                    RadialMenuView menu = new();
                    menu.Show();
                });
            }
        }
    }
}
