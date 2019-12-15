using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.JsonConverters
{
    public class BinaryExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(BinaryExpression));

        public override object ReadJson(JsonReader reader, Type objectType,  object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            JObject jObject = new JObject();

            jObject["NodeType"] = ((Expression)value).NodeType.ToString();
            jObject["Left"] = JObject.FromObject(((BinaryExpression)value).Left, serializer);
            jObject["Right"] = JObject.FromObject(((BinaryExpression)value).Right, serializer);

            jObject.WriteTo(writer);
        }

    }
}
