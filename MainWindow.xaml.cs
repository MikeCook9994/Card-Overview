using CsvHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
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

namespace card_overview_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Windows

        private List<List<ButtonControl>> buttons;
        private List<List<CardView>> cards;
        private int cols = 1;
        private int rows = 7;

        //Settings
        private int cardWidth = 100;
        private int cardHeight = 100;

        private string backgroundColor = "#00FF00";
        private Color tbBackgroundColor = Colors.White;
        private Color tbTextColor = Colors.Black;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Resize()
        {
            cardWindow.Width = (cols * cardWidth) + 17;
            cardWindow.Height = (rows * cardHeight) + 40;

            if ((cols * cardWidth) + 8 > 300)
            {
                Width = (cols * cardWidth) + 8;
            }
            else
            {
                Width = 300;
            }

            if ((rows * cardHeight) + 60 > 300)
            {
                Height = (rows * cardHeight) + 60;
            }
            else
            {
                Height = 300;
            }
        }



        private bool LoadFromFile(string filename)
        {
            string[] lines;
            if ((lines = System.IO.File.ReadAllLines(filename)) != null)
            {
                //Clear everything
                cardWindow.ClearAll();
                ClearAll();

                cards = new List<List<CardView>>();
                buttons = new List<List<ButtonControl>>();

                cols = int.Parse(lines[0]);
                rows = int.Parse(lines[1]);

                for (int i = 0; i < cols; i++)
                {
                    List<ButtonControl> colB = new List<ButtonControl>();
                    List<CardView> colC = new List<CardView>();

                    for (int j = 0; j < rows; j++)
                    {
                        string card = lines[(rows * i) + j + 2];
                        CardView cv = new CardView();
                        cv.SetTbBackgroundColor(tbBackgroundColor);
                        cv.SetTextColor(tbTextColor);
                        cv.SetImage(int.Parse(card.Split(' ')[0]));
                        cv.SetVisibility(bool.Parse(card.Split(' ')[1]));
                        ButtonControl bc = new ButtonControl(cv);
                        colB.Add(bc);
                        colC.Add(cv);

                        Canvas.SetLeft(bc, 100 * i);
                        Canvas.SetTop(bc, cardHeight * j);
                        mainCanvas.Children.Add(bc);

                        Canvas.SetLeft(cv, cardWidth * i);
                        Canvas.SetTop(cv, cardHeight * j);
                        cardWindow.AddCardView(cv);
                    }

                    buttons.Add(colB);
                    cards.Add(colC);
                }

                Resize();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ClearAll()
        {
            mainCanvas.Children.Clear();
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cardWindow != null)
            {
                cardWindow.Close();
            }
            if (searchBox != null)
            {
                searchBox.Close();
            }
            if (settings != null)
            {
                settings.Close();
            }
            if (about != null)
            {
                about.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }



        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCardList();
            LoadSettings();
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is CardWindow)
                    cardWindow = window as CardWindow;
            }

            cardWindow.SetBackgroundColor(backgroundColor);
            cardWindow.Show();

            Resize();

            buttons = new List<List<ButtonControl>>();
            cards = new List<List<CardView>>();

            for (int i = 0; i < cols; i++)
            {
                List<ButtonControl> colB = new List<ButtonControl>();
                List<CardView> colC = new List<CardView>();

                for (int j = 0; j < rows; j++)
                {
                    CardView cv = new CardView();
                    cv.SetTbBackgroundColor(tbBackgroundColor);
                    cv.SetTextColor(tbTextColor);
                    ButtonControl bc = new ButtonControl(cv);
                    colB.Add(bc);
                    colC.Add(cv);

                    Canvas.SetLeft(bc, 100 * i);
                    Canvas.SetTop(bc, cardHeight * j);
                    mainCanvas.Children.Add(bc);

                    Canvas.SetLeft(cv, cardWidth * i);
                    Canvas.SetTop(cv, cardHeight * j);
                    cardWindow.AddCardView(cv);
                }

                buttons.Add(colB);
                cards.Add(colC);
            }
        }
    }
}
