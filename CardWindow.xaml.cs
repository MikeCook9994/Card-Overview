﻿using System;
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
    /// Interaction logic for CardWindow.xaml
    /// </summary>
    public partial class CardWindow : Window
    {
        private DataTable cardTable;
        private Settings settings;
        private About about;
        private SearchBox searchBox;

        private List<List<CardView>> cards;
        private int cols = 1;
        private int rows = 7;

        private int cardWidth = 100;
        private int cardHeight = 100;

        private string backgroundColor = "#00FF00";
        private Color tbBackgroundColor = Colors.White;
        private Color tbTextColor = Colors.Black;

        private string iconLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\icons\\";

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

            List<CardView> colC = new List<CardView>();

            int i = cols - 1;
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

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) //Remove Column
        {
            //Remove column from both canvases
            int i = cols - 1;
            for (int j = 0; j < rows; j++)
            {
                RemoveCardView(cards[i][j]);
            }

            //Remove column from both lists
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
                cards[i].Add(cv);

                Canvas.SetLeft(cv, cardWidth * i);
                Canvas.SetTop(cv, cardHeight * j);
                AddCardView(cv);
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e) // Remove Row
        {
            //Remove row from both canvases
            int j = rows - 1;
            for (int i = 0; i < cols; i++)
            {
                RemoveCardView(cards[i][j]);

                //Remove row from both lists
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

        private bool LoadFromFile(string filename)
        {
            string[] lines;
            if ((lines = System.IO.File.ReadAllLines(filename)) != null)
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

        private void Resize()
        {
            Width = (cols * cardWidth) + 17;
            Height = (rows * cardHeight) + 40;

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

        public string GetIconLocation()
        {
            return iconLocation;
        }

        public string GetCardFilename(int cardid)
        {
            DataRow result = cardTable.Select("id = '" + cardid + "'")[0];
            int id = int.Parse(result["id"].ToString());
            return id.ToString("D3") + ".PNG";
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCardList();
            LoadSettings();

            SetBackgroundColor(backgroundColor);
            Show();

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
    }
}
