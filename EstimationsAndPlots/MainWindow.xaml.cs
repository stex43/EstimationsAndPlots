using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
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
        private PlotModel model = new PlotModel();
        private Function operatingFunction = new LinearFunction();
        private List<Point> operatingDataSet = new List<Point>();

        private Dictionary<string, Function> functions = new Dictionary<string, Function>()
        {
            { "LinearFunction", new LinearFunction() },
        };

        public MainWindow()
        {
            InitializeComponent();            

            model = Plot.ActualModel;            

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "EstimationsAndPlots").ToArray();
            List<string> functionNamesList = new List<string>();
            foreach (Type type in typelist)
            {
                if (type.BaseType == Type.GetType("EstimationsAndPlots.Function"))
                     functionNamesList.Add(type.Name);
            }
            FunctionChoice.ItemsSource = functionNamesList;

            var myController = new PlotController();

            Plot.Controller = myController;

            myController.BindKeyDown(OxyKey.Right, OxyPlot.PlotCommands.PanLeft);
            myController.BindKeyDown(OxyKey.Left, OxyPlot.PlotCommands.PanRight);
            myController.BindKeyDown(OxyKey.Up, OxyPlot.PlotCommands.PanDown);
            myController.BindKeyDown(OxyKey.Down, OxyPlot.PlotCommands.PanUp);


        }

        private void FunctionChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            operatingFunction = functions[(string)FunctionChoice.SelectedValue];

            model.Series.Add(new FunctionSeries(operatingFunction.FunctionValue, 0, 10, 0.1, "sooo..."));
            Plot.InvalidatePlot();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            OpenFileDialog openDialog = new OpenFileDialog
            {
                FileName = "Data", 
                DefaultExt = ".txt", 
                Filter = "Текстовые документы (.txt)|*.txt" // Filter files by extension
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
                    var listXY = stringPoint.Replace("\r", string.Empty).Split();

                    try
                    {
                        if (listXY.Count() != 2)
                        {
                            throw new IOException("Ошибка при чтении файла", i);
                        }

                        if (!double.TryParse(listXY[0], out double x))
                        {
                            throw new IOException("Ошибка при чтении файла", i);
                        }

                        if (!double.TryParse(listXY[1], out double y))
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

                var scatterSeries = new OxyPlot.Series.ScatterSeries { MarkerType = MarkerType.Circle };
                foreach (var point in operatingDataSet)
                {
                    scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y, 3));
                }

                model.Series.Add(scatterSeries);
                Plot.InvalidatePlot();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}