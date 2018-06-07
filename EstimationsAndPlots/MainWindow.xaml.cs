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
        private ObservableCollection<Point> operatingDataSet = new ObservableCollection<Point>();
        private ObservableCollection<FunctionParameter> operatingFunctionParameters =
            new ObservableCollection<FunctionParameter>();
        private int maxiter = 10000;
        private double eps = 1e-8;
        private List<double[]> parametersFunctions = new List<double[]>();
        private IMinimize minimizer;
        private IDistance metrics;

        private Dictionary<string, string> dictionaryFunction = new Dictionary<string, string>
        {
            { "Полином", "FunctionPolynom"}
        };
        private Dictionary<string, string> dictionaryMetrics = new Dictionary<string, string>
        {
            { "Сумма квадратов остатков", "SquaredResidualsSumDistance"}
        };
        private Dictionary<string, string> dictionaryMinimizer = new Dictionary<string, string>
        {
            { "Нелдера-Мида", "MinimizeNelderMead"},
            { "BFGS", "MinimizeBFGS"}
        };

        public MainWindow()
        {
            InitializeComponent();

            PointsTable.ItemsSource = operatingDataSet;
            ParametersTable.ItemsSource = operatingFunctionParameters;

            model = Plot.ActualModel;
            
            var typeNameList = dictionaryFunction.Keys;
            FunctionChoice.ItemsSource = typeNameList;

            typeNameList = dictionaryMetrics.Keys;
            MetricsChoice.ItemsSource = typeNameList;

            typeNameList = dictionaryMinimizer.Keys;
            MinimizerChoice.ItemsSource = typeNameList;

            var myController = new PlotController();

            Plot.Controller = myController;

            myController.BindKeyDown(OxyKey.Right, OxyPlot.PlotCommands.PanLeft);
            myController.BindKeyDown(OxyKey.Left, OxyPlot.PlotCommands.PanRight);
            myController.BindKeyDown(OxyKey.Up, OxyPlot.PlotCommands.PanDown);
            myController.BindKeyDown(OxyKey.Down, OxyPlot.PlotCommands.PanUp);

            myController.UnbindMouseWheel();

            MaxiterValue.Text = maxiter.ToString();
            EpsValue.Text = eps.ToString();

            this.model.MouseDown += (s, e) => this.Plot_MouseDown(s, e);
            model.KeyDown += (s, e) => Plot_KeyDown(s, e);
            model.MouseMove += (s, e) => this.Plot_MouseMove(s, e);
            model.MouseUp += (s, e) => this.Plot_MouseUp(s, e);
        }

        private void PointsTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (parametersFunctions.Count > 1)
            {
                var res = MessageBox.Show("Это действие удалит полученные результаты. Продолжить?", "",
                    MessageBoxButton.YesNo);

                if (res == MessageBoxResult.No)
                {
                    return;
                }
            }
            DrawFunctions();
        }

        private void ParametersTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            parametersFunctions.Clear();
            for (int i = 0; i < operatingFunctionParameters.Count; i++)
            {
                var parameter = operatingFunctionParameters[i];
                operatingFunction.SetParameterValue(parameter.ParameterName, parameter.ParameterValue);
            }

            parametersFunctions.Add(operatingFunction.GetParametersValues());

            DrawFunctions();
        }

        private void ClearPoints_Click(object sender, RoutedEventArgs e)
        {
            operatingDataSet.Clear();

            DrawFunctions();
        }
    }
}