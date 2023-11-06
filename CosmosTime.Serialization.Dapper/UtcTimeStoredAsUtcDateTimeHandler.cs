using Dapper;
using System;
using System.Data;

namespace CosmosTime.Serialization.Dapper
{
	/// <summary>
	/// How to use: add to top of Program.cs:
	/// 
	/// https://stackoverflow.com/questions/12510299/get-datetime-as-utc-with-dapper
	/// 
	/// SqlMapper.AddTypeHandler(typeof(UtcTime), new UtcTimeStoredAsDateTimeInUtcHandler());
	/// </summary>
	public class UtcTimeStoredAsUtcDateTimeHandler : SqlMapper.TypeHandler<UtcTime>
	{
		/// <inheritdoc/>
		public override void SetValue(IDbDataParameter parameter, UtcTime value)
		{
			parameter.Value = value.UtcDateTime;
		}

		/// <inheritdoc/>
		public override UtcTime Parse(object value)
		{
			var dt = (DateTime)value;
			if (dt.Kind != DateTimeKind.Unspecified)
				throw new Exception("impossible: kind is not unspecified");

			return UtcTime.FromUtcDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc));
			//return UtcTime.FromUnspecifiedDateTime(dt, TimeSpan.Zero);
			//return UtcTime.FromUnspecifiedDateTime(dt, TimeZoneInfo.Utc);
		}
	}

}
