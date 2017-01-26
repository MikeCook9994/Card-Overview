﻿using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace card_overview_wpf
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : Window
    {
        private CardWindow window;
        private CardView card;

        public SearchBox(CardWindow maindWindow, CardView c)
        {
            InitializeComponent();
            window = maindWindow;
            card = c;
        }

        private async void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string start = tb.Text;

            await Task.Delay(200);
            if (start.Equals(tb.Text))
                Search();
        }

        private void Search()
        {
            if (window != null)
            {
                listBoxResults.Items.Clear();
                DataRow[] result = window.SearchCardList(textBoxSearch.Text);
                foreach (DataRow row in result)
                {
                    listBoxResults.Items.Add(row["name"]);
                }
            }
        }

        private void buttonSet_Click(object sender, RoutedEventArgs e)
        {
            if(card != null)
            {
                card.SetImage(window.GetCardId(listBoxResults.SelectedItem.ToString()));
                Close();
            }
        }

        public void Start(CardView cv)
        {
            card = cv;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
