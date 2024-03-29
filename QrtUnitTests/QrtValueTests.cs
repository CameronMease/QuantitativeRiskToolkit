﻿using MathNet.Numerics.LinearAlgebra;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.Tests
{
    [TestClass]
    public class QrtValueTests
    {
        const int NumberOfSamples = 1000;

        [TestMethod]
        public void ImplicitOperatorDouble()
        {
            const double Val = 5.6;

            Simulation.NumberOfSamples = NumberOfSamples;

            QrtValue obj = Val;

            Assert.IsFalse(obj.IsDistribution);
            Assert.IsFalse(obj.ConstrainedToInt);
            Assert.AreEqual(obj.ScalarValue, Val);
            Assert.AreEqual(obj.DistributionValue, null);
        }

        [TestMethod]
        public void ImplicitOperatorInteger()
        {
            const int Val = 5;

            Simulation.NumberOfSamples = NumberOfSamples;

            QrtValue obj = Val;

            Assert.IsFalse(obj.IsDistribution);
            Assert.IsTrue(obj.ConstrainedToInt);
            Assert.AreEqual(obj.ScalarValue, Val);
            Assert.AreEqual(obj.DistributionValue, null);
        }

        [TestMethod]
        public void ImplicitOperatorDistributionDouble()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;

            ContinuousUniformDistribution distr = new ContinuousUniformDistribution(1.0, 1000.0);

            QrtValue qrtVal = distr;

            Assert.IsTrue(qrtVal.IsDistribution);
            Assert.IsFalse(qrtVal.ConstrainedToInt);
            Assert.AreEqual(qrtVal.ScalarValue, 0);
            Assert.AreEqual(qrtVal.DistributionValue.GetResult(), distr.GetResult());
        }

        [TestMethod]
        public void ImplicitOperatorDistributionInteger()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;

            DiscreteUniformDistribution distr = new DiscreteUniformDistribution(1, 1000);

            QrtValue qrtVal = distr;

            Assert.IsTrue(qrtVal.IsDistribution);
            Assert.IsTrue(qrtVal.ConstrainedToInt);
            Assert.AreEqual(qrtVal.ScalarValue, 0);
            Assert.AreEqual(qrtVal.DistributionValue.GetResult(), distr.GetResult());
        }

        [TestMethod]
        public void IndexDistribution()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;

            ContinuousUniformDistribution distr = new ContinuousUniformDistribution(1.0, 1000.0);

            QrtValue qrtVal = distr;

            for (int i = 0; i < ExpectedResults.NumberOfSamples; i++)
            {
                Assert.AreEqual(distr.GetResult()[i], qrtVal[i]);
            }
        }

        [TestMethod]
        public void IndexScalar()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;

            const double val = 45.765;

            QrtValue qrtVal = val;

            for (int i = 0; i < ExpectedResults.NumberOfSamples; i++)
            {
                Assert.AreEqual(qrtVal[i], val);
            }
        }

        [TestMethod]
        public void ImplicitOperatorExceptions()
        {
            bool exceptionCaught = false;

            try
            {
                QrtValue val = null;
            }
            catch(ArgumentNullException)
            {
                exceptionCaught = true;
            }

            //Assert.IsTrue(exceptionCaught);

            exceptionCaught = false;

            try
            {
                QrtValue val = new DiscreteUniformDistribution(1.0, 5.0);
            }
            catch(ArgumentException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);
        }

        [TestMethod]
        public void RawVector()
        {
            Distribution distr = new FixedDistribution(1.0, 2.0, 3.0, 4.0, 5.0);

            var qrtValue = (QrtValue)distr;

            Simulation.NumberOfSamples = 5;

            Assert.AreEqual(distr.GetResult(), qrtValue.RawVector);

            qrtValue = (QrtValue)5.0;

            Assert.AreEqual(Vector<double>.Build.Dense(5, 5.0), qrtValue.RawVector);
        }

        [TestMethod]
        public void MaxMin()
        {
            Distribution distr = new FixedDistribution(1.0, 2.0, 3.0, 4.0, 5.0);
            var qrtValue = (QrtValue)distr;

            Assert.AreEqual(5.0, qrtValue.Maximum);
            Assert.AreEqual(1.0, qrtValue.Minimum);

            qrtValue = (QrtValue)10;
            Assert.AreEqual(10.0, qrtValue.Maximum);
            Assert.AreEqual(10.0, qrtValue.Minimum);
        }

    }
}
