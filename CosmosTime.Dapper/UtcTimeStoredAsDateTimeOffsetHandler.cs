using System;
using System.Data;
using Dapper;

namespace CosmosTime.Dapper
{
    /// <summary>
    /// How to use: add to top of Program.cs:
    /// 
    /// https://stackoverflow.com/questions/12510299/get-datetime-as-utc-with-dapper
    /// 
    /// SqlMapper.AddTypeHandler(typeof(UtcTime), new UtcTimeStoredAsDateTimeOffsetHandler());
    /// </summary>
    public class UtcTimeStoredAsDateTimeOffsetHandler : SqlMapper.TypeHandler<UtcTime>
    {
        /// <inheritdoc/>
        public override void SetValue(IDbDataParameter parameter, UtcTime value)
        {
            parameter.Value = new DateTimeOffset(value.UtcDateTime);
        }

        /// <inheritdoc/>
        public override UtcTime Parse(object value)
        {
            var dto = (DateTimeOffset)value;
            return UtcTime.FromUtcDateTime(dto.UtcDateTime);
        }
    }
}
