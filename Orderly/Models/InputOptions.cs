using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Orderly.Models
{
    public class InputOptions : INotifyPropertyChanged
    {
        private bool useAlt = true;
        private bool useCtrl = true;
        private bool useShift = false;   
        private Keys keyCode = Keys.P;

        public bool UseAlt
        {
            get => useAlt;
            set
            {
                SetProperty(ref useAlt, value);
            }
        }
        public bool UseCtrl
        {
            get => useCtrl;
            set
            {
                SetProperty(ref useCtrl, value);
            }
        }
        public bool UseShift
        {
            get => useShift;
            set
            {
                SetProperty(ref useShift, value);   
            }
        }
        public Keys KeyCode
        {
            get => keyCode;
            set
            {
                SetProperty(ref keyCode, value);
            }
        }

        public override string ToString()
        {
            string sb = string.Empty;
            sb += UseCtrl ? "Ctrl + " : string.Empty;
            sb += UseAlt ? "Alt + " : string.Empty;
            sb += UseShift ? "Shift + " : string.Empty;
            sb += KeyCode;
            return sb;
        }

        #region Property Changed
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void SetProperty<T>(ref T destination, T value, [CallerMemberName] string propertyName = null!)
        {
            destination = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
