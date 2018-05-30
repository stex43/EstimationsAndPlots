using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    public class LinearFunction : Function
    {
        public LinearFunction() : this(new double[] { 1, 0 })
        {

        }

        public LinearFunction(double[] initialParameters)
        {
            Parameters = new Dictionary<string, double>
            {
                { "a", initialParameters[0] },
                { "b", initialParameters[1] },
            };
        }

        public override double FunctionValue(double x)
        {
            return Parameters["a"] * x + Parameters["b"];
        }

        public override double[] FunctionValue(double[] x)
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
