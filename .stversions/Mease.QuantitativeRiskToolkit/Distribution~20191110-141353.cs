using MathNet.Numerics.LinearAlgebra;
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

        public Guid Guid { get; private set; } = Guid.NewGuid();

        protected Vector<double> CachedResult { get; set; } = null;
        protected bool NeedsRecalculation { get; private set; } = true;

        public abstract Vector<double> Result();

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
        new DistributionExpression(Expression.Multiply(
            Expression.Call(Expression.Constant(a), ResultMethodInfo),
            Expression.Call(Expression.Constant(b), ResultMethodInfo)),
            a,
            b);

        public static DistributionExpression operator * (Distribution a, double b) =>
            new DistributionExpression(Expression.Multiply(
                Expression.Call(Expression.Constant(a), ResultMethodInfo),
                Expression.Constant(b)),
                a);

        public static DistributionExpression operator *(double a, Distribution b) => b * a;

        private static readonly MethodInfo ResultMethodInfo = typeof(Distribution).GetMethod("Result", new Type[] { });

        protected void SetNeedsRecalculation()
        {
            NeedsRecalculation = true;
            RecalculationRequired?.Invoke(this, new EventArgs());
        }

        protected void SetCalculationsUpToDate()
        {
            NeedsRecalculation = false;
        }
    }
}
