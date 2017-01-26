using System.Windows;
using System.Windows.Media;

namespace card_overview_wpf
{
    /// <summary>
    /// Interaction logic for CardWindow.xaml
    /// </summary>
    public partial class CardWindow : Window
    {
        public CardWindow()
        {
            InitializeComponent();
        }

        public void AddCardView(CardView cv)
        {
            mainCanvas.Children.Add(cv);
        }

        public void RemoveCardView(CardView cv)
        {
            mainCanvas.Children.Remove(cv);
        }

        public void SetBackgroundColor(string colorCode)
        {
            colorCode = colorCode.Replace("#", "");
            byte r = byte.Parse(colorCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(colorCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(colorCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            Background = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public void ClearAll()
        {
            mainCanvas.Children.Clear();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
