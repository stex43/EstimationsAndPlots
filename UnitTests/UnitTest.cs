using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EstimationsAndPlots;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestNelderMead()
        {
            LinearFunction function = new LinearFunction();

            List<Point> data = new List<Point>();
            data.Add(new Point(0, 1));
            data.Add(new Point(1, 3));
            SquaredResidualsSumDistance distance = new SquaredResidualsSumDistance(data.ToArray());

            var minimizer = new NelderMeadMinimizer();
            var res = minimizer.Minimize(function, distance, 0.01, 100);

            function.SetParametersValues(res);

            Assert.AreEqual(res[0], 2, 1e-8);
            Assert.AreEqual(res[1], 1, 1e-8);
        }
    }
}
