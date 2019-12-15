using MathNet.Numerics.LinearAlgebra;
using Mease.QuantitativeRiskToolkit;
using System;
using System.IO;

namespace QRTTest
{
    class Program
    {
        static void Main(string[] args)
        { 
            var d1 = new DiscreteUniformDistribution(1, 50);
            var d2 = new DiscreteUniformDistribution(84, 100);
            var d3 = new DiscreteUniformDistribution(d1, d2);
            var d4 = d3.Resample();

            PrintVector(d1);
            PrintVector(d2);
            PrintVector(d3);
            PrintVector(d4);

            Console.WriteLine(d3.Median);

            using (var writer = new StreamWriter(@"c:\temp\dumdum.csv"))
            {
                var v1 = d1.Result();
                var v2 = d2.Result();
                var v3 = d3.Result();

                for (int i =0; i < Simulation.NumberOfSamples; i++)
                {
                    writer.WriteLine($"{v1[i]},{v2[i]},{v3[i]}");
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
