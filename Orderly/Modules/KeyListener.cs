using SharpHook;

namespace Orderly.Modules
{
    public static class KeyListener
    {
        public static Action? KeyCombinationPressed;

        [STAThread]
        public static void InitializeHook()
        {
            var hook = new TaskPoolGlobalHook();
            hook.KeyPressed += OnKeyPressed;
            hook.Run();
        }

        private static void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
        {
            
        }
    }
}
