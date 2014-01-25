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
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test
{
	[TestFixture]
	public class ReportModelFixture
	{
		[Test]
		public void PlainConstructor()
		{
			ReportModel m = ReportModel.Create();
			Assert.IsNotNull(m,"should be not 'null'");
		}
		
		[Test]
		public void ConstructorWithGraphics ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Millimeter);
			Assert.IsNotNull(m,"should be not 'null'");
		}
		
		[Test]
		public void ReportSettings ()
		{
			ReportModel m = ReportModel.Create();
			Assert.IsNotNull(m.ReportSettings,"ReportSettings should not be 'null'");
		}
		
		[Test]
		public void CheckGraphicsUnit ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			Assert.AreEqual(m.ReportSettings.GraphicsUnit,System.Drawing.GraphicsUnit.Inch,"Should be Inch");
		}
		
		
		[Test]
		public void CheckReportHeader ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.ReportHeader;
			Assert.AreEqual(ReportSectionNames.ReportHeader,s.Name);
		}
		
		[Test]
		public void CheckPageHeader ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.PageHeader;
			Assert.AreEqual(ReportSectionNames.ReportPageHeader,s.Name);
		}
		
		[Test]
		public void CheckDetailSection ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.DetailSection;
			Assert.AreEqual(ReportSectionNames.ReportDetail,s.Name);
		}
		
		
		[Test]
		public void CheckPageFooter ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.PageFooter;
			Assert.AreEqual(ReportSectionNames.ReportPageFooter,s.Name);
		}
		
		[Test]
		public void CheckReportFooter ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.ReportFooter;
			Assert.AreEqual(ReportSectionNames.ReportFooter,s.Name);
		}
		
		[Test]
		public void CheckDataModel ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			Assert.AreEqual(GlobalEnums.PushPullModel.FormSheet,m.DataModel);
		}
	}
}
