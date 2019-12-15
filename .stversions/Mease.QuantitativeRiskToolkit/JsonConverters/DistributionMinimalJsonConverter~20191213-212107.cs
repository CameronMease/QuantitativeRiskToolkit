using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.JsonConverters
{
    public class DistributionMinimalJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(Distribution)) && !objectType.IsAssignableTo(typeof(DistributionExpression));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var distribution = (Distribution)value;

            JObject jObject = new JObject();

            jObject["Guid"] = distribution.Guid.ToString();

            jObject.WriteTo(writer);
        }
    }
}
