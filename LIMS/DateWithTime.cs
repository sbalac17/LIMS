using System;
using System.Globalization;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace LIMS
{
    [ModelBinder(typeof(DateWithTimeModelBinder))]
    [JsonConverter(typeof(DateWithTimeSerializer))]
    public struct DateWithTime : IFormattable
    {
        public const string Format = "dd/MM/yyyy h:mm:ss tt";
        public const string Placeholder = "{0:" + Format + "}";

        private readonly DateTimeOffset _date;

        public DateWithTime(DateTimeOffset date)
        {
            _date = date;
        }

        public static implicit operator DateWithTime(DateTimeOffset date)
        {
            return new DateWithTime(date);
        }

        public static implicit operator DateTimeOffset(DateWithTime date)
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
    /// Binds <see cref="DateWithTime"/> in format <c>DD/MM/YYYY HH:MM:SS PM</c>.
    /// </summary>
    public class DateWithTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return new DateWithTime(DateTimeOffset.ParseExact(result.AttemptedValue, DateWithTime.Format, null, DateTimeStyles.AssumeLocal));
        }
    }

    public class DateWithTimeSerializer : JsonConverter<DateWithTime>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, DateWithTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(DateWithTime.Format, CultureInfo.InvariantCulture));
        }

        public override DateWithTime ReadJson(JsonReader reader, Type objectType, DateWithTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new Exception("DateWithTime value must be a string.");

            var value = reader.Value.ToString();
            return new DateWithTime(DateTimeOffset.ParseExact(value, DateWithTime.Format, null, DateTimeStyles.AssumeLocal));
        }
    }
}
