using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            MyModel = new PlotModel();

            MyModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = -5, Maximum = 5, MajorGridlineStyle = LineStyle.Solid });
            MyModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, MajorGridlineStyle = LineStyle.Solid });
            
            var axe = new LineSeries();
            axe.Points.Add(new DataPoint(MyModel.Axes[0].Minimum, 0));
            axe.Points.Add(new DataPoint(MyModel.Axes[0].Maximum, 0));
            MyModel.Series.Add(axe);

            MyModel.PlotType = PlotType.Cartesian;            
        }

        public PlotModel MyModel { get; private set; }
    }
}