using System;
using System.Globalization;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace LIMS
{
    [ModelBinder(typeof(DateModelBinder))]
    [JsonConverter(typeof(DateSerializer))]
    public struct Date : IFormattable
    {
        public const string Format = "dd/MM/yyyy";
        public const string Placeholder = "{0:" + Format + "}";

        private readonly DateTimeOffset _date;

        public Date(DateTimeOffset date)
        {
            _date = date;
        }

        public static implicit operator Date(DateTimeOffset date)
        {
            return new Date(date);
        }

        public static implicit operator DateTimeOffset(Date date)
        {
            return date._date;
        }

        public override string ToString()
        {
            return _date.ToString(Format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _date.ToString(format, formatProvider);
        }
    }

    /// <summary>
    /// Binds <see cref="Date"/> in format <c>DD/MM/YYYY</c>.
    /// </summary>
    public class DateModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return new Date(DateTimeOffset.ParseExact(result.AttemptedValue, Date.Format, null, DateTimeStyles.AssumeLocal));
        }
    }

    public class DateSerializer : JsonConverter<Date>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, Date value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(Date.Format, CultureInfo.InvariantCulture));
        }

        public override Date ReadJson(JsonReader reader, Type objectType, Date existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new Exception("Date value must be a string.");

            var value = reader.Value.ToString();
            return new Date(DateTimeOffset.ParseExact(value, Date.Format, null, DateTimeStyles.AssumeLocal));
        }
    }
}
