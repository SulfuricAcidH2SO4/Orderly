﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Orderly.Database.Entities;
using Orderly.EE;
using Orderly.ViewModels.Pages;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;

namespace Orderly.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void Popup_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (sender is not Popup pop || pop.DataContext is not Category) return;
            ViewModel.UpdateCategory((Category)pop.DataContext);
        }

        private void OnSadFaceLoaded(object sender, RoutedEventArgs e)
        {
            if(sender is TextBlock tb) {
                tb.Text = EEManager.GetRandomAngryFace();
            }
        }
    }
}
