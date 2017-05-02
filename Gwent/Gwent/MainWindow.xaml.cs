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
using nDBLoader;
using nGwentCard;

namespace Gwent
{
    
    public partial class MainWindow : Window
    {
        private DBLoader Loader = new DBLoader();
        private Battleground battlegrd;
        public MainMenu Menu;
        public ChouseCard Chouse;
        public Battlefield Battlefield;

        public MainWindow()
        {
            InitializeComponent();
            battlegrd = new Battleground();
            GetMenuControls();
            ChouseCardInit();
            GetBattleFieldCountrols();
            MainGrid.Children[MainGrid.Children.IndexOf(Menu)].Visibility = Visibility.Collapsed;
            MainGrid.Children[MainGrid.Children.IndexOf(Chouse)].Visibility = Visibility.Collapsed;
            MainGrid.Children[MainGrid.Children.IndexOf(Battlefield)].Visibility = Visibility.Collapsed;

        }
        private void GetMenuControls()
        {
            Menu = new MainMenu();
            Menu.MainWindow = this;           
            MainGrid.Children.Add(Menu);                 
        }
        public void GetChouseCardControls(List<GwentCard> Cards, List<string> Fractions)
        {
            Chouse = new ChouseCard();
            Chouse.MainWindow = this;
            Chouse.Fractions = Fractions;
            Chouse.Init(Cards);
            MainGrid.Children.Add(Chouse);
        }
        private void GetBattleFieldCountrols()
        {
            Battlefield = new Battlefield();
            MainGrid.Children.Add(Battlefield);
        }

        private void StartGameInit()
        {
            
        }

        private void ChouseCardInit()
        {
            Loader = new DBLoader();
            battlegrd.AllCards = Loader.LoadCards();            
            GetChouseCardControls(battlegrd.AllCards, Loader.Fractions);
            Loader.Dispose();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainGrid.Children[MainGrid.Children.IndexOf(Menu)].Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }

   
}
