using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class ExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            bool returnVal = objectType.IsAssignableTo(typeof(Expression));
            return returnVal;
        }

        public override object ReadJson(JsonReader reader, Type objectType,  object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jObject = new JObject();

            jObject["Operator"] = value.GetType().FullName;

           

            jObject.WriteTo(writer);
        }

    }
}
