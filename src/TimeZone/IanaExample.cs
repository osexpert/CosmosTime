using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime.TimeZone
{
	class IanaExample
	{
		public void tets()
        {
            var iana = TimeZoneConverter.TZConvert.WindowsToIana(TimeZoneInfo.Local.Id);
        }
	}
}
