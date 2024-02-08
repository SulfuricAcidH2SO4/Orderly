using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Orderly.Modules
{
    public static class KeyListener
    {
        public static Action? KeyCombinationPressed;

        private static IKeyboardMouseEvents? hook;

        public static void InitializeHook()
        {
            hook = Hook.GlobalEvents();
            hook.KeyDown += OnKeyDown;
        }

        private static void OnKeyDown(object? sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.P && e.Control && e.Alt) {
                GC.Collect();
            }
        }
    }
}
