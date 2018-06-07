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
    public partial class MainWindow : Window
    {
        private void FunctionChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (string)FunctionChoice.SelectedValue;
            if (selected != "Полином")
            {
                PolynomDegree.Visibility = Visibility.Hidden;
                DegreeLabel.Visibility = Visibility.Hidden;

                ParametersTable.Visibility = Visibility.Visible;

                Type TestType = Type.GetType("EstimationsAndPlots." + dictionaryFunction[selected]);

                if (TestType != null)
                {
                    ConstructorInfo ci = TestType.GetConstructor(new Type[] { typeof(int) });
                    operatingFunction = (Function)ci.Invoke(new object[] { 1 });
                }

                SetNewOperatingFunction();
            }
            else
            {
                PolynomDegree.Visibility = Visibility.Visible;
                DegreeLabel.Visibility = Visibility.Visible;
            }

            Plot.Focus();
        }

        private void PolynomDegree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {          
            ParametersTable.Visibility = Visibility.Visible;

            var stringDegree = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
            operatingFunction = new FunctionPolynom(int.Parse(stringDegree));

            Plot.Focus();

            SetNewOperatingFunction();
        }

        private void SetNewOperatingFunction()
        {
            operatingFunctionParameters.Clear();

            parametersFunctions.Clear();
            parametersFunctions.Add(operatingFunction.GetParametersValues());

            foreach (var parameter in operatingFunction.GetParameters())
            {
                var newParameter = new FunctionParameter(parameter.Key, parameter.Value);

                operatingFunctionParameters.Add(newParameter);
            }

            DrawFunctions();

            if (minimizer != null && metrics != null)
            {
                Optimize.IsEnabled = true;
            }

            OptimizationOptionsUpdate(MaxiterValue, new RoutedEventArgs());
            OptimizationOptionsUpdate(EpsValue, new RoutedEventArgs());

            FunctionRepresentation.Content = operatingFunction.TextFunctionRepresentation;
        }

        private void MinimizerChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (string)MinimizerChoice.SelectedValue;
            Type TestType = Type.GetType("EstimationsAndPlots." + dictionaryMinimizer[selected]);

            if (TestType != null)
            {
                ConstructorInfo ci = TestType.GetConstructor(new Type[] { });
                minimizer = (IMinimize)ci.Invoke(new object[] { });
            }

            if (metrics != null && operatingFunction != null)
            {
                Optimize.IsEnabled = true;
            }
        }

        private void MetricsChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (string)MetricsChoice.SelectedValue;
            Type TestType = Type.GetType("EstimationsAndPlots." + dictionaryMetrics[selected]);

            if (TestType != null)
            {
                ConstructorInfo ci = TestType.GetConstructor(new Type[] { typeof(ObservableCollection<Point>) });
                metrics = (IDistance)ci.Invoke(new object[] { operatingDataSet });
            }

            if (minimizer != null && operatingFunction != null)
            {
                Optimize.IsEnabled = true;
            }
        }
    }
}
