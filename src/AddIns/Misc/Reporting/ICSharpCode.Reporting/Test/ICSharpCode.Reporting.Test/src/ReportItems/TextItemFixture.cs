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
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.ReportItems
{
	[TestFixture]
	public class TextItemFixture
	{
		[Test]
		public void IsNameSetOnInitialize() {
			var ti = new BaseTextItem();
			Assert.That (ti.Name,Is.EqualTo("BaseTextItem"));
		}
		
		
		[Test]
		public void ChangeName() {
			var newName = "changed";
			var ti = new BaseTextItem();
			ti.Name = newName;
			Assert.That(ti.Name,Is.EqualTo(newName));
		}
		
		
		[Test]
		public void DefaultFontOnInitialize() {
			var ti = new BaseTextItem();
			Assert.That(ti.Font,Is.EqualTo(GlobalValues.DefaultFont));
		}
		
		[Test]
		public void CreateExportText() {
			var ti = new BaseTextItem();
			var exportText = (ExportText)ti.CreateExportColumn();
			Assert.That(exportText.Name,Is.EqualTo(ti.Name));
			Assert.That(exportText.Location,Is.EqualTo(ti.Location));
			Assert.That(exportText.Size,Is.EqualTo(ti.Size));
			Assert.That(exportText.Font , Is.EqualTo(GlobalValues.DefaultFont));
		}
		

		[Test]
		public void FormatTimeSpanfromTime() {
			var ti = new BaseTextItem();
			ti.DataType = "System.TimeSpan";
			ti.FormatString = "hh:mm:ss";
			ti.Text = new TimeSpan(7,17,20).ToString();
			var exportColumn = (ExportText)ti.CreateExportColumn();
			StandardFormatter.FormatOutput(exportColumn);
			Assert.That(ti.Text, Is.EqualTo("07:17:20"));
		}
		
		/*
		[Test]
		public void FormatTimeSpanFromTicks() {
			var ti = new BaseTextItem();
			ti.DataType = "System.TimeSpan";
			ti.FormatString = "hh:mm:ss";
			var x =  new TimeSpan(7,17,20);
			var y = x.Ticks;
//			ti.Text = x.ToString();
//			TimeSpan myts = new TimeSpan(-555234423213113);
//			ti.Text = myts.ToString();
			ti.Text = y.ToString();
			var exportColumn = (ExportText)ti.CreateExportColumn();
			StandardFormatter.FormatOutput(exportColumn);
		}
		 */
		}
}
