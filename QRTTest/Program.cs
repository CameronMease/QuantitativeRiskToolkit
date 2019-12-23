using MathNet.Numerics.LinearAlgebra;
using Pinknose.QuantitativeRiskToolkit;
using Pinknose.QuantitativeRiskToolkit.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using UnitsNet;

namespace QRTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Simulation.NumberOfSamples = 5;
            Simulation.SeedRequestCount = 8;
            Simulation.RandomOrgApiKey = "e0cc07b7-96b2-4d68-a71a-905cea205c9f";

            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);

            var d = new EventFrequencyDistribution(1)
            {
                FrequencyEstimate = new ContinuousUniformDistribution(0, 10.5, 0)
            };

            var r = d.GetResult();

            d.ProbabilityOfOccurence = .5;

            r = d.GetResult();
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
