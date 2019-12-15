using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.JsonConverters
{
    public class ConstantExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(ConstantExpression));

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

            var expression = (ConstantExpression)value;

            JObject jObject = new JObject();

            jObject["NodeType"] = ((Expression)value).NodeType.ToString();

            if (expression.Value.IsInteger())
            {
                jObject["Value"] = (Int32)expression.Value;
            }
            else if (expression.Value.IsFloating())
            {
                jObject["Value"] = (Double)expression.Value;
            }
            else
            {
                jObject["Value"] = JObject.FromObject(expression.Value, serializer);
            }

            jObject.WriteTo(writer);
        }
    }
}
