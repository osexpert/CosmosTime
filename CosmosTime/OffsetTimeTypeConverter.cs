﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace CosmosTime
{
    /// <summary>
    /// Format\parse variable length format
    /// </summary>
    public class OffsetTimeTypeConverter : TypeConverter
    {
        /// <inheritdoc />
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(string);

        /// <inheritdoc />
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string utcOffsetString)
            {
                if (OffsetTime.TryParse(utcOffsetString, out var uo))
                    return uo;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <inheritdoc />
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var u = (OffsetTime)value;
                return u.ToString(); // variable length
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
