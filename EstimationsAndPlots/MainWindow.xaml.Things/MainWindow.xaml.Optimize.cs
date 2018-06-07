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
        private void Optimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operatingDataSet.Count == 0)
                {
                    throw new NullReferenceException();
                }

                var distance = metrics.Distance(operatingFunction);

                /*for (int i = 0; i < optimizationStepsCount; i++)
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
                }*/

                var newParameters = minimizer.Minimize(operatingFunction, metrics, eps, maxiter);

                operatingFunction.SetParametersValues(newParameters);
                parametersFunctions.Add(newParameters);

                operatingFunctionParameters.Clear();
                foreach (var parameter in operatingFunction.GetParameters())
                {
                    var newParameter = new FunctionParameter(parameter.Key, parameter.Value);

                    operatingFunctionParameters.Add(newParameter);
                }

                DrawFunctions();

                LogBox.Text += string.Format("Используемая функция: {0}\n", FunctionChoice.Text);
                LogBox.Text += string.Format("Используемая метрика: {0}\n", MetricsChoice.Text);
                LogBox.Text += string.Format("Используемая функция: {0}\n\n", MinimizerChoice.Text);
                foreach (var parameter in operatingFunctionParameters)
                {
                    LogBox.Text += parameter.ParameterStringToPrint();
                }
                LogBox.Text += string.Format("Расстояние: {0}\n", metrics.Distance(operatingFunction));
                LogBox.Text += "------------------------------------------\n";
                LogBox.ScrollToLine(LogBox.LineCount - 2);

                ClearOptimize.IsEnabled = true;

                Optimize.IsEnabled = false;
                PointsTable.IsEnabled = false;
                ParametersTable.IsEnabled = false;
                AddingDotsMode.IsChecked = false;
                AddingDotsMode.IsEnabled = false;
                ClearPoints.IsEnabled = false;
                MaxiterValue.IsEnabled = false;
                EpsValue.IsEnabled = false;
                FunctionChoice.IsEnabled = false;
                MinimizerChoice.IsEnabled = false;
                MetricsChoice.IsEnabled = false;
                PolynomDegree.IsEnabled = false;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Задайте данные", "", MessageBoxButton.OK);
            }
        }

        private void OptimizationOptionsUpdate(object sender, RoutedEventArgs e)
        {
            if ((TextBox)sender == MaxiterValue)
            {
                if (!int.TryParse(MaxiterValue.Text, out int newMaxiter))
                {
                    MessageBox.Show("Введённое число слишком велико", "", MessageBoxButton.OK);
                    MaxiterValue.Text = maxiter.ToString();
                }
                else if (newMaxiter == 0)
                {
                    MessageBox.Show("Значение не может быть нулём", "", MessageBoxButton.OK);
                    MaxiterValue.Text = maxiter.ToString();
                }
                else
                {
                    maxiter = newMaxiter;
                }
            }
            else if ((TextBox)sender == EpsValue)
            {
                var numberStyle = NumberStyles.Float | NumberStyles.AllowCurrencySymbol;
                if (!double.TryParse(EpsValue.Text, numberStyle, CultureInfo.InvariantCulture, out double newEps))
                {
                    MessageBox.Show("Неверный формат числа", "", MessageBoxButton.OK);
                    EpsValue.Text = eps.ToString();
                }
                else if (newEps == 0)
                {
                    MessageBox.Show("Значение не может быть нулём", "", MessageBoxButton.OK);
                    EpsValue.Text = eps.ToString();
                }
                else if (newEps < 0)
                {
                    MessageBox.Show("Значение не может быть меньше нуля", "", MessageBoxButton.OK);
                    EpsValue.Text = eps.ToString();
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

                if ((TextBox)sender == MaxiterValue && (!isKeyNumber || e.Key == Key.Space) && e.Key != Key.Back
                    && !isKeyArrow)
                {
                    e.Handled = true;
                }
                else if ((TextBox)sender == EpsValue && !isKeyNumber && e.Key != Key.Back && !isKeyPlusMinus && 
                    e.Key != Key.OemPeriod && e.Key != Key.Decimal && !isKeyArrow)
                {
                    e.Handled = true;
                }
            }
        }

        private void ClearOptimize_Click(object sender, RoutedEventArgs e)
        {
            parametersFunctions.Clear();
            parametersFunctions.Add(operatingFunction.GetParametersValues());

            Optimize.IsEnabled = true;
            PointsTable.IsEnabled = true;
            ParametersTable.IsEnabled = true;
            AddingDotsMode.IsEnabled = true;
            ClearPoints.IsEnabled = true;
            MaxiterValue.IsEnabled = true;
            EpsValue.IsEnabled = true;
            FunctionChoice.IsEnabled = true;
            MinimizerChoice.IsEnabled = true;
            MetricsChoice.IsEnabled = true;
            PolynomDegree.IsEnabled = true;

            DrawFunctions();
        }
    }
}
