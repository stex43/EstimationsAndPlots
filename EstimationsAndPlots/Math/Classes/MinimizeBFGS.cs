using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra;

namespace EstimationsAndPlots
{
    public class MinimizeBFGS : IMinimize
    {
        class MinimizedFunction : IObjectiveFunction
        {
            private Function function;
            private IDistance distance;

            public MinimizedFunction(Function function, IDistance distance)
            {
                this.function = function;
                this.distance = distance;
            }

            public Vector<double> Point => Vector<double>.Build.Dense(function.GetParametersValues());

            public double Value => distance.Distance(function);

            public bool IsGradientSupported => true;

            public Vector<double> Gradient => Vector<double>.Build.Dense(distance.DistanceDerivative(function));

            public bool IsHessianSupported => false;

            public Matrix<double> Hessian => throw new NotImplementedException();

            public IObjectiveFunction CreateNew()
            {
                var newFunction = function.Copy();
                return new MinimizedFunction(newFunction, distance);
            }

            public void EvaluateAt(Vector<double> point)
            {
                function.SetParametersValues(point.ToArray());
            }

            public IObjectiveFunction Fork()
            {
                throw new NotImplementedException();
            }
        }

        public double[] Minimize(Function function, IDistance distance, double eps, int maxiter)
        {
            var f = new BfgsMinimizer(eps, eps, eps, maxiter);

            var p0 = Vector<double>.Build.Dense(function.GetParametersValues());

            var e = f.FindMinimum(new MinimizedFunction(function, distance), p0);

            return e.MinimizingPoint.ToArray();
        }
    }
}
