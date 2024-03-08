using Orderly.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Orderly.ViewModels.Pages.Tools
{
    public partial class PasswordGeneratorToolViewModel : ViewModelBase
    {
        [ObservableProperty]
        int length;
        [ObservableProperty]
        string generatedPassword = string.Empty;
        [ObservableProperty]
        bool useNumbers;
        [ObservableProperty]
        bool useSymbols;
        [ObservableProperty]
        bool useUppercase;
        [ObservableProperty]
        string strengthRate;
        [ObservableProperty]
        int strength;

        public PasswordGeneratorToolViewModel()
        {
            PropertyChanged += OnPropertyChanged;
        }

        [RelayCommand]
        public void CopyPassword()
        {
            Clipboard.SetText(GeneratedPassword);
            App.GetService<ISnackbarService>().Show("Password copied!", 
                "The generated password was sucessfully copied in your clipboard",
                Wpf.Ui.Controls.ControlAppearance.Primary, 
                new SymbolIcon(SymbolRegular.Checkmark12), 
                TimeSpan.FromSeconds(3));
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GeneratedPassword)
             || e.PropertyName == nameof(Strength)
             || e.PropertyName == nameof(StrengthRate)) return;

            GeneratedPassword = PasswordGenerator.GenerateSecurePassword(Length, UseUppercase, UseNumbers, UseSymbols);
            int tmpStrength = PasswordStrengthChecker.CalculatePasswordStrength(GeneratedPassword);

            if (tmpStrength == 0) {
                Strength = 1;
                StrengthRate = "Weak";
            }
            else if (tmpStrength == 1) {
                Strength = 10;
                StrengthRate = "Kinda secure";
            }
            else {
                Strength = 20;
                StrengthRate = "Bulletproof";
            }
        }
    }
}
