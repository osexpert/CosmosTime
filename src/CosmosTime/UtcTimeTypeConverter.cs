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
	public class UtcTimeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string str)
			{
				if (UtcTime.TryParse(str, out var ut))
					return ut;

				// TEMPORARY NINJA hack for class StartXxx (these are not updated yet)
//				if (str.Length == "2018-01-03T11:29:21".Length && !str.EndsWith("Z"))
//					if (UtcTime.TryParse(str + ".0000000Z", out var ut2))
//						return ut2;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var utc = (UtcTime)value;
				// Problem: but in cosmos SDK v2 at least. Linq does not generate correct time strings, they are generated variable length.
				//https://stackoverflow.com/questions/63112044/bug-in-datetime-handling-of-cosmos-db-documentclient		 		
				// https://github.com/Azure/azure-cosmos-dotnet-v3/issues/1732
				// Change back to variable length after upgrading to SDK v3 (if it really works there...)
				return utc.ToString(); // variable len
//				return utc.ToCosmosDb();
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
