/*
 *   [SHORT DESCRIPTION]
 *   
 *   Copyright(C) 2019  Cameron Mease (cameron@pinknose.net)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pinknose.QuantitativeRiskToolkit
{
    public class FastContinuousUniformDistribution : EstimatedDistribution
    {
        private QrtValue _min;
        private QrtValue _max;

        public FastContinuousUniformDistribution() : base()
        {

        }

        public FastContinuousUniformDistribution(QrtValue min, QrtValue max) : base()
        {
            Min = min;
            Max = max;
        }

        public FastContinuousUniformDistribution(double min, double max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
        }

        [JsonProperty]
        public QrtValue Min
        {
            get => _min;
            set
            {
                _min = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public QrtValue Max
        {
            get => _max;
            set
            {
                _max = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public override bool ConstrainedToInt => false;

        protected override Vector<double> ComputeResult()
        {
            RandomSource rand = Simulation.GetRandom(RandomSeed);

            
            double[] randomNumbers = new double[Simulation.NumberOfSamples];
            var tempMin = Min;
            var tempMax = Max;

#if false
            Vector<double> vector = Vector<double>.Build.Dense(Simulation.NumberOfSamples);
            rand.NextDoubles(randomNumbers);

            Parallel.For(0, Simulation.NumberOfSamples, (i) =>
            {
                vector[i] = randomNumbers[i] * (tempMax[i] - tempMin[i]) + tempMin[i];
            });
#endif

#if true
            Vector<double> vector; //= Vector<double>.Build.Dense(Simulation.NumberOfSamples);
            rand.NextDoubles(randomNumbers);

            Vector<double> randVector = Vector<double>.Build.DenseOfArray(randomNumbers);

            vector = randVector.PointwiseMultiply(tempMax.RawVector - tempMin.RawVector) + tempMin.RawVector;
           
#endif

#if false
            var vector = pcw.RunCalculations(rand, Simulation.NumberOfSamples,
                (index, randomNumber) =>
                {
                    return randomNumber * (tempMax[index] - tempMin[index]) + tempMin[index];
                });
#endif

            return vector;
        }

        private ParallelCalculationWorkflow pcw = new ParallelCalculationWorkflow();

        public int ChunkSize { get => pcw.ChunkSize; set => pcw.ChunkSize = value; }
    }
}
