using System;
using System.Collections.Generic;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.Tests
{
    public static class UnitTestHelpers
    {
        public static bool PropertiesAreEqual<T>(T object1, T object2)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var value1 = property.GetValue(object1);
                var value2 = property.GetValue(object2);

                if (value1 != value2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
