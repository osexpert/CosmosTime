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
	/// SqlMapper.AddTypeHandler(typeof(UtcTime), new UtcOffsetTimeStoredAsDateTimeOffsetHandler());
	/// </summary>
	public class OffsetTimeStoredAsDateTimeOffsetHandler : SqlMapper.TypeHandler<OffsetTime>
	{
		/// <inheritdoc/>
		public override void SetValue(IDbDataParameter parameter, OffsetTime value)
		{
			parameter.Value = value.ToDateTimeOffset();
		}

		/// <inheritdoc/>
		public override OffsetTime Parse(object value)
		{
			var dto = (DateTimeOffset)value;
			return dto.ToOffsetTime();
		}
	}

}
