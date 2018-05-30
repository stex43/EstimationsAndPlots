using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    public class SquaredResidualsSumDistance : IDistance
    {
        public SquaredResidualsSumDistance(Point[] data)
        {
            this.data = new Point[data.Count()];
            data.CopyTo(this.data, 0);
        }

        private Point[] data;
        
        public Point[] Data { get => data; set => data = value; }

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
    }
}
