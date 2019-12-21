using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.Tests
{
    [TestClass]
    public class ArithmeticTests
    {
        [TestMethod]
        public void Addition()
        {
            Simulation.NumberOfSamples = 5;
            var d1 = new FixedDistribution(1, 1, 1, 1, 1);
            var d2 = new FixedDistribution(2, 2, 2, 2, 2);
            var d3 = new FixedDistribution(3, 3, 3, 3, 3);

            var r1 = d1 + d2;
            Assert.AreEqual(r1.GetResult(), d3.GetResult());

            var r2 = d1 + 2;
            Assert.AreEqual(r2.GetResult(), d3.GetResult());

            var r3 = 2 + d1;
            Assert.AreEqual(r3.GetResult(), d3.GetResult());
        }

        [TestMethod]
        public void Subtraction()
        {
            Simulation.NumberOfSamples = 5;
            var d1 = new FixedDistribution(1, 1, 1, 1, 1);
            var d2 = new FixedDistribution(2, 2, 2, 2, 2);
            var d3 = new FixedDistribution(3, 3, 3, 3, 3);

            var r1 = d3 - d2;
            Assert.AreEqual(r1.GetResult(), d1.GetResult());

            var r2 = d3 - 2;
            Assert.AreEqual(r2.GetResult(), d1.GetResult());

            var r3 = 3 - d2;
            Assert.AreEqual(r3.GetResult(), d1.GetResult());
        }

        [TestMethod]
        public void Multiplication()
        {
            Simulation.NumberOfSamples = 5;
            var d1 = new FixedDistribution(2, 2, 2, 2, 2);
            var d2 = new FixedDistribution(3, 3, 3, 3, 3);
            var d3 = new FixedDistribution(6, 6, 6, 6, 6);

            var r1 = d1 * d2;
            Assert.AreEqual(r1.GetResult(), d3.GetResult());

            var r2 = d1 * 3;
            Assert.AreEqual(r2.GetResult(), d3.GetResult());

            var r3 = 3 * d1;
            Assert.AreEqual(r3.GetResult(), d3.GetResult());
        }

        [TestMethod]
        public void Division()
        {
            Simulation.NumberOfSamples = 5;
            var d1 = new FixedDistribution(2, 2, 2, 2, 2);
            var d2 = new FixedDistribution(3, 3, 3, 3, 3);
            var d3 = new FixedDistribution(6, 6, 6, 6, 6);

            var r1 = d3 / d2;
            Assert.AreEqual(r1.GetResult(), d1.GetResult());

            var r2 = d3 / 3;
            Assert.AreEqual(r2.GetResult(), d1.GetResult());

            var r3 = 6 / d2;
            Assert.AreEqual(r3.GetResult(), d1.GetResult());
        }
    }
}
