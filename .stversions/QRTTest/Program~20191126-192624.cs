using MathNet.Numerics.LinearAlgebra;
using Mease.QuantitativeRiskToolkit;
using Newtonsoft.Json;
using System;
using System.IO;

namespace QRTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation.NumberOfSamples = 1000000;
            Simulation.SeedRequestCount = 8;
            Simulation.RandomOrgApiKey = "e0cc07b7-96b2-4d68-a71a-905cea205c9f";


            var d1 = new DiscreteUniformDistribution(1, 50);
            var d2 = new DiscreteUniformDistribution(84, 100);
            var d3 = new ContinuousUniformDistribution(d1, d2);
            var d4 = d3.Resample();
            var d5 = d3 + d4 + 2 + 5;

            var duhh = d5.ReferencedDistributions;

            var tempString = JsonConvert.SerializeObject(d5);

            PrintVector(d1);
            PrintVector(d2);
            PrintVector(d3);
            PrintVector(d4);

            Console.WriteLine(d3.Median);

            Simulation.NumberOfSamples = 1000;
            var duhh41 = new BernoulliDistribution(.435762, 123456);
            var duhh42 = JsonConvert.SerializeObject(duhh41.Result());
            

            using (var writer = new StreamWriter(@"c:\temp\dumdum.csv"))
            {
                var v1 = d1.Result();
                var v2 = d2.Result();
                var v3 = d3.Result();
                var v4 = d4.Result();
                var v5 = d5.Result();

                for (int i =0; i < Simulation.NumberOfSamples; i++)
                {
                    writer.WriteLine($"{v1[i]},{v2[i]},{v3[i]},{v4[i]},{v5[i]}");
                    //writer.WriteLine($"{d1.Result()[i]}");
                }
            }
            
        }

        static void PrintVector(Distribution est) 
        {
            var results = est.Result();

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
