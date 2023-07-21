using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// Format\parse variable length format
	/// </summary>
	public class ZoneTimeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string str)
			{
				if (ZoneTime.TryParse(str, out var ut))
					return ut;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var utc = (ZoneTime)value;
				return utc.ToString(); // variable len millis
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
