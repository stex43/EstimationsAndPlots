using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimationsAndPlots
{
    public class FunctionParameter
    {
        public FunctionParameter(string name, double value)
        {
            parameterName = name;
            parameterValue = value;
        }

        private string parameterName;
        private double parameterValue;

        public string ParameterName { get => parameterName; set => parameterName = value; }
        public double ParameterValue { get => parameterValue; set => parameterValue = value; }

        public string ParameterStringToPrint()
        {
            return string.Format("Параметр {0}: {1}\n", parameterName, parameterValue);
        }
    }
}
