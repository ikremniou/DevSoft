﻿using System;
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

namespace Gwent
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        public MainWindow MainWindow { get; set; }
        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Close();
        }

        private void btnChouseCard_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainGrid.Children[MainWindow.MainGrid.Children.IndexOf(MainWindow.Menu)].Visibility = Visibility.Collapsed;
            MainWindow.MainGrid.Children[MainWindow.MainGrid.Children.IndexOf(MainWindow.Chouse)].Visibility = Visibility.Visible;
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MainGrid.Children[MainWindow.MainGrid.Children.IndexOf(MainWindow.Menu)].Visibility = Visibility.Collapsed;
            MainWindow.MainGrid.Children[MainWindow.MainGrid.Children.IndexOf(MainWindow.Battlefield)].Visibility = Visibility.Visible;
        }
    }
}
