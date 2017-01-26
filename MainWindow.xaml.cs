﻿using CsvHelper;
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
        private DataTable cardTable;
        private Settings settings;
        private About about;
        private CardWindow cardWindow;
        private SearchBox searchBox;

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

        private string iconLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\icons\\";

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
                        CardView cv = new CardView(this);
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
                    CardView cv = new CardView(this);
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


        private void MenuItem_Click(object sender, RoutedEventArgs e) //Open
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Card Overview Files (*.cof)|*.cof";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName != "")
                {
                    LoadFromFile(openFileDialog.FileName);
                }
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) //Save
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

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) //Add Column
        {
            cols++;
            Resize();

            List<ButtonControl> colB = new List<ButtonControl>();
            List<CardView> colC = new List<CardView>();

            int i = cols - 1;
            for (int j = 0; j < rows; j++)
            {
                CardView cv = new CardView(this);
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

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) //Remove Column
        {
            //Remove column from both canvases
            int i = cols - 1;
            for (int j = 0; j < rows; j++)
            {
                mainCanvas.Children.Remove(buttons[i][j]);
                cardWindow.RemoveCardView(cards[i][j]);
            }

            //Remove column from both lists
            buttons.RemoveAt(cols - 1);
            cards.RemoveAt(cols - 1);

            //Resize
            cols--;
            Resize();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) // Add Row
        {
            rows++;
            Resize();

            int j = rows - 1;
            for (int i = 0; i < cols; i++)
            {
                CardView cv = new CardView(this);
                cv.SetTbBackgroundColor(tbBackgroundColor);
                cv.SetTextColor(tbTextColor);
                ButtonControl bc = new ButtonControl(cv);
                buttons[i].Add(bc);
                cards[i].Add(cv);

                Canvas.SetLeft(bc, 100 * i);
                Canvas.SetTop(bc, cardHeight * j);
                mainCanvas.Children.Add(bc);

                Canvas.SetLeft(cv, cardWidth * i);
                Canvas.SetTop(cv, cardHeight * j);
                cardWindow.AddCardView(cv);
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e) // Remove Row
        {
            //Remove row from both canvases
            int j = rows - 1;
            for (int i = 0; i < cols; i++)
            {
                mainCanvas.Children.Remove(buttons[i][j]);
                cardWindow.RemoveCardView(cards[i][j]);

                //Remove row from both lists
                buttons[i].RemoveAt(j);
                cards[i].RemoveAt(j);
            }

            //Resize
            rows--;
            Resize();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e) //Settings
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

        private void MenuItem_Click_7(object sender, RoutedEventArgs e) //About
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

        public string GetCardFilename(int cardid)
        {
            DataRow result = cardTable.Select("id = '" + cardid + "'")[0];
            int id = int.Parse(result["id"].ToString());
            return id.ToString("D3") + ".PNG";
        }

        public string GetIconLocation()
        {
            return iconLocation;
        }

        public int GetCardId(string name)
        {
            DataRow result = cardTable.Select("name = '" + name + "'")[0];
            int id = int.Parse(result["id"].ToString());
            return id;
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

        public void SetIconLocation(string location)
        {
            iconLocation = location;
        }

        public void SetBackgroundColor(string colorCode)
        {
            backgroundColor = colorCode;
            cardWindow.SetBackgroundColor(colorCode);
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

        public string GetBackgroundColor()
        {
            return backgroundColor;
        }

        public Color GetTbBackgroundColor()
        {
            return tbBackgroundColor;
        }

        public Color GetTbTextColor()
        {
            return tbTextColor;
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
    }
}
