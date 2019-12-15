using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pinknose.QuantitativeRiskToolkit.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.Tests
{
    [TestClass()]
    public class SerializationTests
    {
        [TestMethod()]
        public void SimpleDistribution()
        {
            Simulation.NumberOfSamples = 5;
            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);

            string name = "My Distribution";

            var val = new DiscreteUniformDistribution(1, 5, 0)
            {
                Name = name
            };

            Guid guid = val.Guid;
            
            string json = JObject.FromObject(val, serializer).ToString();
            Simulation.ClearDistributionList();
            var newVal = JObject.Parse(json).ToObject<DiscreteUniformDistribution>(serializer);

            Assert.AreEqual(val.Guid, newVal.Guid);
            Assert.AreEqual(val.ContrainedToInt, newVal.ContrainedToInt);
            Assert.AreEqual(val.RandomSeed, newVal.RandomSeed);
            Assert.AreEqual(val.Min.ScalarValue, newVal.Min.ScalarValue);
            //Assert.AreEqual(val.Minimum, newVal.Minimum);
            //Assert.AreEqual(val.Max.ScalarValue, 5);
            //Assert.AreEqual(val.Maximum, 5);
            //Assert.AreEqual(val.Mean, 3.4);
            //Assert.AreEqual(val.Name, name);
        }

        [TestMethod()]
        public void ScalarIntegerQrtValue()
        {
            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);

            QrtValue val = 5;

            string json = JObject.FromObject(val, serializer).ToString();
            var newVal = JObject.Parse(json).ToObject<QrtValue>();

            Assert.AreEqual(val.ConstrainedToInt, newVal.ConstrainedToInt);
            Assert.AreEqual(val.IsDistribution, newVal.IsDistribution);
            Assert.AreEqual(val.ScalarValue, newVal.ScalarValue);           
        }

        [TestMethod()]
        public void ScalarDoubleQrtValue()
        {
            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);

            QrtValue val = 5.0;

            string json = JObject.FromObject(val, serializer).ToString();
            var newVal = JObject.Parse(json).ToObject<QrtValue>();

            Assert.AreEqual(val.ConstrainedToInt, newVal.ConstrainedToInt);
            Assert.AreEqual(val.IsDistribution, newVal.IsDistribution);
            Assert.AreEqual(val.ScalarValue, newVal.ScalarValue);
        }
    }
}
