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
using System.Drawing;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Tests
{
	[TestFixture]
	public class ReportSettingsFixture
	{
		ReportSettings reportSettings;
		
		[Test]
		public void DefaultConstructureShouldReturnStandardValues()
		{
			Assert.IsNotNull(reportSettings,"Should not be 'null'");
			Assert.AreEqual(GlobalValues.DefaultReportName,reportSettings.ReportName);
		}
		
		
		[Test]
		public void DefaultPageSize ()
		{
			Assert.AreEqual(GlobalValues.DefaultPageSize,reportSettings.PageSize);
		}
		
		
		[Test]
		public void LandScape_True_Return_PageSize_For_LandScape ()
		{
			reportSettings.Landscape = true;
			var landscapeSize = new Size(Globals.GlobalValues.DefaultPageSize.Height,
			                             Globals.GlobalValues.DefaultPageSize.Width);
			Assert.That(reportSettings.PageSize,Is.EqualTo(landscapeSize));
		}
		
		[SetUp]
		public void Setup () {
			reportSettings = new ReportSettings();
		}
	}
}
