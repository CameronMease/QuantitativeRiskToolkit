using MathNet.Numerics.LinearAlgebra;
using Pinknose.QuantitativeRiskToolkit;
using Pinknose.QuantitativeRiskToolkit.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace QRTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulation.NumberOfSamples = 5;
            Simulation.SeedRequestCount = 8;
            Simulation.RandomOrgApiKey = "e0cc07b7-96b2-4d68-a71a-905cea205c9f";

            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);

            var val = new DiscreteUniformDistribution(1, 5, 0);

            string json = JObject.FromObject(val, serializer).ToString();

            Simulation.ClearDistributionList();
            
            var newVal = JObject.Parse(json).ToObject<DiscreteUniformDistribution>();
          
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
