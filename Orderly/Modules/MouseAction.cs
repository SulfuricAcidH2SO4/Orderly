using System.Runtime.InteropServices;

namespace Orderly.Modules
{
    public static class MouseAction
    {
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        public static void ClickAtPosition(double x, double y, int durationLength = 10)
        {
            SetCursorPos((int)x, (int)y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, (int)x, (int)y, 0, 0);
            Thread.Sleep(durationLength);
            mouse_event(MOUSEEVENTF_LEFTUP, (int)x, (int)y, 0, 0);
        }
    }
}
