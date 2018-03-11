using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace card_overview_wpf
{
    /// <summary>
    /// Interaction logic for CardView.xaml
    /// </summary>
    public partial class CardView : System.Windows.Controls.UserControl
    {
        private int cardId;
        private MainWindow mainWindow;

        public CardView(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        public void Increment()
        {
            if(!modifyMenu.IsEnabled)
            {
                textBoxCount.Text = (int.Parse(textBoxCount.Text) + 1).ToString();
            }
        }

        public void Decrement()
        {
            if (int.Parse(textBoxCount.Text) > 0 && !modifyMenu.IsEnabled)
            {
                textBoxCount.Text = (int.Parse(textBoxCount.Text) - 1).ToString();
            }
        }

        public void SetTbBackgroundColor(Color color)
        {
            textBoxCount.Background = new SolidColorBrush(color);
        }

        public void SetTextColor(Color color)
        {
            textBoxCount.Foreground = new SolidColorBrush(color);
        }

        public int GetCardId()
        {
            return cardId;
        }

        public bool GetVisibility()
        {
            return textBoxCount.Visibility == Visibility.Visible;
        }

        private void OnClick_Increment(object sender, MouseButtonEventArgs e)
        {
            Increment();
        }

        private void OnClick_Decrement(object sender, MouseButtonEventArgs e)
        {
            Decrement();
        }

        private void Toggle_Visibility(object sender, RoutedEventArgs e) //Toggle visibility
        {
            if (imageCard.Visibility == Visibility.Visible)
            {
                textBoxCount.Visibility = Visibility.Hidden;
                imageCard.Visibility = Visibility.Hidden;
            }
            else
            {
                textBoxCount.Visibility = Visibility.Visible;
                imageCard.Visibility = Visibility.Visible;
            }
        }

        public void SetVisibility(bool vis)
        {
            if (!vis)
            {
                textBoxCount.Visibility = Visibility.Hidden;
                imageCard.Visibility = Visibility.Hidden;
            }
            else
            {
                textBoxCount.Visibility = Visibility.Visible;
                imageCard.Visibility = Visibility.Visible;
            }
        }

        public void SetImage(int id)
        {
            try
            {
                if (id > 0)
                {
                    string s = mainWindow.GetIconLocation() + mainWindow.GetCardFilename(id);
                    imageCard.Source = new BitmapImage(new Uri(@s));
                    cardId = id;
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Image not found. Please configure the icon location in settings.");
            }
        }

        public void ToggleContextMenu(bool state)
        {
            modifyMenu.IsEnabled = state;
            if(state)
            {
                modifyMenu.Visibility = Visibility.Visible;
            }
            else
            {
                modifyMenu.Visibility = Visibility.Hidden;
            }
        }

        private void Select_Image(object sender, RoutedEventArgs e) //Select Image
        {
            mainWindow.ShowSearchBox(this);
        }
    }
}