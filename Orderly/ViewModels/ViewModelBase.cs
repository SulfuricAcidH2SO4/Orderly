using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool isLoading;
        [ObservableProperty]
        public string loadingMessage = string.Empty;

        protected void RunCommand(Action commandAction)
        {
            Task.Factory.StartNew(() => {
                try {
                    IsLoading = true;
                    commandAction.Invoke();
                }
                catch {
                    throw;
                }
                finally { IsLoading = false; }
            });
        }
    }
}
