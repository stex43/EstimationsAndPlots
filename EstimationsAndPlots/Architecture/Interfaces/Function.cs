using System;
using System.Collections.Generic;
using System.Linq;

namespace EstimationsAndPlots
{
    public abstract class Function
    {
        abstract public double FunctionValue(double x);
        abstract public double[] FunctionValue(double[] x);

        protected Dictionary<string, double> Parameters;

        public int NumberOfParameters => Parameters.Count;

        public List<string> GetParametersNames()
        {
            return new List<string>(Parameters.Keys);
        }

        public double GetParameterValue(string parameterName)
        {
            return Parameters[parameterName];
        }

        public double[] GetParametersValues()
        {
            IEnumerable<double> parametersValues =
                from p in Parameters.Values
                select p;

            return parametersValues.ToArray();
        }

        public Dictionary<string, double> GetParameters()
        {
            return new Dictionary<string, double> (Parameters);
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

        public void SetParametersValues(double[] parametersValues)
        {
            if (parametersValues.Count() == NumberOfParameters)
            {
                var parametersNames = GetParametersNames();
                for (int i = 0; i < NumberOfParameters; i++)
                {
                    Parameters[parametersNames[i]] = parametersValues[i];
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
