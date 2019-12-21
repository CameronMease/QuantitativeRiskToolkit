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
            var duhh1 = Speed.FromFeetPerSecond(6);       


            Simulation.NumberOfSamples = 5;
            Simulation.SeedRequestCount = 8;
            Simulation.RandomOrgApiKey = "e0cc07b7-96b2-4d68-a71a-905cea205c9f";

            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);

            var d1 = new DiscreteUniformDistribution(1,5, 0);
            var d2 = new DiscreteUniformDistribution(6, 10, 0);
            var d3 = new DiscreteUniformDistribution(d1, d2, 0);

            string jsonD1 = JObject.FromObject(d1, serializer).ToString();
            string jsonD2 = JObject.FromObject(d2, serializer).ToString();
            string jsonD3 = JObject.FromObject(d3, serializer).ToString();

            Simulation.ClearDistributionList();

            var newD1 = JObject.Parse(jsonD1).ToObject<DiscreteUniformDistribution>(serializer);
            var newD2 = JObject.Parse(jsonD2).ToObject<DiscreteUniformDistribution>(serializer);
            var newVal = JObject.Parse(jsonD3).ToObject<DiscreteUniformDistribution>(serializer);
            var duhh = newVal.Min.DistributionValue;
          
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
