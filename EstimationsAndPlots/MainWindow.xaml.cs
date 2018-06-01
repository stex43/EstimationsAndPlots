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
        private int maxiter;
        private double eps;
        private int optimizationStepsCount = 10;
        private List<double[]> parametersFunctions = new List<double[]>();

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

            MaxiterName.Visibility = Visibility.Visible;
            Maxiter.Visibility = Visibility.Visible;
            EpsName.Visibility = Visibility.Visible;
            Eps.Visibility = Visibility.Visible;

            Optimize.Visibility = Visibility.Visible;

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

            model.Series.Clear();

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
           
            DrawFunctions();
        }

        private void OptimizationOptionsUpdate(object sender, RoutedEventArgs e)
        {
            if ((TextBox)sender == Maxiter)
            {
                maxiter = int.Parse(Maxiter.Text);
            }
            else if ((TextBox)sender == Eps)
            {
                eps = double.Parse(Eps.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            }
        }

        private void OptimizationOptions_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OptimizationOptionsUpdate(sender, new RoutedEventArgs());
            }
        }
    }
}