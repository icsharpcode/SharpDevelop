/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 26.10.2008
 * Zeit: 18:28
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Data;
using NUnit.Framework;
using ICSharpCode.Reports.Core;
using System.Drawing;
	
namespace ICSharpCode.Reports.Core.Test
{
	[TestFixture]
	public class ReportSettingsFixture
	{
		const string reportName = "ReportName";	
		const string fileName = "FileName.srd";
		Size defaultSize = new Size(827,1169);
		
		
		[Test]
		public void DefaultConstructureNotNull()
		{
			ReportSettings rs = new ReportSettings();
			Assert.IsNotNull(rs,"Should not be 'null'");
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName,"Should be 'Report1'");
			Assert.AreEqual(GlobalValues.PlainFileName,rs.FileName,"Should be 'Report1.srd");
			Assert.AreEqual(defaultSize,rs.PageSize);
		}
		
		
		[Test]
		public void ConstructorWithParams ()
		{
			ReportSettings rs = new ReportSettings (this.defaultSize,reportName,"FileName");
			Assert.IsNotNull(rs,"Should not be null");
			
			Assert.AreEqual(defaultSize,rs.PageSize);
			Assert.AreEqual(reportName,rs.ReportName,"Should be 'ReportName'");
			Assert.AreEqual(fileName,rs.FileName,"Should be FileName.srd'");
		}
		
		
		[Test]
		public void ConstructorWithEmptyReportName ()
		{
			ReportSettings rs = new ReportSettings (defaultSize,"","FileName");
			Assert.IsNotNull(rs,"Should not be null");
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName,"Should be 'Report1'");
			Assert.AreEqual(fileName,rs.FileName);
		}
		
		
		[Test]
		public void ConstructorWithEmptyFileName ()
		{
			ReportSettings rs = new ReportSettings (defaultSize,reportName,"");
			Assert.AreEqual(rs.ReportName,reportName,"Should be 'ReportName'");
			Assert.AreEqual(rs.FileName,GlobalValues.PlainFileName,"Should be 'report1.srd'");
		}
		
		
		[Test]
		public void CheckDefaultCollections ()
		{
			ReportSettings rs = new ReportSettings();
			Assert.IsNotNull (rs.AvailableFieldsCollection);
			Assert.AreEqual (0,rs.AvailableFieldsCollection.Count);
			
			Assert.IsNotNull (rs.GroupColumnsCollection);
			Assert.AreEqual (0,rs.GroupColumnsCollection.Count);
			
			Assert.IsNotNull (rs.SortColumnCollection);
			Assert.AreEqual (0,rs.SortColumnCollection.Count);
			
			Assert.IsNotNull (rs.ParameterCollection);
			Assert.AreEqual (0,rs.ParameterCollection.Count);
				
		}
		
		
		[Test]
		public void CheckDefaultSettings ()
		{
			ReportSettings rs = new ReportSettings();
			Assert.AreEqual(true,rs.UseStandardPrinter,"StandartPrinter should be 'true'");
			Assert.AreEqual (rs.GraphicsUnit,System.Drawing.GraphicsUnit.Pixel,"GraphicsUnit should be 'millimeter'");
			
			Assert.AreEqual (new System.Windows.Forms.Padding(5),rs.Padding);
		
			Assert.AreEqual (GlobalEnums.ReportType.FormSheet,rs.ReportType);
			Assert.AreEqual (GlobalEnums.PushPullModel.FormSheet,rs.DataModel);
			
			Assert.AreEqual (String.Empty,rs.ConnectionString);
			Assert.AreEqual (String.Empty,rs.CommandText);
			Assert.AreEqual (System.Data.CommandType.Text,rs.CommandType);
			
			Assert.AreEqual ("Microsoft Sans Serif" ,rs.DefaultFont.Name);
			Assert.AreEqual (10,rs.DefaultFont.Size);
		}
		
		
		[Test]
		public void BlankReportNameReturnsDefaultReportName()
		{
			ReportSettings rs = new ReportSettings();
			rs.ReportName = String.Empty;
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName);
		}
		
		[Test]
		public void BlankFileNameReturnsDefaultFileName()
		{
			ReportSettings rs = new ReportSettings();
			rs.FileName = String.Empty;
			Assert.AreEqual(GlobalValues.PlainFileName,rs.FileName);
		}
		
		[Test]
		public void FileNameChangedEvent()
		{
			bool fired = false;
			
			ReportSettings rs = new ReportSettings();
			rs.FileNameChanged += delegate { fired = true; };
			rs.FileName = "aaaa";
			Assert.IsTrue(fired);
		}
			
		
		
		[TestFixtureSetUp]
		public void Init()
		{
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
		}
	}
}
