using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.JsonConverters
{
    public class DistributionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(Distribution));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            JToken jToken = ((JTokenReader)reader).CurrentToken;

            var constructor = objectType.GetConstructor(new Type[] { typeof(Guid) });
            var distributionGuid = new Guid(jToken["Guid"].Value<string>());
            var distribution = constructor.Invoke(new object[] { distributionGuid });
            
            if (objectType.IsAssignableTo(typeof(DistributionExpression)))
            {
                throw new NotImplementedException();
            }
            else
            {
                serializer.Populate(reader, distribution);
            }

            return distribution;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonSerializerHelpers.OptInWriteJson(writer, value, serializer, true);
        }
    }
}
