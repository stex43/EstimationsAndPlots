using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    class SquaredResidualsSumDistance : IDistance
    {
        public SquaredResidualsSumDistance(Point[] data)
        {
            Data = data;
        }

        public Point[] Data
        {
            get => Data;
            set
            {
                value.CopyTo(Data, 0);
            }
        }

        public double Distance(IFunction function)
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
