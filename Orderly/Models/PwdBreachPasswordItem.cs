using Orderly.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orderly.Models
{
    public partial class PwdBreachPasswordItem : ObservableObject
    {
        [ObservableProperty]
        Credential? credential;

        [ObservableProperty]
        CredentialBreachStatus status;

        [ObservableProperty]
        bool isVisible = true;

        [ObservableProperty]
        bool isPasswordVisibile = false;
    }
}
