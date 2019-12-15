using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mease.QuantitativeRiskToolkit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using Newtonsoft.Json.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Diagnostics;

namespace Mease.QuantitativeRiskToolkit.Tests
{
    [TestClass()]
    public class DistributionTests
    {
        [TestMethod()]
        public void DiscreteUniformDistribution_Result()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;
            //var duhh = ExpectedResults.GetDistributionObject<BernoulliDistribution>();

            var testInfo = ExpectedResults.Data["DistributionResults"]["DiscreteUniformDistribution"];
            var inputs = testInfo["ConstructorInputs"];
            var resultsArray = testInfo["Results"]
                .Select(jv => (double)jv).ToArray();

            DiscreteUniformDistribution distr = new DiscreteUniformDistribution(
                inputs["min"].Value<int>(),
                inputs["max"].Value<int>(),
                inputs["randomSeed"].Value<int>());

            Vector<double> expectedResults = Vector<double>.Build.DenseOfArray(resultsArray);

            Assert.AreEqual(expectedResults, distr.Result());
        }

        [TestMethod()]
        public void ContinuousUniformDistribution_Result()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;

            var testInfo = ExpectedResults.Data["DistributionResults"]["ContinuousUniformDistribution"];
            var inputs = testInfo["ConstructorInputs"];
            var resultsArray = testInfo["Results"]
                .Select(jv => (double)jv).ToArray();

            ContinuousUniformDistribution distr = new ContinuousUniformDistribution(
                inputs["min"].Value<double>(),
                inputs["max"].Value<double>(),
                inputs["randomSeed"].Value<int>());

            Vector<double> expectedResults = Vector<double>.Build.DenseOfArray(resultsArray);

            Assert.AreEqual(expectedResults, distr.Result());
        }

        [TestMethod()]
        public void BernoulliDistribution_Result()
        {
            Simulation.NumberOfSamples = ExpectedResults.NumberOfSamples;

            var testInfo = ExpectedResults.Data["DistributionResults"]["BernoulliDistribution"];
            var inputs = testInfo["ConstructorInputs"];
            var resultsArray = testInfo["Results"]
                .Select(jv => (double)jv).ToArray();

            BernoulliDistribution distr = new BernoulliDistribution(
                inputs["probability"].Value<double>(),
                inputs["randomSeed"].Value<int>());

            Vector<double> expectedResults = Vector<double>.Build.DenseOfArray(resultsArray);

            Assert.AreEqual(expectedResults, distr.Result());
        }

        [TestMethod]
        public void FixedDistribution_Result()
        {
            FixedDistribution distr = new FixedDistribution(new double[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(distr.Result()[0], 1);
            Assert.AreEqual(distr.Result()[1], 2);
            Assert.AreEqual(distr.Result()[2], 3);
            Assert.AreEqual(distr.Result()[3], 4);
            Assert.AreEqual(distr.Result()[4], 5);
        }

        [TestMethod]
        public void DistributionOperators()
        {
            FixedDistribution fixedDistr = new FixedDistribution(new double[] { 1, 2, 3});

            // Add 2 distributions
            var result = fixedDistr + fixedDistr;

            Assert.AreEqual(result.Result()[0], 2);
            Assert.AreEqual(result.Result()[1], 4);
            Assert.AreEqual(result.Result()[2], 6);

            // Distribution + double
            result = fixedDistr + 2.0;

            Assert.AreEqual(result.Result()[0], 3);
            Assert.AreEqual(result.Result()[1], 4);
            Assert.AreEqual(result.Result()[2], 5);

            // double + distribution
            result = 2.0 + fixedDistr;

            Assert.AreEqual(result.Result()[0], 3);
            Assert.AreEqual(result.Result()[1], 4);
            Assert.AreEqual(result.Result()[2], 5);

            // distribution * distribution
            result = fixedDistr * fixedDistr;

            Assert.AreEqual(result.Result()[0], 1);
            Assert.AreEqual(result.Result()[1], 4);
            Assert.AreEqual(result.Result()[2], 9);

            // Distribution * double
            result = fixedDistr * 2.0;

            Assert.AreEqual(result.Result()[0], 2);
            Assert.AreEqual(result.Result()[1], 4);
            Assert.AreEqual(result.Result()[2], 6);

            // double * distribution
            result = 2.0 * fixedDistr;

            Assert.AreEqual(result.Result()[0], 2);
            Assert.AreEqual(result.Result()[1], 4);
            Assert.AreEqual(result.Result()[2], 6);

            // distribution / distribution
            result = fixedDistr / fixedDistr;

            Assert.AreEqual(result.Result()[0], 1);
            Assert.AreEqual(result.Result()[1], 1);
            Assert.AreEqual(result.Result()[2], 1);

            // Distribution / double
            result = fixedDistr / 2.0;

            Assert.AreEqual(result.Result()[0], 1.0 / 2.0);
            Assert.AreEqual(result.Result()[1], 2.0 / 2.0);
            Assert.AreEqual(result.Result()[2], 3.0 / 2.0);

            // double / distribution
            result = 2.0 / fixedDistr;

            Assert.AreEqual(result.Result()[0], 2.0 / 1.0);
            Assert.AreEqual(result.Result()[1], 2.0 / 2.0);
            Assert.AreEqual(result.Result()[2], 2.0 / 3.0);
        }

        [TestMethod]
        public void Distribution_Resample()
        {
            Simulation.NumberOfSamples = 100000;

            var testInfo = ExpectedResults.Data["DistributionResults"]["ContinuousUniformDistribution"];
            var inputs = testInfo["ConstructorInputs"];

            ContinuousUniformDistribution distr = new ContinuousUniformDistribution(
                inputs["min"].Value<double>(),
                inputs["max"].Value<double>(),
                inputs["randomSeed"].Value<int>());

            var resampledDist = distr.Resample();

            Assert.AreEqual(distr.Maximum, resampledDist.Maximum);
            Assert.AreEqual(distr.Minimum, resampledDist.Minimum);
            Assert.AreEqual(distr.Mean, resampledDist.Mean, 10);
            Assert.AreEqual(distr.Median, resampledDist.Median);
            Assert.AreEqual(distr.StandardDeviation, resampledDist.StandardDeviation, 10);
            Assert.AreEqual(distr.Variance, resampledDist.Variance, 10);

            Assert.AreEqual(distr.ContrainedToInt, resampledDist.ContrainedToInt);

            Assert.IsTrue(
                distr.Result()[0] != resampledDist.Result()[0] ||
                distr.Result()[1] != resampledDist.Result()[1] ||
                distr.Result()[2] != resampledDist.Result()[2] ||
                distr.Result()[3] != resampledDist.Result()[3] ||
                distr.Result()[4] != resampledDist.Result()[4]);

            Assert.AreEqual(distr.Percentile(0), resampledDist.Percentile(0));
            Assert.AreEqual(distr.Percentile(25), resampledDist.Percentile(25));
            Assert.AreEqual(distr.Percentile(50), resampledDist.Percentile(50));
            Assert.AreEqual(distr.Percentile(75), resampledDist.Percentile(75));
            Assert.AreEqual(distr.Percentile(100), resampledDist.Percentile(100));

            // Force recalculation by changing seed on original distribution
            distr.RandomSeed = distr.RandomSeed + 1;
            Assert.AreEqual(distr.Maximum, resampledDist.Maximum);
            Assert.AreEqual(distr.Minimum, resampledDist.Minimum);
            Assert.AreEqual(distr.Mean, resampledDist.Mean, 10);
            Assert.AreEqual(distr.Median, resampledDist.Median);
            Assert.AreEqual(distr.StandardDeviation, resampledDist.StandardDeviation, 10);
            Assert.AreEqual(distr.Variance, resampledDist.Variance, 10);
        }

        [TestMethod]
        public void TestResultsCache()
        {
            var stopwatch = new Stopwatch();

            Simulation.NumberOfSamples = 100000000;
            var distr = new ContinuousUniformDistribution(1, 1000);

            stopwatch.Start();
            distr.Result();
            stopwatch.Stop();
            long test1Milliseconds = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            distr.Result();
            stopwatch.Stop();
            long test2Milliseconds = stopwatch.ElapsedMilliseconds;
        }
    }
}