using System;
using System.Globalization;
using System.Web.Mvc;

namespace LIMS
{
    [ModelBinder(typeof(DateModelBinder))]
    public struct Date : IFormattable
    {
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
            return _date.ToString();
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
            return new Date(DateTimeOffset.ParseExact(result.AttemptedValue, "dd/MM/yyyy", null, DateTimeStyles.AssumeLocal));
        }
    }
}
