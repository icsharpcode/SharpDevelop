// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
