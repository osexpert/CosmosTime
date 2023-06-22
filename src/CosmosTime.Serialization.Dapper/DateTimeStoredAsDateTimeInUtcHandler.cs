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
	/// SqlMapper.RemoveTypeMap(typeof(DateTime));
	/// SqlMapper.AddTypeHandler(typeof(DateTime), new DateTimeStoredAsDateTimeInUtcHandler());
	/// </summary>
	public class DateTimeStoredAsDateTimeInUtcHandler : SqlMapper.TypeHandler<DateTime>
	{
		public override void SetValue(IDbDataParameter parameter, DateTime value)
		{
			// deny unspec time, we don't know what it is?
			if (value.Kind == DateTimeKind.Unspecified)
				throw new Exception("unspecified Kind not allowed");

			// since Utc|Local, ToUniversalTime is predictable.
			parameter.Value = value.ToUniversalTime();
		}

		public override DateTime Parse(object value)
		{
			var dt = (DateTime)value;
			if (dt.Kind != DateTimeKind.Unspecified)
				throw new Exception("impossible: kind is not unspecified");
			return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
		}
	}

}
