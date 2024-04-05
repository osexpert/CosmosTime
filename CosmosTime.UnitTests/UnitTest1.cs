/* Unmerged change from project 'CosmosTime.UnitTests (net4.8)'
Before:
using System.Globalization;
using System.Security.Cryptography;
using System.Xml.Linq;
After:
using CosmosTime.TimeZone;
using FakeTimeZone;
using NodaTime;
*/

using CosmosTime.TimeZone;
using FakeTimeZone;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime;
using NodaTime.Text;

namespace CosmosTime.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        public UnitTest1()
        {
            // warmup
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));
            var s1 = beforeTransition.ToString();
            var s2 = beforeTransition.Date.ToString();
        }

        [TestMethod]
        public void UtcOffsetTime_roundtrip()
        {
            var now = DateTimeOffset.Now;
            var utcOffNow = now.ToOffsetTime();
            var nowDto = utcOffNow.ToDateTimeOffset();

            Assert.AreEqual(now, nowDto);
            Assert.AreEqual(now.Offset, nowDto.Offset);
            Assert.AreEqual(now.LocalDateTime, nowDto.LocalDateTime);
            Assert.AreEqual(now.UtcDateTime, nowDto.UtcDateTime);

            Assert.AreEqual(now.Offset, utcOffNow.Offset);
            Assert.AreEqual(now.Offset, utcOffNow.Offset);
            //Assert.Equal(now.UtcDateTime, utcOffNow.);
        }

        [TestMethod]
        public void UtcTime_Now_roundtrip_DateTime_Now()
        {
            var n = DateTime.Now;
            var utcTime = UtcTime.FromLocalDateTime(n);
            var udt = utcTime.UtcDateTime.ToLocalTime();
            Assert.AreEqual(n, udt);
        }

        [TestMethod]
        public void UtcTime_Now_roundtrip_DateTime_UtcNow()
        {
            var n = DateTime.UtcNow;
            var utcTime = UtcTime.FromUtcDateTime(n);
            var udt = utcTime.UtcDateTime;
            Assert.AreEqual(n, udt);
        }


        //[TestMethod]
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

        //[TestMethod]
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


        [TestMethod]
        public void TzTest()
        {
            var iana = TimeZoneConverter.TZConvert.WindowsToIana(TimeZoneInfo.Local.Id);
            var iana2 = IanaTimeZone.GetIanaId(TimeZoneInfo.Local.Id);
            Assert.AreEqual(iana, iana2);

        }


        [TestMethod]
        public void UtcTime_Parse()
        {
            var r1 = UtcTime.TryParse("2020-01-20", out var v1);
            Assert.IsFalse(r1);
            var r2 = UtcTime.TryParse("2020-01-20Z", out var v2);
            Assert.IsFalse(r2); // not iso
                              //			Assert.Equal(new UtcTime(2020, 01, 20), v2);
                              //		Assert.Equal("2020-01-20T00:00:00Z", v2.ToString());

            var r3 = UtcTime.TryParse("2020-01-20T12:13:14", out var v3);
            Assert.IsFalse(r3);
            var r4 = UtcTime.TryParse("2020-01-20T12:13:14Z", out var v4);
            Assert.IsTrue(r4);
            Assert.AreEqual(new UtcTime(2020, 01, 20, 12, 13, 14), v4);
            Assert.AreEqual("2020-01-20T12:13:14Z", v4.ToString());

            var r5 = UtcTime.TryParse("2020-01-20T12:13:14.123", out var v5);
            Assert.IsFalse(r5);
            var r6 = UtcTime.TryParse("2020-01-20T12:13:14.123Z", out var v6);
            Assert.IsTrue(r6);
            Assert.AreEqual(new UtcTime(2020, 01, 20, 12, 13, 14, 123), v6);
            Assert.AreEqual("2020-01-20T12:13:14.123Z", v6.ToString());

            var r7 = UtcTime.TryParse("2020-01-20T12:13:14.123+00:30", out var v7);
            Assert.IsTrue(r7);
            Assert.AreEqual(new UtcTime(2020, 01, 20, 11, 43, 14, 123), v7);
            var r8 = UtcTime.TryParse("2020-01-20T12:13:14.123-00:30", out var v8);
            Assert.IsTrue(r8);
            Assert.AreEqual(new UtcTime(2020, 01, 20, 12, 43, 14, 123), v8);
            Assert.AreEqual("2020-01-20T12:43:14.123Z", v8.ToString());
        }


        [TestMethod]
        public void UtcOffsetTime_Parse()
        {

            //t.i

            var r1 = OffsetTime.TryParse("2020-01-20", out var v1);
            Assert.IsFalse(r1);
            var r2 = OffsetTime.TryParse("2020-01-20Z", out var v2);
            //Assert.True(r2);
            Assert.IsFalse(r2); // not really ISO
                              //			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20), 0), v2);
                              //		Assert.Equal("2020-01-20T00:00:00+00:00", v2.ToString());

            var r3 = OffsetTime.TryParse("2020-01-20T12:13:14", out var v3);
            Assert.IsFalse(r3);
            var r4 = OffsetTime.TryParse("2020-01-20T12:13:14Z", out var v4);
            Assert.IsTrue(r4);
            Assert.AreEqual(new OffsetTime(new UtcTime(2020, 01, 20, 12, 13, 14), TimeSpan.FromMinutes(0)), v4);
            Assert.AreEqual("2020-01-20T12:13:14+00:00", v4.ToString());

            var r5 = OffsetTime.TryParse("2020-01-20T12:13:14.123", out var v5);
            Assert.IsFalse(r5);
            var r6 = OffsetTime.TryParse("2020-01-20T12:13:14.123Z", out var v6);
            Assert.IsTrue(r6);
            Assert.AreEqual(new OffsetTime(new UtcTime(2020, 01, 20, 12, 13, 14, 123), TimeSpan.FromMinutes(0)), v6);
            Assert.AreEqual("2020-01-20T12:13:14.123+00:00", v6.ToString());

            var r7 = OffsetTime.TryParse("2020-01-20T12:13:14.123+00:30", out var v7);
            Assert.IsTrue(r7);
            Assert.AreEqual(new OffsetTime(new UtcTime(2020, 01, 20, 11, 43, 14, 123), TimeSpan.FromMinutes(30)), v7);
            // read as: local time and you get it by adding offset to utc, so take utc + 00:30 = local time
            Assert.AreEqual("2020-01-20T12:13:14.123+00:30", v7.ToString());
            var r8 = OffsetTime.TryParse("2020-01-20T12:13:14.123-00:30", out var v8);
            Assert.IsTrue(r8);
            Assert.AreEqual(new OffsetTime(new UtcTime(2020, 01, 20, 12, 43, 14, 123), TimeSpan.FromMinutes(-30)), v8);
            Assert.AreEqual("2020-01-20T12:13:14.123-00:30", v8.ToString());



            var r9 = OffsetTime.TryParse("2020-01-20T12:13:14.123+0030", out var v9);
            Assert.IsFalse(r9);
            //			Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 11, 43, 14, 123), 30), v9);
            // read as: local time and you get it by adding offset to utc, so take utc + 00:30 = local time
            //		Assert.Equal("2020-01-20T12:13:14.123+00:30", v9.ToString());
            var r10 = OffsetTime.TryParse("2020-01-20T12:13:14.123-0030", out var v10);
            Assert.IsFalse(r10);
            //Assert.Equal(new UtcOffsetTime(new UtcTime(2020, 01, 20, 12, 43, 14, 123), -30), v10);
            //Assert.Equal("2020-01-20T12:13:14.123-00:30", v10.ToString());


            var r11 = OffsetTime.TryParse("2020-01-20T12:13:14.123+01", out var v11);
            Assert.IsTrue(r11);
            Assert.AreEqual(new OffsetTime(new UtcTime(2020, 01, 20, 11, 13, 14, 123), TimeSpan.FromMinutes(42)), v11); // offset does not matter for equality.
                                                                                                                     // read as: local time and you get it by adding offset to utc, so take utc + 00:30 = local time
            Assert.AreEqual("2020-01-20T12:13:14.123+01:00", v11.ToString());
            var r12 = OffsetTime.TryParse("2020-01-20T12:13:14.123-01", out var v12);
            Assert.IsTrue(r12);
            Assert.AreEqual(new OffsetTime(new UtcTime(2020, 01, 20, 13, 13, 14, 123), TimeSpan.FromMinutes(-42)), v12);  // offset does not matter for equality.
            Assert.AreEqual("2020-01-20T12:13:14.123-01:00", v12.ToString());


        }

        [TestMethod]
        public void UtcOffsetTime_Parse_Unspec()
        {
            var r1 = OffsetTime.TryParse("2020-01-20", out var v1, dto => IanaTimeZone.GetTimeZoneInfo("Europe/Berlin").GetUtcOffset(dto));
            Assert.IsTrue(r1);
            Assert.AreEqual("2020-01-20T00:00:00+01:00", v1.ToString());

            var r2 = OffsetTime.TryParse("2020-01-20", out var v2, dto => IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa").GetUtcOffset(dto));
            Assert.IsTrue(r2);
            Assert.AreEqual("2020-01-20T00:00:00+03:00", v2.ToString());
        }

        [TestMethod]
        public void UtcTime_Parse_Unspec()
        {
            var r1 = UtcTime.TryParse("2020-01-20", out var v1, dto => IanaTimeZone.GetTimeZoneInfo("Europe/Berlin").GetUtcOffset(dto));
            Assert.IsTrue(r1);
            Assert.AreEqual("2020-01-19T23:00:00Z", v1.ToString());

            var r2 = UtcTime.TryParse("2020-01-20", out var v2, dto => IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa").GetUtcOffset(dto));
            Assert.IsTrue(r2);
            Assert.AreEqual("2020-01-19T21:00:00Z", v2.ToString());
        }

        [TestMethod]
        public void UtcTime_ParseCosmosDb()
        {
            var ok = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.1233333Z");
            Assert.AreEqual("2020-01-20T12:13:14.1233333Z", ok.ToString());
            Assert.AreEqual("2020-01-20T12:13:14.1233333Z", ok.ToCosmosDb());

            var ok2 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.0000000Z");
            Assert.AreEqual("2020-01-20T12:13:14Z", ok2.ToString());
            Assert.AreEqual("2020-01-20T12:13:14.0000000Z", ok2.ToCosmosDb());

            Assert.ThrowsException<FormatException>(() =>
            {
                var nok1 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.1233333");
            });

            Assert.ThrowsException<FormatException>(() =>
            {
                var nok2 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.123333Z");
            });


            Assert.ThrowsException<FormatException>(() =>
            {
                var nok3 = UtcTime.ParseCosmosDb("2020-99-20T12:13:14.1233333Z");
            });
            Assert.ThrowsException<FormatException>(() =>
            {
                var nok3 = UtcTime.ParseCosmosDb("2020-01-20T12:13:14.12333330");
            });
        }

        [TestMethod]
        public void UtcOffsetTime_ToString()
        {
            var offT = new OffsetTime(new UtcTime(2020, 3, 15), TimeSpan.FromMinutes(10));
            Assert.AreEqual("2020-03-15T00:10:00+00:10", offT.ToString());

            var offT2 = new OffsetTime(new UtcTime(2020, 3, 15, 0, 0, 0, 123), TimeSpan.FromMinutes(10));
            Assert.AreEqual("2020-03-15T00:10:00.123+00:10", offT2.ToString());
        }

        [TestMethod]
        public void UtcOffsetTime_Now()
        {
            var un = OffsetTime.UtcNow;
            var ln = OffsetTime.LocalNow;
            var naa = OffsetTime.Now(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));
            var localN = OffsetTime.Now(TimeZoneInfo.Local);
            var utcN = OffsetTime.Now(TimeZoneInfo.Utc);

            //var zun = ZonedTime.UtcNow;
            //var zln = ZonedTime.LocalNow;

            var zoun = ZoneTime.UtcNow;
            var zoln = ZoneTime.LocalNow;



            var uozt_un = ZoneTime.UtcNow;
            var uozt_ln = ZoneTime.LocalNow;
            var uozt_aa = ZoneTime.Now(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));


            var ctLocalNow = ClockTime.LocalNow;
            var ctUtcNow = ClockTime.UtcNow;
            var ctAAow = ClockTime.Now(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));

            // TODO: test something
        }

        [TestMethod]
        public void IsoWeek_GetWeeksInRangeIterator()
        {
            var res = IsoWeek.GetWeeksInRangeIterator(new DateTime(2020, 1, 10), new DateTime(2021, 5, 5));
            Assert.AreEqual(70, res.Count());
            Assert.AreEqual("2020-W02", res.First().ToString());
            Assert.AreEqual("2021-W18", res.Last().ToString());

            var res2 = IsoWeek.GetWeeksInYearIterator(2020);
            Assert.AreEqual(53, res2.Count());
            Assert.AreEqual("2020-W01", res2.First().ToString());
            Assert.AreEqual("2020-W53", res2.Last().ToString());

        }

        [TestMethod]
        public void ZonedOffsetTime_Misc()
        {
            var now = ZoneTime.Now(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"));

            var now2 = ZoneTime.Now(IanaTimeZone.GetTimeZoneInfo("Europe/Oslo"));

            var ae = Assert.ThrowsException<ArgumentException>(() =>
            {
                var zof = new ZoneTime(new ClockTime(2020, 1, 20, 4, 5, 6, 7), IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"), TimeSpan.FromMinutes(42));
            });
            Assert.AreEqual("Offset is not valid in zone", ae.Message);

            var zof2 = new ZoneTime(2020, 1, 20, 4, 5, 6, 7, IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"), TimeSpan.FromMinutes(180));
            var zof2_ = new ClockTime(2020, 1, 20, 4, 5, 6, 7).ToZoneTime(IanaTimeZone.GetTimeZoneInfo("Africa/Addis_Ababa"), TimeSpan.FromMinutes(180));
            Assert.AreEqual(zof2, zof2_);

            Assert.AreEqual($"2020-01-20T04:05:06.007+03:00[Africa/Addis_Ababa]", zof2.ToString());
            var dto_utfo = zof2.OffsetTime.ToDateTimeOffset().ToOffsetTime();
            Assert.AreEqual("2020-01-20T04:05:06.007+03:00", dto_utfo.ToString());
            var utfo = zof2.OffsetTime;
            Assert.AreEqual("2020-01-20T04:05:06.007+03:00", utfo.ToString());

            var n = DateTime.Now;
            //var z = new UtcOffsetZoneTime(n.ToUtcZoneTime());
            //var z = new UtcOffsetZoneTime(n.ToUtcTime().ToUtcOffsetTime()
            var z = ZoneTime.FromLocalDateTime(n);//.ToZoneTime();
            Assert.AreEqual(n.Ticks, z.Ticks);
        }

        [TestMethod]
        public void TimeZones_WindowsToIana()
        {
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                if (IanaTimeZone.TryGetIanaId(tz, out var ianaid))
                {
                    if (tz.HasIanaId())
                    {
                        Assert.AreEqual(tz.Id, ianaid);
                    }

                    var reverse = IanaTimeZone.GetTimeZoneInfo(ianaid);

                    Assert.AreEqual(reverse.Id, ianaid);

                    Assert.IsTrue(reverse.HasIanaId());
                }
                else if (tz.Id == "Mid-Atlantic Standard Time")
                {
                    Assert.IsFalse(tz.HasIanaId());
                }
                else if (tz.Id == "Kamchatka Standard Time")
                {
                    Assert.IsFalse(tz.HasIanaId());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        [TestMethod]
        public void TimeZones_Iana()
        {
            var ianaIds = IanaTimeZone.GetIanaIds().ToList();
            foreach (var ianaId in ianaIds)
            {
                var tz = IanaTimeZone.GetTimeZoneInfo(ianaId);
                Assert.IsNotNull(tz);

                var winid = IanaTimeZone.GetWindowsId(ianaId);
                Assert.IsNotNull(winid);
            }

            var winids = IanaTimeZone.GetWindowsIds().ToList();
            foreach (var wid in winids)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(wid);
                Assert.IsNotNull(tz);

                var iana = IanaTimeZone.GetIanaId(wid);
                Assert.IsNotNull(iana);
            }
        }

        //[TestMethod]
        //public void TimeZones_Mssql()
        //{
        //	var ids = MsSqlTimeZone.GetIds().ToList();
        //	foreach (var id in ids)
        //	{
        //		var tz = TimeZoneInfo.FindSystemTimeZoneById(id);
        //		Assert.NotNull(tz);

        //		var name = MsSqlTimeZone.GetNameFromId(id);

        //		Assert.Equal(name, tz.DisplayName);

        //		var id_rev = MsSqlTimeZone.GetIdFromName(name);
        //		Assert.Equal(id, id_rev);
        //	}

        //	var names = MsSqlTimeZone.GetNames().ToList();
        //	foreach (var name in names)
        //	{
        //		var id = MsSqlTimeZone.GetIdFromName(name);
        //		var name_rev = MsSqlTimeZone.GetNameFromId(id);
        //		Assert.Equal(name, name_rev);
        //	}
        //}

        [TestMethod]
        public void UtcOffsetZoneTime_MissingIana()
        {


            //			Assert.ThrowsException<TimeZoneNotFoundException>(() =>
            //			{
            var z = ZoneTime.Now(TimeZoneInfo.FindSystemTimeZoneById("Kamchatka Standard Time"));
            Assert.IsTrue(z.ToString().EndsWith("[Windows/Kamchatka Standard Time]"));
            //		});


            //			Assert.EndsWith("[Windows/Kamchatka Standard Time]", z.ToString());


        }

        [TestMethod]
        public void UtcOffsetZoneTime_Parse2()
        {
            // missing zone
            Assert.IsFalse(ZoneTime.TryParse("2020-01-20T04:05:06.007+03:00", out var _));
            Assert.IsFalse(ZoneTime.TryParse("2020-01-20T04:05:06.007", out var _));
            Assert.IsFalse(ZoneTime.TryParse("2020-01-20T04:05:06.007Z", out var _));

            // this make little sense. it quickly becomes mismatch here. ZonedOffsetTime will have validation for this,
            // so ZonedTime can probably parse via ZonedOffsetTime and discard the offset.

            var b1 = ZoneTime.TryParse("2020-01-20T04:05:06.007+03:00[Europe/Oslo]", out var p1);
            Assert.IsFalse(b1); // wrong offset

            var b1_ok = ZoneTime.TryParse("2020-01-20T04:05:06.007+01:00[Europe/Oslo]", out var p1_ok);
            Assert.IsTrue(b1_ok); // ok offset
            Assert.AreEqual($"2020-01-20T04:05:06.007+01:00[Europe/Oslo]", p1_ok.ToString());

            var b2 = ZoneTime.TryParse("2020-01-20T04:05:06.007[Europe/Oslo]", out var p2);
            Assert.IsTrue(b2);
            Assert.AreEqual($"2020-01-20T04:05:06.007+01:00[Europe/Oslo]", p2.ToString());

            var b3 = ZoneTime.TryParse("2020-01-20T04:05:06.007Z[Europe/Oslo]", out var p3);
            Assert.IsTrue(b3); // Z and tz conflict. Update: spec allows this  // https://datatracker.ietf.org/doc/draft-ietf-sedate-datetime-extended/

            var b4 = ZoneTime.TryParse("2020-01-20T04:05:06.007Z[Etc/UTC]", out var p4);
            Assert.IsTrue(b4); // Z and tz match
            Assert.AreEqual("2020-01-20T04:05:06.007Z[Etc/UTC]", p4.ToString());
        }


        [TestMethod]
        public void UtcOffsetZoneTime_Parse()
        {
            // missing zone
            Assert.IsFalse(ZoneTime.TryParse("2020-01-20T04:05:06.007+03:00", out var _));
            Assert.IsFalse(ZoneTime.TryParse("2020-01-20T04:05:06.007", out var _));
            Assert.IsFalse(ZoneTime.TryParse("2020-01-20T04:05:06.007Z", out var _));

            // this make little sense. it quickly becomes mismatch here. ZonedOffsetTime will have validation for this,
            // so ZonedTime can probably parse via ZonedOffsetTime and discard the offset.

            var b1 = ZoneTime.TryParse("2020-01-20T04:05:06.007+03:00[Europe/Oslo]", out var p1);
            Assert.IsFalse(b1); // wrong offset

            var b1_ok = ZoneTime.TryParse("2020-01-20T04:05:06.007+01:00[Europe/Oslo]", out var p1_ok);
            Assert.IsTrue(b1_ok); // ok offset
            Assert.AreEqual($"2020-01-20T04:05:06.007+01:00[Europe/Oslo]", p1_ok.ToString());

            var b2 = ZoneTime.TryParse("2020-01-20T04:05:06.007[Europe/Oslo]", out var p2);
            Assert.IsTrue(b2);
            Assert.AreEqual($"2020-01-20T04:05:06.007+01:00[Europe/Oslo]", p2.ToString());

            var b3 = ZoneTime.TryParse("2020-01-20T04:05:06.007Z[Europe/Oslo]", out var p3);
            Assert.IsTrue(b3); // Z and tz conflict. Update: spec allows this...  // https://datatracker.ietf.org/doc/draft-ietf-sedate-datetime-extended/

            var b4 = ZoneTime.TryParse("2020-01-20T04:05:06.007Z[UTC]", out var p4);
            Assert.IsTrue(b4); // 
            Assert.AreEqual("2020-01-20T04:05:06.007Z[UTC]", p4.ToString());

            var b5 = ZoneTime.TryParse("2020-01-20T04:05:06.007+00:00[UTC]", out var p5);
            Assert.IsTrue(b5); // +00:00 ok
            Assert.AreEqual("2020-01-20T04:05:06.007Z[UTC]", p5.ToString());

            var b6 = ZoneTime.TryParse("2020-01-20T04:05:06.007-00:00[UTC]", out var p6);
            Assert.IsTrue(b6); // -00:00 ok
            Assert.AreEqual("2020-01-20T04:05:06.007Z[UTC]", p6.ToString());

            var b7 = ZoneTime.TryParse("2020-01-20T04:05:06.007[UTC]", out var p7);
            Assert.IsTrue(b7); // ok
            Assert.AreEqual("2020-01-20T04:05:06.007Z[UTC]", p7.ToString());
        }

        /// <summary>
        /// Based on https://nodatime.org/3.1.x/api/NodaTime.ZonedDateTime.html
        /// Add(ZonedDateTime, Duration)
        /// </summary>
        [TestMethod]
        public void ZonedTime_Pass_dst_Transition()
        {
            var z = new ZoneTime(new ClockTime(2017, 10, 29, 1, 45, 0), IanaTimeZone.GetTimeZoneInfo("Europe/Dublin"), TimeSpan.FromHours(1));
            Assert.AreEqual($"2017-10-29T01:45:00+01:00[Europe/Dublin]", z.ToString());

            var zAfter = z.AddUtc(TimeSpan.FromHours(1));
            // passed DST, same time, offset changed
            Assert.AreEqual($"2017-10-29T01:45:00+00:00[Europe/Dublin]", zAfter.ToString());

            // pretty weird, but this time is ambigous, so standard time offset is chosen.
            var zBack = zAfter.SubtractUtc(TimeSpan.FromHours(1));
            Assert.AreEqual($"2017-10-29T01:45:00+01:00[Europe/Dublin]", zBack.ToString());

        }

        [TestMethod]
        public void ZonedTime_Pass_dst_Transition_compare_to_Noda()
        {

            // Europe/Dublin transitions from UTC+1 to UTC+0 at 2am (local) on 2017-10-29
            var dt = new LocalDateTime(2017, 10, 29, 1, 45, 0);
            DateTimeZone dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

            ZonedDateTime beforeTransition = new ZonedDateTime(dt, dublin, Offset.FromHours(1));

            Assert.AreEqual("2017-10-29T01:45:00 Europe/Dublin (+01)", beforeTransition.ToString());

            var result = ZonedDateTime.Add(beforeTransition, Duration.FromHours(1));
            Assert.AreEqual("2017-10-29T01:45:00 Europe/Dublin (+00)", result.ToString());
            //Console.WriteLine(result.Date);
            // Adding an hour of elapsed time takes us across the DST transition, so we have
            // the same local time (shown on a clock) but a different offset.
            //Console.WriteLine(result);

            // The + operator and Plus instance method are equivalent to the Add static method.
            //var result2 = beforeTransition + Duration.FromHours(1);
            //var result3 = beforeTransition.Plus(Duration.FromHours(1));
            //Console.WriteLine(result2);
            //Console.WriteLine(result3);

            var backAgain = result - Duration.FromHours(1);
            Assert.AreEqual("2017-10-29T01:45:00 Europe/Dublin (+01)", backAgain.ToString());
        }

        [TestMethod]
        public void InvalidTimeTest()
        {
            //Example 1: In the Pacific Time Zone(UTC - 08), the expression
            //new Date(2013, 2, 10, 2, 34, 56).toString() // Sun Mar 10 2013 02:34:56
            //
            var tz = IanaTimeZone.GetTimeZoneInfo("America/Vancouver");
            using (var ftz = new FakeLocalTimeZone(tz))
            {
                var invalidTime = new DateTime(2013, 3, 10, 2, 34, 56);
                var shouldFaul = invalidTime.ToUniversalTime();
                // did not fail, so Windows allows invalid time
                Assert.IsTrue(tz.IsInvalidTime(invalidTime));
            }

            var dt = new LocalDateTime(2013, 3, 10, 2, 34, 56);
            DateTimeZone utcMinus8 = DateTimeZoneProviders.Tzdb["America/Vancouver"];
            var ae1 = Assert.ThrowsException<ArgumentException>(() =>
            {
                var zdt = new ZonedDateTime(dt, utcMinus8, Offset.FromHours(-8));
            });
#if NET6_0_OR_GREATER
            Assert.AreEqual("Offset -08 is invalid for local date and time 10.03.2013 02:34:56 in time zone America/Vancouver (Parameter 'offset')", ae1.Message);
#else
            Assert.AreEqual("Offset -08 is invalid for local date and time 10.03.2013 02:34:56 in time zone America/Vancouver\r\nParameter name: offset", ae1.Message);
#endif

            var ae2 = Assert.ThrowsException<ArgumentException>(() =>
            {
                var zt = new ZoneTime(2013, 3, 10, 2, 34, 56, tz);
            });
            Assert.AreEqual($"Invalid time: '2013-03-10T02:34:56-08:00' is invalid in 'America/Vancouver'", ae2.Message);

            using (var ftz = new FakeLocalTimeZone(tz))
            {
                var local = new DateTime(2013, 3, 10, 2, 34, 56, DateTimeKind.Local);
                var ae3 = Assert.ThrowsException<ArgumentException>(() =>
                {
                    var zt = ZoneTime.FromLocalDateTime(local);
                });
                Assert.AreEqual($"Invalid time: '2013-03-10T02:34:56-08:00' is invalid in 'America/Vancouver'", ae3.Message);
            };


        }

        /// <summary>
        /// based on https://github.com/serokell/tztime
        /// </summary>
        [TestMethod]
        public void serokell_tztime_test()
        {
            //			λ > --It's 00:30 on 2022-11-06 in the America/Winnipeg time zone.
            //λ > tz = TZ.tzByLabel TZ.America__Winnipeg
            //λ > t1 = LocalTime(YearMonthDay 2022 11 6)(TimeOfDay 0 30 0)
            //λ > t1
            //2022 - 11 - 06 00:30:00
            var zt = new ZoneTime(2022, 11, 6, 0, 30, 0, IanaTimeZone.GetTimeZoneInfo("America/Winnipeg"));
            Assert.AreEqual($"2022-11-06T00:30:00-05:00[America/Winnipeg]", zt.ToString());

            var ztplus1h = zt.AddUtcHours(1);
            Assert.AreEqual($"2022-11-06T01:30:00-05:00[America/Winnipeg]", ztplus1h.ToString());
            var ztplus11h = ztplus1h.AddUtcHours(1);
            Assert.AreEqual($"2022-11-06T01:30:00-06:00[America/Winnipeg]", ztplus11h.ToString());
            var ztplus111h = ztplus11h.AddUtcHours(1);
            Assert.AreEqual($"2022-11-06T02:30:00-06:00[America/Winnipeg]", ztplus111h.ToString());
            var ztplus1111h = ztplus111h.AddUtcHours(1);
            Assert.AreEqual($"2022-11-06T03:30:00-06:00[America/Winnipeg]", ztplus1111h.ToString());

            //λ > --We naively add 4 hours to the local time.
            //λ > t2 = addLocalTime(secondsToNominalDiffTime 4 * 60 * 60) t1
            //λ > t2
            //2022 - 11 - 06 04:30:00
            var ztplus4h = zt.AddUtcHours(4);
            Assert.AreEqual(ztplus4h, ztplus1111h);

            var diff = ztplus4h.SubtractUtc(zt);
            Assert.AreEqual(4, diff.TotalHours);
            //λ > --Let's use the `tz` package to convert these times to UTC and
            //λ > --see how many hours have actually passed between t1 and t2.
            //λ > TZ.LTUUnique t1utc _ = TZ.localTimeToUTCFull tz t1
            //λ > TZ.LTUUnique t2utc _ = TZ.localTimeToUTCFull tz t2
            //λ > nominalDiffTimeToSeconds(diffUTCTime t2utc t1utc) / 60 / 60
            //5.000000000000



            //			λ > --It's 23:30 on 2022-03-12 in the America/Winnipeg time zone.
            //λ > tz = TZ.tzByLabel TZ.America__Winnipeg
            //λ > t1 = LocalTime(YearMonthDay 2022 3 12)(TimeOfDay 23 30 0)
            var zt2 = new ZoneTime(2022, 3, 12, 23, 30, 0, IanaTimeZone.GetTimeZoneInfo("America/Winnipeg"));
            //λ > --Convert to UTC, add 1 day, convert back to our time zone.
            var zt2plus1day = zt2.AddUtcHours(24);
            // yes...we did end up on day 14..
            Assert.AreEqual($"2022-03-14T00:30:00-05:00[America/Winnipeg]", zt2plus1day.ToString());

            //λ > TZ.LTUUnique t1utc _ = TZ.localTimeToUTCFull tz t1
            //λ > t2utc = addUTCTime nominalDay t1utc
            //λ > TZ.utcToLocalTimeTZ tz t2utc
            //2022 - 03 - 14 00:30:00


            var dt = new LocalDateTime(2022, 3, 12, 23, 30, 0);
            DateTimeZone amWin = DateTimeZoneProviders.Tzdb["America/Winnipeg"];

            ZonedDateTime before = new ZonedDateTime(dt, amWin, Offset.FromHours(-6));
            var after = before.PlusHours(24);
            // same problem. we moved 24h globally, but locally we got further
            Assert.AreEqual("2022-03-14T00:30:00 America/Winnipeg (-05)", after.ToString());
            Assert.AreEqual(24, (after - before).TotalHours);

        }

#if NET6_0_OR_GREATER
        bool Net6 => true;
#else
        bool Net6 => false;
#endif


        [TestMethod]
        public void ZonedTime_AddClock()
        {
            var zt2 = new ZoneTime(new ClockTime(2022, 3, 12, 23, 30, 0), IanaTimeZone.GetTimeZoneInfo("America/Winnipeg"));
            //λ > --Convert to UTC, add 1 day, convert back to our time zone.
            //var zt2plus1day = zt2.AddClockDays(1);

            var ct = zt2.ToClockTime();

            var s1 = ct.ToString();

            var ctplus1day = ct.AddDays(1);


            var s2 = ctplus1day.ToString();
            Assert.AreEqual("2022-03-13T23:30:00", s2.ToString());

            var zt3 = ctplus1day.ToZoneTime(zt2.Zone);

            // NO...we did not end up on day 14. So now we moved 1 day locally, but not globally...
            // Yes, we moved 1 day forward on the clock (imagine an analog circular clock)
            Assert.AreEqual($"2022-03-13T23:30:00-05:00[America/Winnipeg]", zt3.ToString());
            // in global time, we moved only 23h
            Assert.AreEqual(23, (zt3.SubtractUtc(zt2)).TotalHours);

            // clock time moved 24h
            Assert.AreEqual(24, (ctplus1day - ct).TotalHours);


        }

        [TestMethod]
        public void ZonedTime_AddClock2()
        {
            var t = new ZoneTime(2022, 3, 12, 23, 30, 0, IanaTimeZone.GetTimeZoneInfo("America/Winnipeg"));
            Assert.AreEqual($"2022-03-12T23:30:00-06:00[America/Winnipeg]", t.ToString());
            //λ > --Convert to UTC, add 1 day, convert back to our time zone.
            //var zt2plus1day = zt2.AddClockDays(1);

            var tplus1utcday = t.AddUtcHours(24);
            var tplus1clockday = t.AddClockDays(1);
            // 1 day in utc ends up 2 days ahead
            Assert.AreEqual($"2022-03-14T00:30:00-05:00[America/Winnipeg]", tplus1utcday.ToString());
            // 1 day in clock ends up 1 day (obviously, since time part of the iso string is clock time)
            Assert.AreEqual($"2022-03-13T23:30:00-05:00[America/Winnipeg]", tplus1clockday.ToString());

            var s1 = tplus1utcday.SubtractUtc(t);
            var s2 = tplus1utcday.SubtractClock(t);

            var s3 = tplus1clockday.SubtractUtc(t);
            var s4 = tplus1clockday.SubtractClock(t);

            // comparing utc agains utc and clock agains clock give 1 day
            Assert.AreEqual(TimeSpan.FromDays(1), s1);
            Assert.AreEqual(TimeSpan.FromDays(1), s4);

            // but we see we loose or gain 1 hour here...
            Assert.AreEqual(new TimeSpan(1, 1, 0, 0), s2);
            Assert.AreEqual(TimeSpan.FromHours(23), s3);

            //var s2 = ctplus1day.ToString();
            //Assert.Equal("2022-03-13T23:30:00", s2.ToString());

            //var zt3 = ctplus1day.ToZoneTime(zt2.Zone);

            // NO...we did not end up on day 14. So now we moved 1 day locally, but not globally...
            // Yes, we moved 1 day forward on the clock (imagine an analog circular clock)
            //Assert.Equal("2022-03-13T23:30:00-05:00[America/Chicago]", zt3.ToString());
            // in global time, we moved only 23h
            //Assert.Equal(23, (zt3.SubtractUtc(zt2)).TotalHours);

            // clock time moved 24h
            //Assert.Equal(24, (ctplus1day - ct).TotalHours);


        }

#if false
		[TestMethod]
		public void Amobigious_Choose_offset()
		{
			//Assert.Equal("2017-10-29T01:45:00+01:00[Europe/London]", zBack.ToString());

			var b = ZoneTime.TryParse("2017-10-29T01:45:00[Europe/London]", out var zt, offsets =>
			{
				return offsets.Last();
			});
			Assert.True(b);
			Assert.Equal("2017-10-29T01:45:00+01:00[Europe/London]", zt.ToString());

			var b2 = ZoneTime.TryParse("2017-10-29T01:45:00[Europe/London]", out var zt2, offsets =>
			{
				return offsets.First();
			});
			Assert.True(b2);
			Assert.Equal("2017-10-29T01:45:00+00:00[Europe/London]", zt2.ToString());

			
			//var b3 = ZoneTime.TryParse("2017-10-29T01:45:00[Europe/London]", out var zt3, offsets =>
			//{
			//	throw new Exception("ambigous");
			//});
			//Assert.False(b2);
			
		}
#endif

        [TestMethod]
        public void UtcTime_twoWaysToInitRaw()
        {
            var dt = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Unspecified);
            //var u1 = UtcTime.FromAnyDateTime(dt, TimeZoneInfo.Utc);
            var u2 = UtcTime.FromUnspecifiedDateTime(dt, TimeSpan.Zero);
            var u3 = UtcTime.FromUnspecifiedDateTime(dt, TimeZoneInfo.Utc);
            var u4 = UtcTime.FromUtcDateTime(dt.SpecifyKind(DateTimeKind.Utc));

            Assert.AreEqual(u4, u2);
            Assert.AreEqual(u2, u3);
            Assert.AreEqual("2020-01-02T03:04:05Z", u2.ToString());
        }

        [TestMethod]
        public void ClockTime_ParseMiscFormats()
        {
            var p1 = ClockTime.Parse("2020-01-02T03:04:05Z", (_,_) => true);
            var p2 = ClockTime.Parse("2020-01-02T03:04:05+01:00", (_, _) => true);
            var p3 = ClockTime.Parse("2020-01-02T03:04:05");

            Assert.AreEqual(p1, p2);
            Assert.AreEqual(p1, p3);
            Assert.AreEqual("2020-01-02T03:04:05", p1.ToString());
        }

        [TestMethod]
        public void ClockTime_ParseMiscFormats_RequireOffsetNone()
        {
            Assert.ThrowsException<FormatException>(() =>
            {
                var p1 = ClockTime.Parse("2020-01-02T03:04:05Z");
            });

            Assert.ThrowsException<FormatException>(() =>
            {
                var p2 = ClockTime.Parse("2020-01-02T03:04:05+01:00");
            });

            var p3 = ClockTime.Parse("2020-01-02T03:04:05");

            Assert.AreEqual("2020-01-02T03:04:05", p3.ToString());
        }

        [TestMethod]
        public void UTcTime_DateAndTimeOnly()
        {
            var t = new UtcTime(2020, 4, 6, 7, 8, 42);
            var d = t.Date;
            var tod = t.TimeOfDay;
            Assert.AreEqual("2020-04-06", d.ToIsoString());
            Assert.AreEqual("07:08:42.0000000", tod.ToIsoString());
        }

        [TestMethod]
        public void OFfsetTime_DateAndTimeOnly()
        {
            var t = new OffsetTime(2020, 4, 6, 7, 8, 42, TimeSpan.FromMinutes(42));
            var d = t.Date;
            var tod = t.TimeOfDay;
            Assert.AreEqual("2020-04-06", d.ToIsoString());
            Assert.AreEqual("07:08:42.0000000", tod.ToIsoString());
        }

        [TestMethod]
        public void ZoneTime_DateAndTimeOnly()
        {
            var t = new ZoneTime(2020, 4, 6, 7, 8, 42, TimeZoneInfo.Utc);
            var d = t.Date;
            var tod = t.TimeOfDay;
            Assert.AreEqual("2020-04-06", d.ToIsoString());
            Assert.AreEqual("07:08:42.0000000", tod.ToIsoString());
        }

        [TestMethod]
        public void ClockTime_DateAndTimeOnly()
        {
            var t = new ClockTime(2020, 4, 6, 7, 8, 42);
            var d = t.Date;
            var tod = t.TimeOfDay;
            Assert.AreEqual("2020-04-06", d.ToIsoString());
            Assert.AreEqual("07:08:42.0000000", tod.ToIsoString());
        }

        [TestMethod]
        public void CheckHasInanaId()
        {
            foreach (var ii in IanaTimeZone.GetIanaIds())
            {
                var tz = IanaTimeZone.GetTimeZoneInfo(ii);
                Assert.IsTrue(tz.HasIanaId());
            }
        }


        [TestMethod]
        public void AlwaysTimeZoneInfoUtc()
        {
            if (Net6)
            {
                Assert.AreEqual(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("UTC"));
                Assert.AreNotEqual(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Etc/UTC"));
                Assert.AreNotEqual(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Zulu"));
                Assert.AreNotEqual(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Etc/Zulu"));
                Assert.AreNotEqual(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Universal"));
                Assert.AreNotEqual(TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Etc/Universal"));
            }

            Assert.AreEqual(TimeZoneInfo.Utc, IanaTimeZone.GetTimeZoneInfo("UTC"));
            Assert.AreNotEqual(TimeZoneInfo.Utc, IanaTimeZone.GetTimeZoneInfo("Etc/UTC"));
            Assert.AreNotEqual(TimeZoneInfo.Utc, IanaTimeZone.GetTimeZoneInfo("Zulu"));
            Assert.AreNotEqual(TimeZoneInfo.Utc, IanaTimeZone.GetTimeZoneInfo("Etc/Zulu"));

            if (Net6)
            {
                Assert.AreEqual("(UTC) Coordinated Universal Time", TimeZoneInfo.Utc.DisplayName);
                Assert.AreEqual("Coordinated Universal Time", TimeZoneInfo.Utc.StandardName);
                Assert.AreEqual("Coordinated Universal Time", TimeZoneInfo.Utc.DaylightName);
            }
            else
            {
                Assert.AreEqual("UTC", TimeZoneInfo.Utc.DisplayName);
                Assert.AreEqual("UTC", TimeZoneInfo.Utc.StandardName);
                Assert.AreEqual("UTC", TimeZoneInfo.Utc.DaylightName);
            }

            var z = IanaTimeZone.GetTimeZoneInfo("Etc/Zulu");
            Assert.AreEqual("(UTC) Coordinated Universal Time", z.DisplayName);
            Assert.AreEqual("Coordinated Universal Time", z.StandardName);
            Assert.AreEqual("Coordinated Universal Time", z.DaylightName);

            if (Net6)
            {
                List<string> tzu = new();
                foreach (var ii in IanaTimeZone.GetIanaIds())
                {
                    try
                    {

                        var t = TimeZoneInfo.FindSystemTimeZoneById(ii);
                        if (t.DisplayName == "(UTC) Coordinated Universal Time" || t.DisplayName == "UTC")
                            tzu.Add(t.Id);
                    }
                    catch { }
                }
                Assert.AreEqual(18, tzu.Count);
            }

            List<string> tzu2 = new();
            foreach (var ii in IanaTimeZone.GetIanaIds())
            {
                try
                {

                    var t = IanaTimeZone.GetTimeZoneInfo(ii);
                    if (t.DisplayName == "(UTC) Coordinated Universal Time" || t.DisplayName == "UTC")
                        tzu2.Add(t.Id);
                }
                catch { }
            }
            Assert.AreEqual(18, tzu2.Count);
        }


        [TestMethod]
        public void Can_Convert_Kyiv_To_Windows()
        {
            var result = IanaTimeZone.GetWindowsId("Europe/Kyiv");
            Assert.AreEqual("FLE Standard Time", result);
        }

        [TestMethod]
        public void Can_Convert_Kiev_To_Windows()
        {
            var result = IanaTimeZone.GetWindowsId("Europe/Kiev");
            Assert.AreEqual("FLE Standard Time", result);
        }

        [TestMethod]
        public void TrollNeverWorks()
        {
            Assert.ThrowsException<TimeZoneNotFoundException>(() => TimeZoneInfo.FindSystemTimeZoneById("Antarctica/Troll"));
            Assert.ThrowsException<TimeZoneNotFoundException>(() => IanaTimeZone.GetTimeZoneInfo("Antarctica/Troll"));
        }

        [TestMethod]
        public void TzAndZ()
        {
            // https://datatracker.ietf.org/doc/draft-ietf-sedate-datetime-extended/

            var zt = ZoneTime.Parse("2022-07-08T00:14:07Z[Europe/Paris]");
            Assert.AreEqual("2022-07-08T02:14:07+02:00[Europe/Paris]", zt.ToString());
        }

        [TestMethod]
        public void ClockTime_FromDateTime()
        {
            var now = DateTime.Now;
            var utcNow = now.SpecifyKind(DateTimeKind.Utc);
            var unspecNow = now.SpecifyKind(DateTimeKind.Unspecified);

            var ctNow = ClockTime.FromDateTime(now);
            var ctUtcNow = ClockTime.FromDateTime(utcNow);
            var ctUnspecNow = ClockTime.FromDateTime(unspecNow);

            Assert.AreEqual(ctNow, ctUtcNow);
            Assert.AreEqual(ctUtcNow, ctUnspecNow);
        }

        [TestMethod]
        public void MizzingTz()
        {
            var t = ZoneTime.Parse("2022-07-08T00:14:07Z", (dto, ok) =>
            {
                Assert.AreEqual(OffsetKind.Zulu, ok);
                return IanaTimeZone.GetTimeZoneInfo("Europe/Paris");
            });
            Assert.AreEqual("2022-07-08T02:14:07+02:00[Europe/Paris]", t.ToString());

            var t2 = ZoneTime.Parse("2022-07-08T00:14:07", (dto, ok) =>
            {
                Assert.AreEqual(OffsetKind.None, ok);
                return IanaTimeZone.GetTimeZoneInfo("Europe/Paris");
            });
            Assert.AreEqual("2022-07-08T00:14:07+02:00[Europe/Paris]", t2.ToString());
        }

        [TestMethod]
        public void ClockTime_ParseMiscFormats_disallow_offset()
        {
            Assert.ThrowsException<FormatException>(() =>
            {
                var p1 = ClockTime.Parse("2020-01-02T03:04:05Z");
            });
            Assert.ThrowsException<FormatException>(() =>
            {
                var p2 = ClockTime.Parse("2020-01-02T03:04:05+01:00");
            });

            
        }
    }
}
