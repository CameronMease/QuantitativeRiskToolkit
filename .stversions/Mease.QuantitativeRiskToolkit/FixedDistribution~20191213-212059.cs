using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Mease.QuantitativeRiskToolkit
{
    public class FixedDistribution : Distribution
    {
        public FixedDistribution(params double[] distribution)
        {
            vector = Vector<double>.Build.DenseOfArray(distribution);
            SetCalculationsUpToDate();
        }

        public override bool ContrainedToInt => false;

        private Vector<double> vector;

        protected override Vector<double> ComputeResult()
        {
            return vector;
        }
    }
}
