using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    public class SquaredResidualsSumDistance : IDistance
    {
        public SquaredResidualsSumDistance(ObservableCollection<Point> data)
        {
            this.data = data;
        }

        private ObservableCollection<Point> data;
        
        public ObservableCollection<Point> Data { get => data; set => data = value; }

        public double Distance(Function function)
        {
            double distance = 0;

            foreach (var point in Data)
            {
                var functionValue = function.FunctionValue(point.X);
                distance += Math.Pow(functionValue - point.Y, 2);
            }

            return distance;
        }

        public double[] DistanceDerivative(Function function)
        {
            List<double> ders = new List<double>();            

            foreach (var parameter in function.GetParameters())
            {
                double distanceDerivative = 0;

                foreach (var point in Data)
                {
                    var functionValue = function.FunctionValue(point.X);
                    var functionDerivative = function.FunctionDerivativeParameter(point.X, parameter.Key);
                    distanceDerivative += 2 * (functionValue - point.Y) * functionDerivative;
                }

                ders.Add(distanceDerivative);
            }

            return ders.ToArray();
        }
    }
}
