using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.ViewModels
{
    public class ViewModelBase : ObservableObject
    {
        public bool IsLoading { get; set; } = false;
        public string LoadingMessage = string.Empty;
    }
}
