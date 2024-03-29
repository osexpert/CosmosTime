﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace CosmosTime
{
    /// <summary>
    /// Format\parse variable length format
    /// </summary>
    public class UtcTimeTypeConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                if (UtcTime.TryParse(str, out var ut))
                    return ut;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var utc = (UtcTime)value;
                return utc.ToString(); // variable len
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
