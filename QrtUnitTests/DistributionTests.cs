using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pinknose.QuantitativeRiskToolkit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using Newtonsoft.Json.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System.Diagnostics;
using MathNet.Numerics.Statistics;
using System.IO;

namespace Pinknose.QuantitativeRiskToolkit.Tests
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

            Assert.AreEqual(expectedResults, distr.GetResult());

            distr = new DiscreteUniformDistribution(1, 200);
            Assert.AreEqual(distr.Min.ScalarValue, 1);
            Assert.AreEqual(distr.Max.ScalarValue, 200);

            distr = new DiscreteUniformDistribution();
            Assert.AreEqual(distr.Min.ScalarValue, 0);
            Assert.AreEqual(distr.Max.ScalarValue, 0);
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

            Assert.AreEqual(expectedResults, distr.GetResult());
        }

        [TestMethod()]
        public void BernoulliDistribution()
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

            Assert.AreEqual(expectedResults, distr.GetResult());

            distr = new BernoulliDistribution(.567);
            Assert.AreEqual(distr.Probability.ScalarValue, .567);

            bool exceptionCaught = false;
            try
            {
                distr.Probability = -1;
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);

            exceptionCaught = false;
            try
            {
                distr.Probability = 1.1;
            }
            catch (ArgumentException)
            {
                exceptionCaught = true;
            }

            Assert.IsTrue(exceptionCaught);

            distr.Probability = 0;
            distr.Probability = 1;

            Assert.IsTrue(distr.ConstrainedToInt);

            distr = new BernoulliDistribution();
            Assert.AreEqual(1, distr.Probability.ScalarValue);
        }

        [TestMethod]
        public void FixedDistribution_Result()
        {
            FixedDistribution distr = new FixedDistribution(new double[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(distr.GetResult()[0], 1);
            Assert.AreEqual(distr.GetResult()[1], 2);
            Assert.AreEqual(distr.GetResult()[2], 3);
            Assert.AreEqual(distr.GetResult()[3], 4);
            Assert.AreEqual(distr.GetResult()[4], 5);
        }

        [TestMethod]
        public void DistributionOperators()
        {
            FixedDistribution fixedDistr = new FixedDistribution(new double[] { 1, 2, 3});

            // Distribution + Distribution
            var result = fixedDistr + fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 2);
            Assert.AreEqual(result.GetResult()[1], 4);
            Assert.AreEqual(result.GetResult()[2], 6);

            // Distribution + double
            result = fixedDistr + 2.0;

            Assert.AreEqual(result.GetResult()[0], 3);
            Assert.AreEqual(result.GetResult()[1], 4);
            Assert.AreEqual(result.GetResult()[2], 5);

            // double + distribution
            result = 2.0 + fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 3);
            Assert.AreEqual(result.GetResult()[1], 4);
            Assert.AreEqual(result.GetResult()[2], 5);

            // Distribution - Distribution
            result = fixedDistr - fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 0);
            Assert.AreEqual(result.GetResult()[1], 0);
            Assert.AreEqual(result.GetResult()[2], 0);

            // Distribution - double
            result = fixedDistr - 2.0;

            Assert.AreEqual(result.GetResult()[0], -1);
            Assert.AreEqual(result.GetResult()[1], 0);
            Assert.AreEqual(result.GetResult()[2], 1);

            // double - distribution
            result = 2.0 - fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 1);
            Assert.AreEqual(result.GetResult()[1], 0);
            Assert.AreEqual(result.GetResult()[2], -1);

            // distribution * distribution
            result = fixedDistr * fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 1);
            Assert.AreEqual(result.GetResult()[1], 4);
            Assert.AreEqual(result.GetResult()[2], 9);

            // Distribution * double
            result = fixedDistr * 2.0;

            Assert.AreEqual(result.GetResult()[0], 2);
            Assert.AreEqual(result.GetResult()[1], 4);
            Assert.AreEqual(result.GetResult()[2], 6);

            // double * distribution
            result = 2.0 * fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 2);
            Assert.AreEqual(result.GetResult()[1], 4);
            Assert.AreEqual(result.GetResult()[2], 6);

            // distribution / distribution
            result = fixedDistr / fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 1);
            Assert.AreEqual(result.GetResult()[1], 1);
            Assert.AreEqual(result.GetResult()[2], 1);

            // Distribution / double
            result = fixedDistr / 2.0;

            Assert.AreEqual(result.GetResult()[0], 1.0 / 2.0);
            Assert.AreEqual(result.GetResult()[1], 2.0 / 2.0);
            Assert.AreEqual(result.GetResult()[2], 3.0 / 2.0);

            // double / distribution
            result = 2.0 / fixedDistr;

            Assert.AreEqual(result.GetResult()[0], 2.0 / 1.0);
            Assert.AreEqual(result.GetResult()[1], 2.0 / 2.0);
            Assert.AreEqual(result.GetResult()[2], 2.0 / 3.0);
        }

        [TestMethod]
        public void TestStatistics()
        {
            Simulation.NumberOfSamples = 100000;

            var distr = new ContinuousUniformDistribution(1, 12345);

            Assert.AreEqual(Statistics.Mean(distr.GetResult()), distr.Mean);
            Assert.AreEqual(Statistics.Median(distr.GetResult()), distr.Median);
            Assert.AreEqual(Statistics.StandardDeviation(distr.GetResult()), distr.StandardDeviation);
            Assert.AreEqual(Statistics.Variance(distr.GetResult()), distr.Variance);
            Assert.AreEqual(Statistics.Minimum(distr.GetResult()), distr.Minimum);
            Assert.AreEqual(Statistics.Maximum(distr.GetResult()), distr.Maximum);
            Assert.AreEqual(Statistics.Percentile(distr.GetResult(), 0), distr.Percentile(0));
            Assert.AreEqual(Statistics.Percentile(distr.GetResult(), 5), distr.Percentile(5));
            Assert.AreEqual(Statistics.Percentile(distr.GetResult(), 50), distr.Percentile(50));
            Assert.AreEqual(Statistics.Percentile(distr.GetResult(), 95), distr.Percentile(95));
            Assert.AreEqual(Statistics.Percentile(distr.GetResult(), 100), distr.Percentile(100));

            var histogram = new Histogram(distr.GetResult(), 100);

            for (int i =0; i<histogram.BucketCount; i++)
            {
                Assert.AreEqual(histogram[i].LowerBound, distr.Histogram[i].LowerBound);
                Assert.AreEqual(histogram[i].UpperBound, distr.Histogram[i].UpperBound);
            }
            
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

            Assert.AreEqual(distr.ConstrainedToInt, resampledDist.ConstrainedToInt);

            Assert.IsTrue(
                distr.GetResult()[0] != resampledDist.GetResult()[0] ||
                distr.GetResult()[1] != resampledDist.GetResult()[1] ||
                distr.GetResult()[2] != resampledDist.GetResult()[2] ||
                distr.GetResult()[3] != resampledDist.GetResult()[3] ||
                distr.GetResult()[4] != resampledDist.GetResult()[4]);

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
        public void SimulatedOccurrenceDistribution()
        {
            ContinuousUniformDistribution estimate = new ContinuousUniformDistribution(.5, 1.5, 0);
            EventFrequencyDistribution simulation = new EventFrequencyDistribution(1)
            {
                FrequencyEstimate = estimate
            };

            Assert.AreEqual(simulation.Minimum, 0.0);
            Assert.AreEqual(simulation.Maximum, 2.0);
            //Assert.AreEqual(simulation.Mean, .7511, 0.0001);
            Assert.AreEqual(simulation.Median, 1);

            simulation = new EventFrequencyDistribution(1)
            {
                FrequencyEstimate = 1.5
            };

            Assert.AreEqual(simulation.Minimum, 1.0);
            Assert.AreEqual(simulation.Maximum, 2.0);
            //Assert.AreEqual(simulation.Mean, .7511, 0.0001);
            //Assert.AreEqual(simulation.Median, 1);
        }

        [TestMethod]
        public void TestResultsCache()
        {
            var stopwatch = new Stopwatch();

            Simulation.NumberOfSamples = 100000000;
            var distr = new ContinuousUniformDistribution(1, 1000);

            stopwatch.Start();
            distr.GetResult();
            stopwatch.Stop();
            long test1Milliseconds = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            distr.GetResult();
            stopwatch.Stop();
            long test2Milliseconds = stopwatch.ElapsedMilliseconds;

            Assert.IsTrue(test2Milliseconds < 0.1 * test1Milliseconds);
        }

        /*
        [TestMethod]
        public void Distribution_ToString()
        {
            var distr = new FixedDistribution(1, 2, 3, 4, 5.1);

            Assert.AreEqual("1,2,3,4,5.1", distr.ToString());
        }

        [TestMethod]
        public void Distribution_SaveResults()
        { 
            var distr = new FixedDistribution(1, 2, 3, 4, 5.1);
            string filePath = Path.GetTempFileName();
            distr.SaveResults(filePath);

            string results = File.ReadAllText(filePath);

            Assert.AreEqual("1,2,3,4,5.1", results);

            File.Delete(filePath);
        }
        */
    }
}