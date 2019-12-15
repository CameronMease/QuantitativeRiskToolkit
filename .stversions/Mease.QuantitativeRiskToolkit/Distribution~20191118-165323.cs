using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public abstract class Distribution
    {
        
        public event EventHandler RecalculationRequired;
        protected bool NeedsRecalculation { get; private set; } = true;

        protected void SetNeedsRecalculation()
        {
            NeedsRecalculation = true;
            RecalculationRequired?.Invoke(this, new EventArgs());
        }

        protected void SetCalculationsUpToDate()
        {
            NeedsRecalculation = false;
        }
  
        public Guid Guid { get; protected set; } = Guid.NewGuid();

        private Vector<double> CachedResult { get; set; } = null;
        
        public Vector<double> Result()
        {
            if (NeedsRecalculation || CachedResult == null)
            {
                CachedResult = ComputeResult();
                SetCalculationsUpToDate();
            }

            return CachedResult;
        }

        public DistributionExpression Resample()
        {
            return DistributionExpression.CreateResampleExpression(this);
        }

        protected abstract Vector<double> ComputeResult();

        public Vector<double> ResultAsDouble()
        { 
            return Result().Map<double>(v => Convert.ToDouble(v));
        }

        public abstract bool ContrainedToInt { get; }

        ////////////////////////////////////////////////////////////
        // Operator Overloads
        ////////////////////////////////////////////////////////////

        // +

        public static DistributionExpression operator +(Distribution a, Distribution b) =>
            new DistributionExpression(Expression.Add(
                Expression.Call(Expression.Constant(a), ResultMethodInfo),
                Expression.Call(Expression.Constant(b), ResultMethodInfo)),
                a,
                b);

        public static DistributionExpression operator +(Distribution a, double b) =>
            new DistributionExpression(Expression.Add(
                    Expression.Call(Expression.Constant(a), ResultMethodInfo),
                    Expression.Constant(b)),
                    a);
 

        public static DistributionExpression operator +(double a, Distribution b) => b + a;

        // *


        public static DistributionExpression operator *(Distribution a, Distribution b) =>
            new DistributionExpression(
                Expression.Call(MultiplyMethodInfo,
                    Expression.Call(Expression.Constant(a), ResultMethodInfo),
                    Expression.Call(Expression.Constant(b), ResultMethodInfo)),
                a, b);

        public static DistributionExpression operator *(Distribution a, double b) =>
            new DistributionExpression(Expression.Multiply(
                    Expression.Call(Expression.Constant(a), ResultMethodInfo),
                    Expression.Constant(b)),
                    a);
      

        public static DistributionExpression operator *(double a, Distribution b) => b * a;

        // Statistics 
        public double Mean => Statistics.Mean(Result());
        public double Median => Statistics.Median(Result());
        public double Percentile(int p) => Statistics.Percentile(Result(), p);
        public double StandardDeviation => Statistics.StandardDeviation(Result());
        public double Variance => Statistics.Variance(Result());

        public Histogram Histogram => new Histogram(Result(), 100);

        protected static readonly MethodInfo ResultMethodInfo = typeof(Distribution).GetMethod("Result", new Type[] { });
        protected static readonly MethodInfo ResultAsDoubleMethodInfo = typeof(Distribution).GetMethod("ResultAsDouble", new Type[] { });
        protected static readonly MethodInfo MultiplyMethodInfo = typeof(Vector<Double>).GetMethod("op_DotMultiply");
    }
}
