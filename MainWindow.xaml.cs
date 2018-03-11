using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Win32;

using CsvHelper;

namespace card_overview_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DataTable cardTable;
        private Settings settings;
        private About about;
        private SearchBox searchBox;

        private List<List<CardView>> cards;
        private int cols = 0;
        private int rows = 0;

        private int cardWidth = 100;
        private int cardHeight = 100;

        private string backgroundColor = "#00FF00";
        private Color tbBackgroundColor = Colors.White;
        private Color tbTextColor = Colors.Black;

        private string iconLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\icons\\";

        public MainWindow()
        {
            InitializeComponent();
        }

        public string GetIconLocation()
        {
            return iconLocation;
        }

        public void SetIconLocation(string location)
        {
            iconLocation = location;
        }

        public string GetBackgroundColor()
        {
            return backgroundColor;
        }

        public void SetBackgroundColor(string colorCode)
        {
            colorCode = colorCode.Replace("#", "");
            byte r = byte.Parse(colorCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(colorCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(colorCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            Background = new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        public Color GetTbBackgroundColor()
        {
            return tbBackgroundColor;
        }

        public void SetTbBackgroundColor(Color color)
        {
            tbBackgroundColor = color;
            foreach (List<CardView> column in cards)
            {
                foreach (CardView cv in column)
                {
                    cv.SetTbBackgroundColor(color);
                }
            }
        }

        public Color GetTbTextColor()
        {
            return tbTextColor;
        }

        public void SetTbTextColor(Color color)
        {
            tbTextColor = color;
            foreach (List<CardView> column in cards)
            {
                foreach (CardView cv in column)
                {
                    cv.SetTextColor(color);
                }
            }
        }

        public int GetCardId(string name)
        {
            DataRow result = cardTable.Select("name = '" + name + "'")[0];
            int id = int.Parse(result["id"].ToString());
            return id;
        }

        public string GetCardFilename(int cardid)
        {
            DataRow result = cardTable.Select("id = '" + cardid + "'")[0];
            int id = int.Parse(result["id"].ToString());
            return id.ToString("D3") + ".PNG";
        }

        public void AddCardView(CardView cv)
        {
            mainCanvas.Children.Add(cv);
        }

        public void RemoveCardView(CardView cv)
        {
            mainCanvas.Children.Remove(cv);
        }

        public void ClearAll()
        {
            mainCanvas.Children.Clear();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCardList();
            LoadSettings();

            SetBackgroundColor(backgroundColor);
            Show();

            RedrawCardViewGrid();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void Resize()
        {
            Width = (cols * cardWidth) + 17;
            Height = (rows * cardHeight) + 40;

            if ((cols * cardWidth) + 8 > 265)
            {
                Width = (cols * cardWidth) + 16;
            }
            else
            {
                Width = 265;
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

        private void LoadCardList()
        {
            //Setup the table
            cardTable = new DataTable();

            cardTable.Columns.Add("id", typeof(int));
            cardTable.Columns.Add("name", typeof(string));
            cardTable.Columns.Add("cardtype", typeof(string));
            cardTable.Columns.Add("type", typeof(string));
            cardTable.CaseSensitive = false;

            string filename = "fm-cards.csv";
            CsvReader csv = new CsvReader(File.OpenText(filename));
            csv.Configuration.WillThrowOnMissingField = false;

            while (csv.Read())
            {
                DataRow row = cardTable.NewRow();
                foreach (DataColumn column in cardTable.Columns)
                {
                    row[column.ColumnName] = csv.GetField(column.DataType, column.ColumnName);
                }
                cardTable.Rows.Add(row);
            }
        }

        private bool LoadFromFile(string filename)
        {
            string[] lines;
            if ((lines = File.ReadAllLines(filename)) != null)
            {
                //Clear everything
                ClearAll();

                cards = new List<List<CardView>>();

                cols = int.Parse(lines[0]);
                rows = int.Parse(lines[1]);

                for (int i = 0; i < cols; i++)
                {
                    List<CardView> colC = new List<CardView>();

                    for (int j = 0; j < rows; j++)
                    {
                        string card = lines[(rows * i) + j + 2];
                        CardView cv = new CardView(this);
                        cv.SetTbBackgroundColor(tbBackgroundColor);
                        cv.SetTextColor(tbTextColor);
                        cv.SetImage(int.Parse(card.Split(' ')[0]));
                        cv.SetVisibility(bool.Parse(card.Split(' ')[1]));
                        colC.Add(cv);

                        Canvas.SetLeft(cv, cardWidth * i);
                        Canvas.SetTop(cv, cardHeight * j);
                        AddCardView(cv);
                    }
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

        private void RedrawCardViewGrid()
        {
            Resize();

            cards = new List<List<CardView>>();

            for (int i = 0; i < cols; i++)
            {
                List<CardView> colC = new List<CardView>();

                for (int j = 0; j < rows; j++)
                {
                    CardView cv = new CardView(this);
                    cv.SetTbBackgroundColor(tbBackgroundColor);
                    cv.SetTextColor(tbTextColor);
                    colC.Add(cv);

                    Canvas.SetLeft(cv, cardWidth * i);
                    Canvas.SetTop(cv, cardHeight * j);
                    AddCardView(cv);
                }
                cards.Add(colC);
            }
        }

        public void SaveSettings()
        {
            string filename = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\settings.dat";
            Stream stream;
            try
            {
                if (!File.Exists(filename))
                {
                    stream = File.Open(filename, FileMode.Create);
                    if (stream != null)
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write(iconLocation);
                            writer.Write(Array.IndexOf(Settings.colors, tbBackgroundColor));
                            writer.Write(Array.IndexOf(Settings.colors, tbTextColor));
                            writer.Write(backgroundColor);
                        }
                    }
                }
                else
                {
                    stream = File.Open(filename, FileMode.Open);
                    if (stream != null)
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write(iconLocation);
                            writer.Write(Array.IndexOf(Settings.colors, tbBackgroundColor));
                            writer.Write(Array.IndexOf(Settings.colors, tbTextColor));
                            writer.Write(backgroundColor);
                        }
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public void LoadSettings()
        {
            string filename = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\settings.dat";
            if (File.Exists(filename))
            {
                Stream stream;
                if ((stream = File.Open(filename, FileMode.Open, FileAccess.Read)) != null)
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        iconLocation = reader.ReadString();
                        tbBackgroundColor = Settings.colors[reader.ReadInt32()];
                        tbTextColor = Settings.colors[reader.ReadInt32()];
                        backgroundColor = reader.ReadString();
                    }
                }
            }
            else
            {
                SaveSettings();
            }
        }

        public void ShowSearchBox(CardView cv)
        {
            bool found = false;
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is SearchBox)
                    found = true;
            }

            if (!found)
            {
                searchBox = new SearchBox(this, cv);
                searchBox.Show();
            }
        }

        public DataRow[] SearchCardList(string text)
        {
            DataRow[] result = new DataRow[0];

            int a;
            if (int.TryParse(text, out a))
            {
                DataRow[] result1 = cardTable.Select("id = " + a);
                result = result.Union(result1).ToArray();
            }

            DataRow[] result2 = cardTable.Select("name LIKE '*" + text + "*'");
            result = result.Union(result2).ToArray();

            DataRow[] result3 = cardTable.Select("cardtype LIKE '*" + text + "*'");
            result = result.Union(result3).ToArray();

            DataRow[] result4 = cardTable.Select("type LIKE '*" + text + "*'");
            result = result.Union(result4).ToArray();

            return result;
        }

        private void Open_Layout(object sender, RoutedEventArgs e) //Open
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Card Overview Files (*.cof)|*.cof";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName != "")
                {
                    LoadFromFile(openFileDialog.FileName);
                    modifyMenu.IsChecked = false;

                }
            }
        }

        private void Save_Layout(object sender, RoutedEventArgs e) //Save
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Card Overview Files (*.cof)|*.cof";
            saveFileDialog.RestoreDirectory = true;

            Stream stream;

            if (saveFileDialog.ShowDialog() == true)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(cols);
                        writer.WriteLine(rows);
                        for (int i = 0; i < cols; i++)
                        {
                            for (int j = 0; j < rows; j++)
                            {
                                writer.Write(cards[i][j].GetCardId() + " ");
                                writer.WriteLine((bool)(cards[i][j].GetVisibility()));
                            }
                        }
                    }
                }
            }
        }

        private void Add_Column(object sender, RoutedEventArgs e)
        {
            cols++;
            if (rows == 0)
            {
                rows++;
            }
            RedrawCardViewGrid();
        }

        private void Remove_Column(object sender, RoutedEventArgs e)
        {
            if(cols > 1)
            {
                cols--;
                RedrawCardViewGrid();
            }
        }

        private void Add_Row(object sender, RoutedEventArgs e)
        {
            rows++;
            if(cols == 0)
            {
                cols++;
            }
            RedrawCardViewGrid();
        }

        private void Remove_Row(object sender, RoutedEventArgs e)
        {
            if(rows > 1)
            {
                rows--;
                RedrawCardViewGrid();
            }
        }

        private void Modify_Layout_Checked(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < cols; i++)
            {
                for(int j = 0; j < rows; j++)
                {
                    cards[i][j].ToggleContextMenu(true);
                }
            }
        }

        private void Modify_Layout_Unchecked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    cards[i][j].ToggleContextMenu(false);
                }
            }
        }

        private void Show_Settings_Menu(object sender, RoutedEventArgs e) //Settings
        {
            bool found = false;
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is Settings)
                    found = true;
            }

            if (!found)
            {
                settings = new Settings();
                settings.Show(this);
            }
        }

        private void Show_About_Box(object sender, RoutedEventArgs e) //About
        {
            bool found = false;
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is About)
                    found = true;
            }

            if (!found)
            {
                about = new About();
                about.Show();
            }
        }
    }
}
