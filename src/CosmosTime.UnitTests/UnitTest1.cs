using CosmosTime.TimeZone;
using FakeTimeZone;

namespace CosmosTime.UnitTests
{
	public class UnitTest1
	{
		[Fact]
		public void UtcOffsetTime_roundtrip()
		{
			var now = DateTimeOffset.Now;
			var utcOffNow = now.ToUtcOffsetTime();
			var nowDto = utcOffNow.ToDateTimeOffset();

			Assert.Equal(now, nowDto);
			Assert.Equal(now.Offset, nowDto.Offset);
			Assert.Equal(now.LocalDateTime, nowDto.LocalDateTime);
			Assert.Equal(now.UtcDateTime, nowDto.UtcDateTime);

			Assert.Equal(now.Offset, TimeSpan.FromMinutes(utcOffNow.OffsetMins));
			Assert.Equal(now.Offset, utcOffNow.Offset);
			//Assert.Equal(now.UtcDateTime, utcOffNow.);
		}

		[Fact]
		public void UtcTime_Now_roundtrip_DateTime_Now()
		{
			var n = DateTime.Now;
			var utcTime = n.ToUtcTime();
			var udt = utcTime.UtcDateTime.ToLocalTime();
			Assert.Equal(n, udt);
		}

		[Fact]
		public void UtcTime_Now_roundtrip_DateTime_UtcNow()
		{
			var n = DateTime.UtcNow;
			var utcTime = n.ToUtcTime();
			var udt = utcTime.UtcDateTime;
			Assert.Equal(n, udt);
		}


		[Fact]
		public void LocalTime_when_local_tz_is_utc()
		{
			using (new FakeLocalTimeZone(TimeZoneInfo.Utc))//.FindSystemTimeZoneById("UTC+12")))
			{
				//var localDateTime = new DateTime(2020, 12, 31, 23, 59, 59, DateTimeKind.Local);
				//var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime);

				//Assert.That(TimeZoneInfo.Local.Id, Is.EqualTo("UTC+12")); // ✅ Assertion passes
				//Assert.That(localDateTime, Is.EqualTo(utcDateTime.AddHours(12))); // ✅ Assertion passes

				var now = LocalTime.Now;
				Assert.Equal(DateTimeKind.Local, now.LocalDateTime.Kind);

				var dtNow = DateTime.Now;
				Assert.Equal(DateTimeKind.Local, dtNow.Kind);

			}
		}
		[Fact]
		public void LocalTime_from_various()
		{
			var ltn = DateTime.Now.ToLocalTime2();

			var ltutc = DateTime.UtcNow.ToLocalTime2();



			var someTime = new DateTime(2000, 3, 4);
			Exception e1 = null;
			try
			{
				var ltst = someTime.ToLocalTime2();
			}
			catch (Exception e)
			{
				e1 = e;
			}
			Assert.IsType<ArgumentException>(e1);
			Assert.Equal("unspecified kind not allowed", e1.Message);


			var localInAsis = new LocalTime(someTime, IanaTimeZone.GetTimeZoneInfo("Asia/Sakhalin"));

			using (new FakeLocalTimeZone(IanaTimeZone.GetTimeZoneInfo("Europe/Oslo")))//.FindSystemTimeZoneById("UTC+12")))
			{
				Assert.Equal(new LocalTime(2000, 3, 3, 15, 0, 0), localInAsis);
			}


			Assert.Equal(new UtcTime(2000, 3, 3, 14, 0, 0), localInAsis.ToUtcTime());
		}


		[Fact]
		public void TzTest()
		{
			var iana = TimeZoneConverter.TZConvert.WindowsToIana(TimeZoneInfo.Local.Id);
			var iana2 = IanaTimeZone.GetIanaId(TimeZoneInfo.Local.Id);
			Assert.Equal(iana, iana2);

		}


	}
}