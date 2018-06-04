using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private bool isFunctionSet = false;
        private ObservableCollection<Point> operatingDataSet = new ObservableCollection<Point>();
        private List<TextBox> parametersTextBoxes = new List<TextBox>();
        private List<TextBlock> parametersTextBlocks = new List<TextBlock>();
        private int maxiter = 10000;
        private double eps = 1e-8;
        private int optimizationStepsCount = 10;
        private List<double[]> parametersFunctions = new List<double[]>();

        private Dictionary<string, Function> functions = new Dictionary<string, Function>()
        {
            { "LinearFunction", new LinearFunction() },
        };

        public MainWindow()
        {
            InitializeComponent();

            PointsTable.ItemsSource = operatingDataSet;

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
            parametersTextBoxes.Add(P4);
            parametersTextBoxes.Add(P5);
            parametersTextBoxes.Add(P6);

            parametersTextBlocks.Add(T1);
            parametersTextBlocks.Add(T2);
            parametersTextBlocks.Add(T3);
            parametersTextBlocks.Add(T4);
            parametersTextBlocks.Add(T5);
            parametersTextBlocks.Add(T6);

            Maxiter.Text = maxiter.ToString();
            Eps.Text = eps.ToString();
        }

        private void FunctionChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var textBox in parametersTextBoxes)
            {
                textBox.Visibility = Visibility.Hidden;
            }

            foreach (var textBlock in parametersTextBlocks)
            {
                textBlock.Text = "";
            }

            operatingFunction = functions[(string)FunctionChoice.SelectedValue];
            parametersFunctions.Add(operatingFunction.GetParametersValues());

            DrawFunctions();

            int i = 0;
            foreach (var parameter in operatingFunction.GetParameters())
            {            
                parametersTextBoxes[i].Visibility = Visibility.Visible;
                parametersTextBoxes[i].Text = parameter.Value.ToString();

                parametersTextBlocks[i].Visibility = Visibility.Visible;
                parametersTextBlocks[i].Text = parameter.Key;

                i++;
            }

            isFunctionSet = true;

            Optimize.IsEnabled = true;

            OptimizationOptionsUpdate(Maxiter, new RoutedEventArgs());
            OptimizationOptionsUpdate(Eps, new RoutedEventArgs());
        }

        private void P_UpdateParameter(object sender, RoutedEventArgs e)
        {
            var parameterIndex = parametersTextBoxes.IndexOf((TextBox)sender);

            var parameterName = parametersTextBlocks[parameterIndex].Text;
            var parameterValue = double.Parse(parametersTextBoxes[parameterIndex].Text.Replace(',', '.'), CultureInfo.InvariantCulture);

            operatingFunction.SetParameterValue(parameterName, parameterValue);
            parametersFunctions[0] = operatingFunction.GetParametersValues();           

            DrawFunctions();
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
            try
            {
                if (operatingDataSet.Count == 0)
                {
                    throw new NullReferenceException();
                }

                var optimizer = new NelderMeadMinimizer();
                var distanceCount = new SquaredResidualsSumDistance(operatingDataSet.ToArray());

                var distance = distanceCount.Distance(operatingFunction);

                var distanceStep = (distance - eps) / optimizationStepsCount;

                for (int i = 0; i < optimizationStepsCount; i++)
                {
                    distance -= distanceStep;
                    var newParameters = optimizer.Minimize(operatingFunction, distanceCount, distance, maxiter);

                    double s = 0;
                    for (int j = 0; j < newParameters.Length; j++)
                    {
                        s += Math.Pow(newParameters[j] - parametersFunctions[parametersFunctions.Count - 1][j], 2);
                    }
                    s /= newParameters.Length;
                    if (s > 0.01)
                    {
                        parametersFunctions.Add(newParameters);
                    }
                }

                var resultParameters = parametersFunctions[parametersFunctions.Count - 1];
                for (int i = 0; i < resultParameters.Length; i++)
                {
                    parametersTextBoxes[i].Text = resultParameters[i].ToString("G", CultureInfo.InvariantCulture);
                }

                DrawFunctions();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Задайте данные", "", MessageBoxButton.OK);
            }
        }

        private void OptimizationOptionsUpdate(object sender, RoutedEventArgs e)
        {
            if ((TextBox)sender == Maxiter)
            {
                if (!int.TryParse(Maxiter.Text, out int newMaxiter))
                {
                    MessageBox.Show("Введённое число слишком велико", "", MessageBoxButton.OK);
                    Maxiter.Text = maxiter.ToString();
                }
                else if (newMaxiter == 0)
                {
                    MessageBox.Show("Значение не может быть нулём", "", MessageBoxButton.OK);
                    Maxiter.Text = maxiter.ToString();
                }
                else
                {
                    maxiter = newMaxiter;
                }
            }
            else if ((TextBox)sender == Eps)
            {
                var numberStyle = NumberStyles.Float | NumberStyles.AllowCurrencySymbol;
                if (!double.TryParse(Eps.Text.Replace(',', '.'), numberStyle, CultureInfo.InvariantCulture, out double newEps))
                {
                    MessageBox.Show("Неверный формат числа", "", MessageBoxButton.OK);
                    Eps.Text = eps.ToString();
                }
                else if (newEps == 0)
                {
                    MessageBox.Show("Значение не может быть нулём", "", MessageBoxButton.OK);
                    Eps.Text = eps.ToString();
                }
                else if (newEps < 0)
                {
                    MessageBox.Show("Значение не может быть меньше нуля", "", MessageBoxButton.OK);
                    Eps.Text = eps.ToString();
                }
                else
                {
                    eps = newEps;
                }
            }
        }

        private void OptimizationOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OptimizationOptionsUpdate(sender, new RoutedEventArgs());
            }
            else
            {
                var isKeyNumber = (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                    (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9);
                var isKeyArrow = e.Key == Key.Left || e.Key == Key.Right;
                var isKeyPlusMinus = e.Key == Key.OemPlus || e.Key == Key.OemMinus ||
                    e.Key == Key.Subtract || e.Key == Key.Add;

                if ((TextBox)sender == Maxiter && (!isKeyNumber || e.Key == Key.Space) && e.Key != Key.Back
                    && !isKeyArrow)
                {
                    e.Handled = true;
                }
                else if ((TextBox)sender == Eps && !isKeyNumber && e.Key != Key.Back && e.Key != Key.OemComma
                    && !isKeyPlusMinus && e.Key != Key.OemPeriod && !isKeyArrow)
                {
                    e.Handled = true;
                }
            }
        }

        private void PointsTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            DrawFunctions();
        }
    }
}