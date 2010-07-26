/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 02.11.2008
 * Zeit: 18:16
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	[TestFixture]
	public class ReportStructureFixture
	{
		[Test]
		public void Constructore()
		{
			ReportStructure rs = new ReportStructure();
			Assert.IsNotNull(rs,"ReportStructure should not be 'null'");
		}
		
		[Test]
		public void FormSheetReport ()
		{
			ReportStructure rs = new ReportStructure();
			rs.ReportName = "PlainReport";
			rs.GraphicsUnit = System.Drawing.GraphicsUnit.Millimeter;
			rs.ReportType = GlobalEnums.ReportType.FormSheet;
			rs.DataModel = GlobalEnums.PushPullModel.FormSheet;
			
			ReportModel m = rs.CreateAndFillReportModel ();
			Assert.AreEqual("PlainReport",m.ReportSettings.ReportName,"invalid ReportName");
			Assert.AreEqual(m.ReportSettings.ReportType,GlobalEnums.ReportType.FormSheet,
			                "ReportModel,ReportSettings.eportType should be 'FormSheet'");
			Assert.AreEqual (GlobalEnums.PushPullModel.FormSheet,m.DataModel,
			                 "Datamodel should be 'FormSheet'");
			
		}
		
		[Test]
		public void CreateFormSheetReport ()
		{
			ReportModel rm = ReportGenerationHelper.FormSheetModel();
			Assert.IsNotNull(rm);
				
		}
		
		
		private ReportModel FormSheetModel()
		{
			ReportStructure rs = new ReportStructure();
			rs.ReportName = "PlainReport";
			rs.GraphicsUnit = System.Drawing.GraphicsUnit.Millimeter;
			rs.ReportType = GlobalEnums.ReportType.FormSheet;
			rs.DataModel = GlobalEnums.PushPullModel.FormSheet;	
			ReportModel m = rs.CreateAndFillReportModel ();
			return m;
		}
	}
}
