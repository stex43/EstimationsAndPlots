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
        private PlotModel model = new PlotModel();
        private Function operatingFunction;
        private List<Point> operatingDataSet = new List<Point>();
        private List<TextBox> parametersTextBoxes = new List<TextBox>();
        private List<TextBlock> parametersTextBlocks = new List<TextBlock>();

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

            myController.UnbindMouseWheel();

            parametersTextBoxes.Add(P1);
            parametersTextBoxes.Add(P2);
            parametersTextBoxes.Add(P3);

            parametersTextBlocks.Add(T1);
            parametersTextBlocks.Add(T2);
            parametersTextBlocks.Add(T3);
        }

        private void FunctionChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            operatingFunction = functions[(string)FunctionChoice.SelectedValue];

            DrawFunction("");

            int i = 0;
            foreach (var parameter in operatingFunction.GetParameters())
            {            
                parametersTextBoxes[i].Visibility = Visibility.Visible;
                parametersTextBoxes[i].Text = parameter.Value.ToString();

                parametersTextBlocks[i].Visibility = Visibility.Visible;
                parametersTextBlocks[i].Text = parameter.Key;

                i++;
            }
        }

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
            }
        }

        private void P_UpdateParameter(object sender, RoutedEventArgs e)
        {
            var parameterIndex = parametersTextBoxes.IndexOf((TextBox)sender);

            var parameterName = parametersTextBlocks[parameterIndex].Text;
            var parameterValue = double.Parse(parametersTextBoxes[parameterIndex].Text.Replace(',', '.'), CultureInfo.InvariantCulture);

            operatingFunction.SetParameterValue(parameterName, parameterValue);

            model.Series.Clear();

            DrawFunction("");
        }

        private void P_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                P_UpdateParameter(sender, new RoutedEventArgs());
            }
        }

        private void Optimize_Click(object sender, RoutedEventArgs e)
        {
            var optimizer = new NelderMeadMinimizer();
            var distance = new SquaredResidualsSumDistance(operatingDataSet.ToArray());
            var newParameters = optimizer.Minimize(operatingFunction, distance);

            DrawFunction("optimized");
        }

        private void DrawFunction(string legendName)
        {
            var xMin = model.DefaultXAxis.ActualMinimum;
            var xMax = model.DefaultXAxis.ActualMaximum;

            if (legendName == "")
            {
                model.Series.Add(new FunctionSeries(operatingFunction.FunctionValue, xMin, xMax, (xMax - xMin) / 1000));

            }
            else
            {
                model.Series.Add(new FunctionSeries(operatingFunction.FunctionValue, xMin, xMax, (xMax - xMin) / 1000, legendName));
            }

            Plot.InvalidatePlot();
        }

        private void Plot_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Plot.ZoomAllAxes(1.2);
                DrawFunction("");
            }
            else if (e.Delta < 0)
            {
                Plot.ZoomAllAxes(0.85);
                DrawFunction("");
            }
        }
    }
}