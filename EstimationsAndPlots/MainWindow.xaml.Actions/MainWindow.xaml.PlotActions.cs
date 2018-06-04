﻿using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
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
    public partial class MainWindow : Window
    {        
        private void DrawFunctions()
        {
            model.Series.Clear();

            var xMin = model.DefaultXAxis.ActualMinimum;
            var xMax = model.DefaultXAxis.ActualMaximum;    

            for (int i = 0; i < parametersFunctions.Count; i++)
            {
                operatingFunction.SetParametersValues(parametersFunctions[i]);
                var series = new FunctionSeries(operatingFunction.FunctionValue, xMin, xMax, (xMax - xMin) / 1000);
                if (i == 0)
                {
                    series.Color = OxyColors.Blue;
                }
                else if (i == parametersFunctions.Count - 1)
                {
                    series.Color = OxyColors.Red;
                    series.Title = "результат";
                }
                else
                {
                    series.LineStyle = LineStyle.Dash;
                }
                model.Series.Add(series);
            }

            if (operatingDataSet.Count != 0)
            {
                var scatterSeries = new OxyPlot.Series.ScatterSeries { MarkerType = MarkerType.Circle,
                    MarkerFill = OxyColors.DarkCyan };
                foreach (var point in operatingDataSet)
                {
                    scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y, 3));
                }
                model.Series.Add(scatterSeries);
            }

            Plot.InvalidatePlot();
        }

        private void Plot_MouseWheel(object sender, MouseWheelEventArgs e)
        {            
            if (e.Delta > 0)
            {
                Plot.ZoomAllAxes(1.2);
                DrawFunctions();
            }
            else if (e.Delta < 0)
            {
                Plot.ZoomAllAxes(0.85);
                DrawFunctions();
            }
        }

        private void Plot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var loc = e.GetPosition(this);
            var args = new OxyMouseDownEventArgs();
            args.Position = new ScreenPoint(loc.X, loc.Y);

            this.Plot_MouseDown(sender, args);
        }

        private void Plot_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var plot = model as PlotModel;
            var axisList = plot.Axes.ToList();

            OxyPlot.Axes.Axis x = null, y = null;
            foreach (var ax in axisList)
            {
                if (ax.Position == AxisPosition.Bottom)
                    x = ax;
                else if (ax.Position == AxisPosition.Left)
                    y = ax;
            }

            var point = OxyPlot.Axes.Axis.InverseTransform(e.Position, x, y);

            this.operatingDataSet.Add(new Point(point.X, point.Y));
        }
    }
}