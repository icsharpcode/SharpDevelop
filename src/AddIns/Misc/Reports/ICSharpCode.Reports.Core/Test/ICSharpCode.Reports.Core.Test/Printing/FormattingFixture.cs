// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		public void TimeSpan_H_mm_ss ()
		{
			string toFormat = "5:50:10";
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("5:50:10"));
		}
		
		
		[Test]
		public void NegativeTimeSpan_HH_mm_ss ()
		{
			TimeSpan time;
			string toFormat = "-5:50:10";
			string format = "H:mm:ss";
			var result = StandardFormatter.FormatOutput(toFormat,format,dateTimetype,nullValue);
			Assert.That(result,Is.EqualTo("-5:50:10"));
			bool valid = TimeSpan.TryParseExact(result,
			                                    "c",
			                                    CultureInfo.CurrentCulture,
			                                    out time);
			Assert.That(valid,Is.True);
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
