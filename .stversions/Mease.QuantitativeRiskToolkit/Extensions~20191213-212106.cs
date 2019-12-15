using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public static class Extensions
    {
        public static double[] ConvertToDouble(this int[] intArray) => intArray.Select(v => (double)v).ToArray();

        public static bool IsAssignableTo(this Type thisObjectType, Type objectType) => objectType == thisObjectType || objectType.IsAssignableFrom(thisObjectType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>From https://stackoverflow.com/questions/1130698/checking-if-an-object-is-a-number-in-c-sharp</remarks>
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public static bool IsFloating(this object value)
        {
            return value is float
                    || value is double
                    || value is decimal;
        }

        public static bool IsInteger(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong;
        }
    }
}
