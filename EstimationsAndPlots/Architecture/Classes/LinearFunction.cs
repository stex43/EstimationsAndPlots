using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    class LinearFunction : IFunction
    {
        public LinearFunction()
        {
            Parameters = new double[] { 1, 0 };
        }

        public LinearFunction(double[] initialParameters)
        {
            Parameters = initialParameters;
        }

        private double a, b;

        public int NumberOfParameters => 2;

        public double[] Parameters
        {
            get => Parameters;
            set
            {
                if (value.Count() == NumberOfParameters)
                {
                    a = value[0];
                    b = value[1];
                }
            }
        }

        public double ParameterA { get => a; set => a = value; }

        public double ParameterB { get => b; set => b = value; }

        public double FunctionValue(double x)
        {
            return a * x + b;
        }

        public double[] FunctionValue(double[] x)
        {
            int n = x.Count();
            double[] results = new double[n];

            for (int i = 0; i < n; i++)
            {
                results[i] = FunctionValue(x[i]);
            }

            return results;
        }
    }
}
