﻿/*
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

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pinknose.QuantitativeRiskToolkit
{
    public abstract class Distribution
    {        
        public event EventHandler RecalculationRequired;

        [JsonProperty]
        public string TypeAssemblyQualifiedName => this.GetType().AssemblyQualifiedName;

        protected Distribution() : this(Guid.NewGuid())
        { 
        }

        protected Distribution(Guid guid)
        {
            Guid = guid;
            Simulation.RegisterDistribution(this);
        }

        [JsonIgnore]
        protected bool NeedsRecalculation { get; private set; } = true;

        public void SetNeedsRecalculation()
        {
            NeedsRecalculation = true;
            RecalculationRequired?.Invoke(this, new EventArgs());
        }

        protected void SetCalculationsUpToDate()
        {
            NeedsRecalculation = false;
        }
  
        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public Guid Guid { get; private set; }

        /*
        internal void ChangeGuid (string guid)
        {
            Guid oldGuid = this.Guid;
            this.Guid = Guid.Parse(guid);
            Simulation.ChangeDistributionGuid(oldGuid, this.Guid);
        }
        */

        [JsonIgnore]
        private Vector<double> CachedResult { get; set; } = null;
        
        public Vector<double> GetResult()
        {
            if (Simulation.DeserializationInProgress)
            {
                return Vector<double>.Build.Dense(Simulation.NumberOfSamples);
            }
            else
            {
                if (NeedsRecalculation || CachedResult == null)
                {
                    CachedResult = ComputeResult();
                    SetCalculationsUpToDate();
                }

                return CachedResult;
            }
        }

        public Task<Vector<double>> GetResultAsync()
        {
            Task<Vector<double>> task = new Task<Vector<double>>(() => GetResult());
            task.Start();
            return task;
        }

        public void SaveResults(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(filePath);
            }

            File.WriteAllText(filePath, this.ToString());
        }

        public override string ToString()
        {
            /*
            StringBuilder sb = new StringBuilder();
            var results = GetResult();

            for (int i = 0; i < results.Count; i++)
            {
                sb.Append(results[i]);

                if (i < results.Count - 1)
                {
                    sb.Append(",");
                }
            }
            
            return sb.ToString();
            */
            return "";
        }

        /// <summary>
        /// Returns the same distribution, sampled in a new random order.
        /// </summary>
        /// <returns></returns>
        public ResampledDistribution Resample()
        {
            return new ResampledDistribution(this);
        }

        protected abstract Vector<double> ComputeResult();

        /*
        public Vector<double> ResultAsDouble()
        { 
            return GetResult().Map<double>(v => Convert.ToDouble(v));
        }
        */
        
        [JsonProperty]
        public abstract bool ConstrainedToInt { get; }

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

        // -

        public static DistributionExpression operator -(Distribution a, Distribution b) =>
            new DistributionExpression(Expression.Subtract(
                Expression.Call(Expression.Constant(a), ResultMethodInfo),
                Expression.Call(Expression.Constant(b), ResultMethodInfo)),
                a,
                b);

        public static DistributionExpression operator -(Distribution a, double b) =>
            new DistributionExpression(Expression.Subtract(
                    Expression.Call(Expression.Constant(a), ResultMethodInfo),
                    Expression.Constant(b)),
                    a);

        public static DistributionExpression operator -(double a, Distribution b) =>
            new DistributionExpression(Expression.Subtract(
                    Expression.Constant(a),
                    Expression.Call(Expression.Constant(b), ResultMethodInfo)),
                    b);

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

        // -
        public static DistributionExpression operator /(Distribution a, Distribution b) =>
            new DistributionExpression(
                Expression.Call(DivideMethodInfo,
                    Expression.Call(Expression.Constant(a), ResultMethodInfo),
                    Expression.Call(Expression.Constant(b), ResultMethodInfo)),
                a, b);

        public static DistributionExpression operator /(Distribution a, double b) =>
            new DistributionExpression(Expression.Divide(
                    Expression.Call(Expression.Constant(a), ResultMethodInfo),
                    Expression.Constant(b)),
                    a);

        public static DistributionExpression operator /(double a, Distribution b) =>
            new DistributionExpression(Expression.Divide(
                    Expression.Constant(a),
                    Expression.Call(Expression.Constant(b), ResultMethodInfo)),
                    b);

        [JsonIgnore]
        /// <summary>
        /// Gets any distributions that may be used as inputs to this distribution.
        /// </summary>
        public IEnumerable<Distribution> ReferencedDistributions
        {
            get
            {
                Dictionary<Guid, Distribution> references = new Dictionary<Guid, Distribution>();
                Type thisType = this.GetType();

                // Look for distributions stored as properties
                foreach (var property in thisType.GetProperties())
                {
                    if (property.PropertyType == typeof(Distribution) || property.PropertyType.IsSubclassOf(typeof(Distribution)))
                    {
                        var distr = (Distribution)property.GetValue(this);
                        if (!references.ContainsKey(distr.Guid))
                        {
                            references.Add(distr.Guid, distr);
                        }
                    }
                    else if (property.PropertyType == typeof(QrtValue) || property.PropertyType.IsSubclassOf(typeof(QrtValue)))
                    {
                        var qrtValue = (QrtValue)property.GetValue(this);

                        if (qrtValue.IsDistribution && !references.ContainsKey(qrtValue.DistributionValue.Guid))
                        {
                            references.Add(qrtValue.DistributionValue.Guid, qrtValue.DistributionValue);
                        }
                    }
                }

                //TODO: Need to return references that are in expressions
                /*
                if (thisType == typeof(DistributionExpression) || thisType.IsSubclassOf(typeof(DistributionExpression)))
                {
                    throw new NotImplementedException();
                }
                */

                return references.Values;
            }
        }

        // Statistics
        [JsonIgnore]
        public double Mean => Statistics.Mean(GetResult());
        [JsonIgnore] 
        public double Median => Statistics.Median(GetResult());
        public double Percentile(int p) => Statistics.Percentile(GetResult(), p);
        [JsonIgnore] 
        public double StandardDeviation => Statistics.StandardDeviation(GetResult());
        [JsonIgnore] 
        public double Variance => Statistics.Variance(GetResult());
        [JsonIgnore]
        public double Minimum => Statistics.Minimum(GetResult());
        [JsonIgnore]
        public double Maximum => Statistics.Maximum(GetResult());

        [JsonIgnore]
        public Histogram Histogram => new Histogram(GetResult(), 100);

        protected static readonly MethodInfo ResultMethodInfo = typeof(Distribution).GetMethod("GetResult", Array.Empty<Type>());
        // protected static readonly MethodInfo ResultAsDoubleMethodInfo = typeof(Distribution).GetMethod("ResultAsDouble", Array.Empty<Type>());
        protected static readonly MethodInfo MultiplyMethodInfo = typeof(Vector<Double>).GetMethod("op_DotMultiply");
        protected static readonly MethodInfo DivideMethodInfo = typeof(Vector<Double>).GetMethod("op_DotDivide");
    }
}
