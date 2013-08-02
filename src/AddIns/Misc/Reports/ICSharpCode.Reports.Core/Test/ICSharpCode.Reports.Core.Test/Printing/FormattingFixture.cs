/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 13.02.2013
 * Time: 19:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using ICSharpCode.Reports.Core.BaseClasses.Printing;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Printing
{
	[TestFixture]
	public class FormattingFixture
	{
		private const string stringType  = "System.String";
		private const string dateTimetype = "System.DateTime";
		private const string nullValue ="NullValue";
		
		
		[Test]
		public void String_Is_Not_Formatted()
		{
			string toFormat = "Hello World";
			string format = "dd/MM/yy";
			var result = StandardFormatter.FormatOutput(toFormat,format,stringType,nullValue);
			Assert.That(result,Is.EqualTo(toFormat));
		}
		
		[Test]
		public void Empty_Input_Returns_NullValue()
		{
			string toFormat = string.Empty;;
			string format = "dd/MM/yy";
			var result = StandardFormatter.FormatOutput(toFormat,format,stringType,nullValue);
			Assert.That(result,Is.EqualTo(nullValue));
		}
			
		#region DateTime
		
		[Test]
		public void DateTime_dd_MM_YY ()
		{
			string toFormat = "2012/02/12";
			string format = "dd.MM.yy";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("12.02.12"));
		}
		
		
		[Test]
		public void TypeDateTimeOfResultIsString()
		{
			string toFormat = "2012/02/12";
			string format = "dd.MM.yy";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.TypeOf(typeof(string)));
		}
		
		
		[Test]
		public void ConvertResultToDateTime()
		{
			DateTime date;
			string toFormat = "2012/02/12";
			string format = "dd.MM.yy";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			
			
			bool valid = DateTime.TryParse(toFormat, out date);
			
			Assert.That(valid,Is.True);
			Assert.That(date,Is.EqualTo(new DateTime(2012,02,12)));
		}
		
		#endregion
		
		#region TimeSpan
	
		[Test]
		public void TimeSpan_HH_mm_ss ()
		{
			string toFormat = "5:50:10";
			string format = "HH:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("05:50:10"));
		}

		
		[Test]
		public void NegativeTimeSpan_HH_mm_ss ()
		{
			string toFormat = "-5:50:10";
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("-5:50:10"));
		}
		
		
		[Test]
		public void TimeSpan_HH_mm ()
		{
			string toFormat = "5:50:10";
			string format = "HH:mm";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("05:50"));
		}
		
		
		[Test]
		public void TimeSpan_H_mm_ss ()
		{
			string toFormat = "5:50:10";
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("5:50:10"));
		}
		
		
		[Test]
		public void TimeSpan_D_H_mm_ss ()
		{
			string toFormat = "1,5:50:10";
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("1,5:50:10"));
		}
		
		
		[Test]
		public void TypeOfTimeSpanResultIsString()
		{
			string toFormat = "5,50,10";	
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.TypeOf(typeof(string)));
		}
			
		
		[Test]
		public void ConvertResultToTimeSpan()
		{
			TimeSpan time;
			string toFormat = "5:50:10";
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
				
			bool valid = TimeSpan.TryParseExact(result,
			                                    "c",
			                                    CultureInfo.CurrentCulture,
			                                    out time);
			Assert.That(valid,Is.True);
			Assert.That(time,Is.EqualTo(new TimeSpan(5,50,10)));
		}
		
		#endregion
	}
}
