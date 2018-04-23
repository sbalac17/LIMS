using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LIMS.Test
{
    [TestFixture]
    public class DateTest
    {
        [Test]
        [TestCase(2018, 7, 2, "02/07/2018")]
        [TestCase(2018, 2, 7, "07/02/2018")]
        [TestCase(2018, 12, 7, "07/12/2018")]
        [TestCase(2018, 2, 17, "17/02/2018")]
        public void ToStringFormat(int year, int month, int day, string expected)
        {
            var actual = Date(year, month, day).ToString();
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        [TestCase("02/07/2018", 2018, 7, 2)]
        [TestCase("07/02/2018", 2018, 2, 7)]
        [TestCase("07/12/2018", 2018, 12, 7)]
        [TestCase("17/02/2018", 2018, 2, 17)]
        public void ModelBinderFormat(string input, int expectdYear, int expectedMonth, int expectedDay)
        {
            var binder = new DateModelBinder();

            var values = new DictionaryValueProvider<string>(new Dictionary<string, string>
            {
                { "Date", input }
            }, CultureInfo.InvariantCulture);

            var context = new ModelBindingContext
            {
                ValueProvider = values,
                ModelName = "Date"
            };

            var obj = binder.BindModel(null, context);
            Assert.IsInstanceOf(typeof(Date), obj);

            DateTimeOffset date = (Date)obj;
            Assert.AreEqual(expectdYear, date.Year);
            Assert.AreEqual(expectedMonth, date.Month);
            Assert.AreEqual(expectedDay, date.Day);
        }

        class JsonClass
        {
            public Date Date;
        }

        [Test]
        [TestCase(2018, 7, 2, "02/07/2018")]
        [TestCase(2018, 2, 7, "07/02/2018")]
        [TestCase(2018, 12, 7, "07/12/2018")]
        [TestCase(2018, 2, 17, "17/02/2018")]
        public void JsonSerializeFormat(int year, int month, int day, string expected)
        {
            var actualJson = JsonConvert.SerializeObject(new JsonClass
            {
                Date = Date(year, month, day)
            });

            var expectedJson = $"{{\"Date\":\"{expected}\"}}";
            Assert.AreEqual(expectedJson, actualJson);
        }
        
        [Test]
        [TestCase("02/07/2018", 2018, 7, 2)]
        [TestCase("07/02/2018", 2018, 2, 7)]
        [TestCase("07/12/2018", 2018, 12, 7)]
        [TestCase("17/02/2018", 2018, 2, 17)]
        public void JsonDeserializeFormat(string input, int expectdYear, int expectedMonth, int expectedDay)
        {
            var json = $"{{\"Date\":\"{input}\"}}";
            DateTimeOffset date = JsonConvert.DeserializeObject<JsonClass>(json).Date;

            Assert.AreEqual(expectdYear, date.Year);
            Assert.AreEqual(expectedMonth, date.Month);
            Assert.AreEqual(expectedDay, date.Day);
        }

        private Date Date(int year, int month, int day) =>
            new Date(new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero));
    }
}
