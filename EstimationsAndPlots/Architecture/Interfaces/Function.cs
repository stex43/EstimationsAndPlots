using System.Collections.Generic;

namespace EstimationsAndPlots
{
    abstract class Function
    {
        abstract public double FunctionValue(double x);
        abstract public double[] FunctionValue(double[] x);

        protected Dictionary<string, double> Parameters;

        public IEnumerable<string> GetParametersNames()
        {
            return Parameters.Keys;
        }

        public IEnumerable<double> GetParametersValues()
        {
            return Parameters.Values;
        }

        public void SetParameterValue(string parameterName, double parameterValue)
        {
            if (Parameters.ContainsKey(parameterName))
            {
                Parameters[parameterName] = parameterValue;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }
}
