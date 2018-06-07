using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    public class FunctionPolynom : Function
    {
        private int degree;        

        public FunctionPolynom(int degree)
        {
            Parameters = new Dictionary<string, double>();
            int aCode = char.ConvertToUtf32("a", 0);

            Parameters.Add(char.ConvertFromUtf32(aCode), 1);
            for (int i = 1; i <= degree; i++)
            {
                Parameters.Add(char.ConvertFromUtf32(aCode + i), 0);
            }

            this.degree = degree;
        }

        public FunctionPolynom(int degree, double[] initialParameters)
        {
            if (degree > 5 || degree < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (degree + 1 != initialParameters.Length)
            {
                throw new ArgumentException();
            }

            Parameters = new Dictionary<string, double>();
            int aCode = char.ConvertToUtf32("a", 0);

            for (int i = 0; i <= degree; i++)
            {
                Parameters.Add(char.ConvertFromUtf32(aCode + i), initialParameters[i]);
            }

            this.degree = degree;
        }
        
        public override string TextFunctionRepresentation
        {
            get
            {
                int aCode = char.ConvertToUtf32("a", 0);
                string result= "y = ";
                for (int i = 0; i < degree - 1; i++)
                {
                    result += char.ConvertFromUtf32(aCode + i) + string.Format("*x^{0} + ", degree - i);
                }
                
                if (degree > 0)
                {
                    result += char.ConvertFromUtf32(aCode + degree - 1) + "*x + ";
                }
                result += char.ConvertFromUtf32(aCode + degree);

                return result;
            }
        }

        public override Function Copy()
        {
            var newFunction = new FunctionPolynom(degree, this.GetParametersValues());
            return newFunction;
        }

        public override double FunctionDerivativeParameter(double x, string parameterName)
        {
            var index = GetParametersNames().IndexOf(parameterName);
            var currentDegree = degree - index;

            return Math.Pow(x, currentDegree);
        }

        public override double FunctionValue(double x)
        {
            double result = 0;
            int aCode = char.ConvertToUtf32("a", 0);
            for (int i = 0; i <= degree; i++)
            {
                result += Parameters[char.ConvertFromUtf32(aCode + i)] * Math.Pow(x, degree - i);
            }

            return result;
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
