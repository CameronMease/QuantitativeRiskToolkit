using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.JsonConverters
{
    public static class JsonSettings
    {
        private static BinaryExpressionJsonConverter binaryExpressionJsonConverter = new BinaryExpressionJsonConverter();
        private static ConstantExpressionJsonConverter constantExpressionJsonConverter = new ConstantExpressionJsonConverter();
        private static MethodCallExpressionJsonConverter methodCallExpressionJsonConverter = new MethodCallExpressionJsonConverter();
        private static QrtValueJsonConverter qrtValueJsonConverter = new QrtValueJsonConverter();
        private static DistributionMinimalJsonConverter distributionMinimalJsonConverter = new DistributionMinimalJsonConverter();
        private static DistributionExpressionJsonConverter distributionExpressionJsonConverter = new DistributionExpressionJsonConverter();
        private static DistributionJsonConverter distributionJsonConverter = new DistributionJsonConverter();

        public static JsonSerializerSettings SerializerSettings
        {            
            get
            {
                var settings = new JsonSerializerSettings();

                settings.Converters.Add(distributionJsonConverter);
                settings.Converters.Add(binaryExpressionJsonConverter);
                settings.Converters.Add(constantExpressionJsonConverter);
                settings.Converters.Add(methodCallExpressionJsonConverter);
                settings.Converters.Add(qrtValueJsonConverter);
                //settings.Converters.Add(distributionExpressionJsonConverter);

                return settings;
            }
        }

        /*
        public static JsonSerializerSettings DistributionSerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();

                settings.Converters.Add(binaryExpressionJsonConverter);
                settings.Converters.Add(constantExpressionJsonConverter);
                settings.Converters.Add(methodCallExpressionJsonConverter);
                settings.Converters.Add(qrtValueJsonConverter);
                settings.Converters.Add(distributionExpressionJsonConverter);

                return settings;
            }
        }
        */

        

        
    }
}
