using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace EstimationsAndPlots
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlotModel model = new PlotModel();
        private Function operatingFunction = new LinearFunction();

        private Dictionary<string, Function> functions = new Dictionary<string, Function>()
        {
            { "LinearFunction", new LinearFunction() },
        };

        public MainWindow()
        {
            InitializeComponent();

            //model.Series.Add(new FunctionSeries(Math.Sin, 0, 10, 0.1, "sin(x)"));

            model = Plot.ActualModel;
            model.Series.Add(new FunctionSeries(Math.Sin, 0, 10, 0.1, "sin(x)"));

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "EstimationsAndPlots").ToArray();
            List<string> functionNamesList = new List<string>();
            foreach (Type type in typelist)
            {
                if (type.BaseType == Type.GetType("EstimationsAndPlots.Function"))
                     functionNamesList.Add(type.Name);
            }
            FunctionChoice.ItemsSource = functionNamesList;

        }

        private void FunctionChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            operatingFunction = functions[(string)FunctionChoice.SelectedValue];

            model.Series.Add(new FunctionSeries(operatingFunction.FunctionValue, 0, 10, 0.1, "sooo..."));
            Plot.InvalidatePlot();
        }
    }
}