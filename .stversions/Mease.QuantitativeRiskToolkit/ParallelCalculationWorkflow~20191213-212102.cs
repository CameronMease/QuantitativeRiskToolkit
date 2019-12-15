﻿using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Mease.QuantitativeRiskToolkit
{
    public class ParallelCalculationWorkflow
    {
        private Vector<double> results;
        public int ChunkSize { get; set; } = 1000;

        private Func<int, double, double> SampleFunction { get; set; }

        public Vector<double> RunCalculations(RandomSource random, int iterations, Func<int, double, double> sampleFunction)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            else if (iterations % ChunkSize != 0)
            {
                throw new ArgumentException($"Iteration count must be a multiple of {ChunkSize}.", nameof(iterations));
            }

            SampleFunction = sampleFunction;

            int numberOfChunks = iterations / ChunkSize;

            results = Vector<double>.Build.Dense(iterations);

            var randomNumberBuffer = new BufferBlock<(int chunkNumber, double[] randomNumbers)>();
            var calculationBlock = new ActionBlock<(int chunkNumber, double[] randomNumbers)>(DoCalculation, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = -1 });

            randomNumberBuffer.LinkTo(calculationBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            for (int i = 0; i < numberOfChunks; i++)
            {
                double[] randomArray = new double[ChunkSize];
                random.NextDoubles(randomArray);
                randomNumberBuffer.Post((i, randomArray));
            }

            randomNumberBuffer.Complete();

            calculationBlock.Completion.Wait();

            return results;
        }

        public Vector<double> RunCalculations(RandomSource random, int iterations)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }
            else if (iterations % ChunkSize != 0)
            {
                throw new ArgumentException($"Iteration count must be a multiple of {ChunkSize}.", nameof(iterations));
            }

            int numberOfChunks = iterations / ChunkSize;

            results = Vector<double>.Build.Dense(iterations);

            var randomNumberBuffer = new BufferBlock<(int chunkNumber, double[] randomNumbers)>();
            var calculationBlock = new ActionBlock<(int chunkNumber, double[] randomNumbers)>(DoCalculation, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = -1});

            randomNumberBuffer.LinkTo(calculationBlock, new DataflowLinkOptions() { PropagateCompletion = true });


            for (int i = 0; i < numberOfChunks; i++)
            {
                double[] randomArray = new double[ChunkSize];
                random.NextDoubles(randomArray);
                randomNumberBuffer.Post((i, randomArray));                
            }

            randomNumberBuffer.Complete();

            calculationBlock.Completion.Wait();

            return results;
        }

        /*
        private void DoCalculation((int chunkNumber, double[] randomNumbers) inputs)
        {
            Parallel.For(0, inputs.randomNumbers.Length, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, i =>
            {
                results[ChunkSize * inputs.chunkNumber + i] = inputs.randomNumbers[i] * (100 - 1) + 1;
            });
        }
        */

        private void DoCalculation((int chunkNumber, double[] randomNumbers) inputs)
        {
            Parallel.For(0, inputs.randomNumbers.Length, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, i =>
            {
                results[ChunkSize * inputs.chunkNumber + i] = SampleFunction(i, inputs.randomNumbers[i]);
            });
        }

    }
}
