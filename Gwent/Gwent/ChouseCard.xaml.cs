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
using nGwentCard;

namespace Gwent
{
    
    public partial class ChouseCard : UserControl
    {
        public MainWindow MainWindow { get; set; }
        private List<Image> LeftImages;
        private List<Image> RightImages;
        private int CurrFractionID = 1;
        private List<GwentCard> AllCards;
        private List<GwentCard> UserCards;

        public ChouseCard()
        {
            InitializeComponent();
        }

        public void Init(List<GwentCard> Cards)
        {
            AllCards = Cards;
            LeftImages = InitFractionCards(AllCards);            
            DisplayImages(grdAllCards,LeftImages);
        }

        private void btnToMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.GetMenuControls();
        }

        private void ImageClick(object sender, RoutedEventArgs e)
        {


        }

        private void AddToGrid(Grid MyGrid, Image Image, int Row, int Colum)
        {
            Grid.SetRow(Image, Row);
            Grid.SetColumn(Image, Colum);
            MyGrid.Children.Add(Image);
        }

        private void DisplayImages(Grid Grid, List<Image> Images)
        {
            const int CARDS_ON_ROW = 3;
            int Counter = 0;
            while (Counter < Images.Count)
            {
                RowDefinition row = new RowDefinition();
                Grid.RowDefinitions.Add(row);
                for (int i = 0; i < CARDS_ON_ROW; i++)
                {
                    if (Counter < Images.Count)
                    {
                        AddToGrid(Grid, Images[Counter], (Counter / CARDS_ON_ROW) + 1, i); 
                    }
                    Counter++;
                }
            }
                
        }

        private List<Image> InitFractionCards(List<GwentCard> Cards)
        {
            List<Image> Images = new List<Image>();
            foreach (GwentCard Card in Cards)
            {
                if ((Card.FractionID == CurrFractionID) || (Card.FractionID == 0))
                {
                    Image img = new Image();
                    BitmapImage bti = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + Card.ToImgPath, UriKind.Absolute));
                    img.Stretch = Stretch.Fill;
                    img.Source = bti;
                    img.Tag = Card;
                    Images.Add(img);
                }
            }
            return Images; 
        }

        private void DisplayUserCards(List<GwentCard> Cards, Grid Grid)
        {

        }

        private void btnPrevFraction_Click(object sender, RoutedEventArgs e)
        {
            UserCards.Clear();
            LeftImages.Clear();
            RightImages.Clear();
        }

        private void btnNextFraction_Click(object sender, RoutedEventArgs e)
        {
            UserCards.Clear();
            LeftImages.Clear();
            RightImages.Clear();
        }
    }
}