using MathNet.Numerics.LinearAlgebra;
using Mease.QuantitativeRiskToolkit;
using System;

namespace QRTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var d1 = new DiscreteUniformDistribution(1, 100, 123);
            var d2 = new DiscreteUniformDistribution(1, 100);
            var d3 = d1 + d2;
            var d4 = d3 * 2.0;

            var d5 = 2.0 * d3;
            var d6 = new BernoulliDistribution() { Probability = .1 };
            var d7 = d5 * d6;

            Vector<double> duhh = d1.Result();
            
            var duhh1 = duhh.PointwiseMultiply(duhh);

            PrintVector(d1);
            PrintVector(d2);
            PrintVector(d3);
            PrintVector(d4);
            //PrintVector(d7);
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
    }
}
