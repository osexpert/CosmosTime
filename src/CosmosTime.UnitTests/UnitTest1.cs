using CosmosTime.TimeZone;
using FakeTimeZone;
using Xunit.Sdk;

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

			Assert.Equal(now.Offset, TimeSpan.FromMinutes(utcOffNow.OffsetMinutes));
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


		//[Fact]
		//public void LocalTime_when_local_tz_is_utc()
		//{
		//	using (new FakeLocalTimeZone(TimeZoneInfo.Utc))//.FindSystemTimeZoneById("UTC+12")))
		//	{
		//		//var localDateTime = new DateTime(2020, 12, 31, 23, 59, 59, DateTimeKind.Local);
		//		//var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime);

		//		//Assert.That(TimeZoneInfo.Local.Id, Is.EqualTo("UTC+12")); // ✅ Assertion passes
		//		//Assert.That(localDateTime, Is.EqualTo(utcDateTime.AddHours(12))); // ✅ Assertion passes

		//		var now = LocalTime.Now;
		//		Assert.Equal(DateTimeKind.Local, now.LocalDateTime.Kind);

		//		var dtNow = DateTime.Now;
		//		Assert.Equal(DateTimeKind.Local, dtNow.Kind);

		//	}
		//}

		//[Fact]
		//public void LocalTime_from_various()
		//{
		//	var ltn = DateTime.Now.ToLocalTime2();

		//	var ltutc = DateTime.UtcNow.ToLocalTime2();



		//	var someTime = new DateTime(2000, 3, 4);
		//	Exception e1 = null;
		//	try
		//	{
		//		var ltst = someTime.ToLocalTime2();
		//	}
		//	catch (Exception e)
		//	{
		//		e1 = e;
		//	}
		//	Assert.IsType<ArgumentException>(e1);
		//	Assert.Equal("unspecified kind not allowed", e1.Message);


		//	var localInAsis = new LocalTime(someTime, IanaTimeZone.GetTimeZoneInfo("Asia/Sakhalin"));

		//	using (new FakeLocalTimeZone(IanaTimeZone.GetTimeZoneInfo("Europe/Oslo")))//.FindSystemTimeZoneById("UTC+12")))
		//	{
		//		Assert.Equal(new LocalTime(2000, 3, 3, 15, 0, 0), localInAsis);
		//	}


		//	Assert.Equal(new UtcTime(2000, 3, 3, 14, 0, 0), localInAsis.ToUtcTime());
		//}


		[Fact]
		public void TzTest()
		{
			var iana = TimeZoneConverter.TZConvert.WindowsToIana(TimeZoneInfo.Local.Id);
			var iana2 = IanaTimeZone.GetIanaId(TimeZoneInfo.Local.Id);
			Assert.Equal(iana, iana2);

		}


		[Fact]
		public void UtcTime_Parse()
		{
			var r1 = UtcTime.TryParse("2020-01-20", out var v1);
			Assert.False(r1);
			var r2 = UtcTime.TryParse("2020-01-20Z", out var v2);
			Assert.False(r2); // not iso
							  //			Assert.Equal(new UtcTime(2020, 01, 20), v2);
							  //		Assert.Equal("2020-01-20T00:00:00Z", v2.ToString());

			var r3 = UtcTime.TryParse("2020-01-20T12:13:14", out var v3);
			Assert.False(r3);
			var r4 = UtcTime.TryParse("2020-01-20T12:13:14Z", out var v4);
			Assert.True(r4);
			Assert.Equal(new UtcTime(2020, 01, 20, 12, 13, 14), v4);
			Assert.Equal("2020-01-20T12:13:14Z", v4.ToString());

			var r5 = UtcTime.TryParse("2020-01-20T12:13:14.123", out var v5);
			Assert.False(r5);
			var r6 = UtcTime.TryParse("2020-01-20T12:13:14.123Z", out var v6);
			Assert.True(r6);
			Assert.Equal(new UtcTime(2020, 01, 20, 12, 13, 14, 123), v6);
			Assert.Equal("2020-01-20T12:13:14.123Z", v6.ToString());

			var r7 = UtcTime.TryParse("2020-01-20T12:13:14.123+00:30", out var v7);
			Assert.True(r7);
			Assert.Equal(new UtcTime(2020, 01, 20, 11, 43, 14, 123), v7);
			var r8 = UtcTime.TryParse("2020-01-20T12:13:14.123-00:30", out var v8);
			Assert.True(r8);
			Assert.Equal(new UtcTime(2020, 01, 20, 12, 43, 14, 123), v8);
			Assert.Equal("2020-01-20T12:43:14.123Z", v8.ToString());
		}


		[Fact]
		public void UtcOffsetTime_Parse()
		{

			//t.i

			var r1 = UtcOffsetTime.TryParse("2020-01-20", out var v1);
			Assert.False(r1);
			var r2 = UtcOffsetTime.TryParse("2020-01-20Z", out var v2);
			//Assert.True(r2);
			Assert.False(r2); // not really ISO
							  //			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20), 0), v2);
							  //		Assert.Equal("2020-01-20T00:00:00+00:00", v2.ToString());

			var r3 = UtcOffsetTime.TryParse("2020-01-20T12:13:14", out var v3);
			Assert.False(r3);
			var r4 = UtcOffsetTime.TryParse("2020-01-20T12:13:14Z", out var v4);
			Assert.True(r4);
			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 12, 13, 14), 0), v4);
			Assert.Equal("2020-01-20T12:13:14+00:00", v4.ToString());

			var r5 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123", out var v5);
			Assert.False(r5);
			var r6 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123Z", out var v6);
			Assert.True(r6);
			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 12, 13, 14, 123), 0), v6);
			Assert.Equal("2020-01-20T12:13:14.123+00:00", v6.ToString());

			var r7 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123+00:30", out var v7);
			Assert.True(r7);
			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 11, 43, 14, 123), 30), v7);
			// read as: local time and you get it by adding offset to utc, so take utc + 00:30 = local time
			Assert.Equal("2020-01-20T12:13:14.123+00:30", v7.ToString());
			var r8 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123-00:30", out var v8);
			Assert.True(r8);
			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 12, 43, 14, 123), -30), v8);
			Assert.Equal("2020-01-20T12:13:14.123-00:30", v8.ToString());



			var r9 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123+0030", out var v9);
			Assert.False(r9);
			//			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 11, 43, 14, 123), 30), v9);
			// read as: local time and you get it by adding offset to utc, so take utc + 00:30 = local time
			//		Assert.Equal("2020-01-20T12:13:14.123+00:30", v9.ToString());
			var r10 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123-0030", out var v10);
			Assert.False(r10);
			//Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 12, 43, 14, 123), -30), v10);
			//Assert.Equal("2020-01-20T12:13:14.123-00:30", v10.ToString());


			var r11 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123+01", out var v11);
			Assert.True(r11);
			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 11, 13, 14, 123), 42), v11); // offset does not matter for equality.
																								  // read as: local time and you get it by adding offset to utc, so take utc + 00:30 = local time
			Assert.Equal("2020-01-20T12:13:14.123+01:00", v11.ToString());
			var r12 = UtcOffsetTime.TryParse("2020-01-20T12:13:14.123-01", out var v12);
			Assert.True(r12);
			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 13, 13, 14, 123), -42), v12);  // offset does not matter for equality.
			Assert.Equal("2020-01-20T12:13:14.123-01:00", v12.ToString());


		}

		[Fact]
		public void UtcOffsetTime_Parse_Unspec()
		{
			var r1 = UtcOffsetTime.TryParse("2020-01-20", out var v1, dt => IanaTimeZone.GetTimeZoneInfo("Europe/Berlin"));
			Assert.True(r1);
			Assert.Equal("2020-01-20T00:00:00+01:00", v1.ToString());

			var r2 = UtcOffsetTime.TryParse("2020-01-20", out var v2, dt => IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));
			Assert.True(r2);
			Assert.Equal("2020-01-20T00:00:00+03:00", v2.ToString());
		}

		[Fact]
		public void UtcTime_Parse_Unspec()
		{
			var r1 = UtcTime.TryParse("2020-01-20", out var v1, dt => IanaTimeZone.GetTimeZoneInfo("Europe/Berlin"));
			Assert.True(r1);
			Assert.Equal("2020-01-19T23:00:00Z", v1.ToString());

			var r2 = UtcTime.TryParse("2020-01-20", out var v2, dt => IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));
			Assert.True(r2);
			Assert.Equal("2020-01-19T21:00:00Z", v2.ToString());
		}

		[Fact]
		public void UtcTime_ParseCosmosDb()
		{
			var ok = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.1233333Z");
			Assert.Equal("2020-01-20T12:13:14.1233333Z", ok.ToString());
			Assert.Equal("2020-01-20T12:13:14.1233333Z", ok.ToCosmosDb());

			var ok2 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.0000000Z");
			Assert.Equal("2020-01-20T12:13:14Z", ok2.ToString());
			Assert.Equal("2020-01-20T12:13:14.0000000Z", ok2.ToCosmosDb());

			Assert.Throws<FormatException>(() =>
			{
				var nok1 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.1233333");
			});

			Assert.Throws<FormatException>(() =>
			{
				var nok2 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.123333Z");
			});


			Assert.Throws<FormatException>(() =>
			{
				var nok3 = UtcTime.ParseCosmosDb("2020-99-20T12:13:14.1233333Z");
			});
			Assert.Throws<FormatException>(() =>
			{
				var nok3 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.12333330");
			});
		}

		[Fact]
		public void UtcOffsetTime_ToString()
		{
			var offT = new UtcOffsetTime(new UtcTime(2020, 3, 15), 10);
			Assert.Equal("2020-03-15T00:10:00+00:10", offT.ToString());

			var offT2 = new UtcOffsetTime(new UtcTime(2020, 3, 15, 0, 0, 0, 123), 10);
			Assert.Equal("2020-03-15T00:10:00.123+00:10", offT2.ToString());
		}

		[Fact]
		public void UtcOffsetTime_Now()
		{
			var un = UtcOffsetTime.UtcNow;
			var ln = UtcOffsetTime.LocalNow;
			var naa = UtcOffsetTime.Now(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));
			var localN = UtcOffsetTime.Now(TimeZoneInfo.Local);
			var utcN = UtcOffsetTime.Now(TimeZoneInfo.Utc);
			// TODO: test something
		}

		[Fact]
		public void IsoWeek_GetWeeksInRangeIterator()
		{
			var res = IsoWeek.GetWeeksInRangeIterator(new DateTime(2020, 1, 10), new DateTime(2021, 5, 5));
			Assert.Equal(70, res.Count());
			Assert.Equal("2020-W02", res.First().ToString());
			Assert.Equal("2021-W18", res.Last().ToString());

			var res2 = IsoWeek.GetWeeksInYearIterator(2020);
			Assert.Equal(53, res2.Count());
			Assert.Equal("2020-W01", res2.First().ToString());
			Assert.Equal("2020-W53", res2.Last().ToString());

		}

		[Fact]
		public void ZonedOffsetTime_Misc()
		{
			var now = ZonedOffsetTime.Now(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));

			var now2 = ZonedOffsetTime.Now(IanaTimeZone.GetTimeZoneInfo("Europe/Oslo"));

			Assert.Throws<ArgumentException>(() =>
			{
				try
				{
					var zof = new ZonedOffsetTime(new ZonedTime(2020, 1, 20, 4, 5, 6, 7, IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa")), 42);
				}
				catch (ArgumentException e)
				{
					Assert.Equal("Offset is not valid in zone", e.Message);
					throw;
				}
			});

			var zof2 = new ZonedOffsetTime(new ZonedTime(2020, 1, 20, 4, 5, 6, 7, IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa")), 180);

			Assert.Equal("2020-01-20T04:05:06.007+03:00[Africa/Nairobi]", zof2.ToString());
			var dto_utfo = zof2.ToDateTimeOffset().ToUtcOffsetTime();
			Assert.Equal("2020-01-20T04:05:06.007+03:00", dto_utfo.ToString());
			var utfo = zof2.ToUtcOffsetTime();
			Assert.Equal("2020-01-20T04:05:06.007+03:00", utfo.ToString());
		}
	}
}