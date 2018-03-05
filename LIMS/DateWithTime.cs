using System;
using System.Globalization;
using System.Web.Mvc;

namespace LIMS
{
    [ModelBinder(typeof(DateWithTimeModelBinder))]
    public struct DateWithTime : IFormattable
    {
        public const string Format = "{0:dd/MM/yyyy h:mm:ss tt}";

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
            return _date.ToString();
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
            return new DateWithTime(DateTimeOffset.ParseExact(result.AttemptedValue, "dd/MM/yyyy h:mm:ss tt", null, DateTimeStyles.AssumeLocal));
        }
    }
}
