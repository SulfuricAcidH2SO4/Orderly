// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using Orderly.Database.Entities;
using Orderly.EE;
using Orderly.ViewModels.Pages;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Wpf.Ui.Controls;
using TextBlock = System.Windows.Controls.TextBlock;
using TextBox = Wpf.Ui.Controls.TextBox;

namespace Orderly.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        private bool isDragging;
        private bool dragDeadZoneTime; 

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

        private void OnTextSearchChanged(object sender, TextChangedEventArgs e)
        {
            string textQuery = (sender as TextBox).Text;
            ViewModel.SortList(textQuery, false);
        }

        private void CredentialMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragDeadZoneTime = true;
            Task.Factory.StartNew(() => {
                Thread.Sleep(200);
                if (dragDeadZoneTime) {
                    
                    Dispatcher.Invoke(() => {
                        draggedElement.Visibility = Visibility.Visible;
                    });
                    isDragging = true;
                }
            });
        }

        private void CredentialMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragDeadZoneTime = false;
            draggedElement.Visibility = Visibility.Collapsed;
            Task.Factory.StartNew(() => {
                Thread.Sleep(150);
                isDragging = false;
            });
        }

        private void grMainGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging) {
                Point mousePoint = Mouse.GetPosition(canvasDrag);

                draggedElement.SetValue(Canvas.LeftProperty, mousePoint.X);
                draggedElement.SetValue(Canvas.TopProperty, mousePoint.Y);
            }
        }

        private void CategoryMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging) return;
            var lol = draggedElement;
        }

        private void CategoryMouseEnter(object sender, MouseEventArgs e)
        {

        }

        private void CategoryMouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
