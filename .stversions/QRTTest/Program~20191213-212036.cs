using MathNet.Numerics.LinearAlgebra;
using Mease.QuantitativeRiskToolkit;
using Mease.QuantitativeRiskToolkit.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace QRTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation.NumberOfSamples = 5;
            Simulation.SeedRequestCount = 8;
            Simulation.RandomOrgApiKey = "e0cc07b7-96b2-4d68-a71a-905cea205c9f";


            var d1 = new DiscreteUniformDistribution(1, 50);
            var d2 = new DiscreteUniformDistribution(84, 100);
            var d3 = new ContinuousUniformDistribution(d1, d2)
            {
                Name = "dumdum"
            };
            var d4 = d3.Resample();
            var d5 = d3 + d4 + 2 + 5;


            var duhh = d5.ReferencedDistributions;

            //var tempString = JsonConvert.SerializeObject(d5);

            var min = new ContinuousUniformDistribution(1, 5);
            var mostLikely = new ContinuousUniformDistribution(7,8);
            var max = new ContinuousUniformDistribution(90, 100);
            var p1 = new PertDistribution(1,50, 100);

            var p2 = new PertDistribution(new ContinuousUniformDistribution(1,5), new ContinuousUniformDistribution(48,59), new ContinuousUniformDistribution(99,100));

            Simulation.NumberOfSamples = 100000;

            Stopwatch stopwatch = new Stopwatch();

            //p2.ChunkSize = 10000;

            stopwatch.Restart();
            for (int i =0; i < 1; i++)
            {
                p1.SetNeedsRecalculation();
                var p1T = p1.GetResultAsync().GetAwaiter().GetResult();
                p2.SetNeedsRecalculation();
                var p2T = p2.GetResultAsync().GetAwaiter().GetResult();
            }
            stopwatch.Stop();
            var p2Ms = stopwatch.ElapsedMilliseconds;

            //p2.ChunkSize = 100000;

            stopwatch.Restart();
            for (int i = 0; i < 1; i++)
            {
                p2.SetNeedsRecalculation();
                p2.GetResult();
            }
            stopwatch.Stop();
            var p1Ms = stopwatch.ElapsedMilliseconds;

            p1.SaveResults(@"c:\temp\dumdum.csv");

            PrintVector(d1);
            PrintVector(d2);
            PrintVector(d3);
            PrintVector(d4);

            Console.WriteLine(d3.Median);

            Simulation.NumberOfSamples = 1000;
            var duhh41 = new BernoulliDistribution(.435762, 123456);

            var d6 = (d1 + 5) / d2;
            var duhh42 = JsonConvert.SerializeObject(d6, JsonSettings.SerializerSettings);

            var duhh123 = Simulation.Serialize();

            Simulation.Deserialize(duhh123);

            return;

            using (var writer = new StreamWriter(@"c:\temp\dumdum.csv"))
            {
                var v1 = d1.GetResult();
                var v2 = d2.GetResult();
                var v3 = d3.GetResult();
                var v4 = d4.GetResult();
                var v5 = d5.GetResult();

                for (int i =0; i < Simulation.NumberOfSamples; i++)
                {
                    writer.WriteLine($"{v1[i]},{v2[i]},{v3[i]},{v4[i]},{v5[i]}");
                    //writer.WriteLine($"{d1.Result()[i]}");
                }
            }
            
        }

        static void PrintVector(Distribution est) 
        {
            var results = est.GetResult();

            for (int i = 0; i < 10; i++)
            {
                Console.Write("{0:N}, ", results[i]);
            }

            Console.WriteLine();
        }

        static void PrintVector(Vector<double> results)
        {

            for (int i = 0; i < 10; i++)
            {
                Console.Write("{0:N}, ", results[i]);
            }

            Console.WriteLine();
        }
    }
}
