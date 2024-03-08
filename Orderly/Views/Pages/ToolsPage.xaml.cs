﻿using Orderly.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Orderly.Views.Pages
{
    /// <summary>
    /// Interaction logic for ToolsPage.xaml
    /// </summary>
    public partial class ToolsPage : INavigableView<ToolsPageViewModel>
    {
        public ToolsPageViewModel ViewModel { get; set; }

        public ToolsPage(ToolsPageViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }
    }
}
