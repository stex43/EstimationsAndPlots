using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace EstimationsAndPlots
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                FileName = "Data",
                DefaultExt = ".txt",
                Filter = "Текстовые документы (.txt)|*.txt"
            };

            bool? result = openDialog.ShowDialog();

            if (result == true)
            {
                string filename = openDialog.FileName;
                StreamReader readingFileStream = new StreamReader(filename);
                var listStringPoints = readingFileStream.ReadToEnd().Split('\n');

                for (int i = 0; i < listStringPoints.Count(); i++)
                {
                    var stringPoint = listStringPoints[i];
                    var listXY = stringPoint.Replace("\r", string.Empty).Replace(',', '.').Split();

                    try
                    {
                        if (listXY.Count() != 2)
                        {
                            throw new IOException("Ошибка при чтении файла", i);
                        }

                        if (!double.TryParse(listXY[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x))
                        {
                            throw new IOException("Ошибка при чтении файла", i);
                        }

                        if (!double.TryParse(listXY[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
                        {
                            throw new IOException("Ошибка при чтении файла", i);
                        }

                        operatingDataSet.Add(new Point(x, y));
                    }
                    catch (IOException exception)
                    {
                        MessageBox.Show(string.Format("Ошибка в строке #{0}", i + 1), exception.Message, MessageBoxButton.OK);
                        operatingDataSet.Clear();
                        return;
                    }
                }

                readingFileStream.Close();

                DrawFunctions();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                FileName = "Data",
                DefaultExt = ".txt",
                Filter = "Текстовые документы (.txt)|*.txt"
            };

            bool? result = saveDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveDialog.FileName;
                StreamWriter writingFileStream = new StreamWriter(filename);

                foreach (var point in operatingDataSet)
                {
                    writingFileStream.WriteLine(string.Format("{0} {1}", point.X, point.Y));
                }

                writingFileStream.Close();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}