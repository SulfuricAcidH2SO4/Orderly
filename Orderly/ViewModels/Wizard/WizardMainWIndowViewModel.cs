using Orderly.DaVault;
using Orderly.Helpers;
using Orderly.Interfaces;
using Orderly.Modules;
using Orderly.Views.Windows;
using Orderly.Views.Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Wpf.Ui;

namespace Orderly.ViewModels.Wizard
{
    public partial class WizardMainWIndowViewModel : ViewModelBase
    {
        int currentStep = 0;
        [ObservableProperty]
        private bool showStep1 = true;
        [ObservableProperty]
        private bool showStep2 = false;
        [ObservableProperty]
        private bool showStep3 = false;
        [ObservableProperty]
        private bool showStep4 = false;
        [ObservableProperty]
        private bool showStep5 = false;
        [ObservableProperty]
        private string pass1 = string.Empty;
        [ObservableProperty]
        private string pass2 = string.Empty;
        [ObservableProperty]
        private bool isPasswordError;
        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        ProgramConfiguration config;

        public WizardMainWIndowViewModel(IProgramConfiguration config)
        {
            Config = (ProgramConfiguration?)config!;
        }

        [RelayCommand]
        private void NextPage()
        {
            if(currentStep == 2) {
                if (Pass1 != Pass2) {
                    IsPasswordError = true;
                    ErrorMessage = "Passwords don't match!";
                    return;
                }

                string pattern = @"^(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{6,}$";

                if (!Regex.IsMatch(Pass1, pattern)) {
                    IsPasswordError = true;
                    ErrorMessage = "Passwords did not meet the requirements!";
                    return;
                }

                Config.AbsolutePassword = EncryptionHelper.HashPassword(Pass1);
                ErrorMessage = string.Empty;
            }
            if(currentStep == 4) {
                Config.Save();
                Vault v = App.GetService<Vault>();
                v.PasswordEncryptionKey = EncryptionHelper.HashPassword(Config.AbsolutePassword).Substring(0, 24);
                MainWindow window = (MainWindow)App.GetService<INavigationWindow>();
                window.Show();
                WizardMainWindow wizardWindow = App.GetService<WizardMainWindow>();
                wizardWindow.CLoseWizard();
            }
            currentStep++;
            ShowStep1 = currentStep == 0;
            ShowStep2 = currentStep == 1;
            ShowStep3 = currentStep == 2;
            ShowStep4 = currentStep == 3;
            ShowStep5 = currentStep == 4;
        }
        [RelayCommand]
        private void MoveToPage(string page)
        {
            currentStep = int.Parse(page);
            ShowStep1 = currentStep == 0;
            ShowStep2 = currentStep == 1;
            ShowStep3 = currentStep == 2;
            ShowStep4 = currentStep == 3;
            ShowStep5 = currentStep == 4;
        }
    }
}
