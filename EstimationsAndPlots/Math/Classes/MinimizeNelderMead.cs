using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.LinearAlgebra;

namespace EstimationsAndPlots
{
    public class MinimizeNelderMead : IMinimize
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

            public bool IsGradientSupported => throw new NotImplementedException();

            public Vector<double> Gradient => throw new NotImplementedException();

            public bool IsHessianSupported => throw new NotImplementedException();

            public Matrix<double> Hessian => throw new NotImplementedException();

            public IObjectiveFunction CreateNew()
            {
                throw new NotImplementedException();
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
            var f = new NelderMeadSimplex(eps, maxiter);

            var p0 = Vector<double>.Build.Dense(function.GetParametersValues());

            var e = f.FindMinimum(new MinimizedFunction(function, distance), p0);
            
            return e.MinimizingPoint.ToArray();
        }
    }
}
