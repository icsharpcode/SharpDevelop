/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 30.10.2008
 * Zeit: 20:09
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;
using ICSharpCode.Reports.Core;

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
			Assert.AreEqual(GlobalEnums.ReportSection.ReportHeader.ToString(),s.Name);
		}
		
		[Test]
		public void CheckPageHeader ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.PageHeader;
			Assert.AreEqual(GlobalEnums.ReportSection.ReportPageHeader.ToString(),s.Name);
		}
		
		[Test]
		public void CheckDetailSection ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.DetailSection;
			Assert.AreEqual(GlobalEnums.ReportSection.ReportDetail.ToString(),s.Name);
		}
		
		
		[Test]
		public void CheckPageFooter ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.PageFooter;
			Assert.AreEqual(GlobalEnums.ReportSection.ReportPageFooter.ToString(),s.Name);
		}
		
		[Test]
		public void CheckReportFooter ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			BaseSection s = m.ReportFooter;
			Assert.AreEqual(GlobalEnums.ReportSection.ReportFooter.ToString(),s.Name);
		}
		
		[Test]
		public void CheckDataModel ()
		{
			ReportModel m = ReportModel.Create (System.Drawing.GraphicsUnit.Inch);
			Assert.AreEqual(GlobalEnums.PushPullModel.FormSheet,m.DataModel);
		}
	}
}
