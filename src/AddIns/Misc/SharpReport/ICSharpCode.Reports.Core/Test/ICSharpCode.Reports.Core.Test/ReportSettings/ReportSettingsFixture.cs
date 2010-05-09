/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 26.10.2008
 * Zeit: 18:28
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.IO;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test
{
	[TestFixture]
	public class ReportSettingsFixture:ConcernOf<ReportSettings>
	{
		const string reportName = "ReportName";	
		const string fileName = "FileName.srd";
		
		#region Constructor
		
		[Test]
		public void DefaultConstructureShouldReturnStandardValues()
		{
			Assert.IsNotNull(Sut,"Should not be 'null'");
			Assert.AreEqual(GlobalValues.DefaultReportName,Sut.ReportName,"Should be 'Report1'");
			FileInfo fileInfo = new System.IO.FileInfo(Sut.FileName);
			Assert.AreEqual(GlobalValues.PlainFileName,fileInfo.Name,"Should be 'Report1.srd");
			Assert.AreEqual(GlobalValues.DefaultPageSize,Sut.PageSize);
		}
		
		
		[Test]
		public void ConstructorWithParams ()
		{
			ReportSettings rs = new ReportSettings (GlobalValues.DefaultPageSize,reportName,"FileName");
			Assert.IsNotNull(rs,"Should not be null");
			FileInfo fileInfo = new FileInfo(rs.FileName);
			Assert.AreEqual(GlobalValues.DefaultPageSize,rs.PageSize);
			Assert.AreEqual(reportName,rs.ReportName,"Should be 'ReportName'");
			Assert.AreEqual(fileName,fileInfo.Name,"Should be FileName.srd'");
		}
		
		
		[Test]
		public void ConstructorWithEmptyReportName ()
		{
			ReportSettings rs = new ReportSettings (GlobalValues.DefaultPageSize,"","FileName");
			FileInfo fileInfo = new FileInfo(rs.FileName);
			Assert.IsNotNull(rs,"Should not be null");
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName,"Should be 'Report1'");
			Assert.AreEqual(fileName,fileInfo.Name);
		}
		
		
		[Test]
		public void ConstructorWithEmptyFileName ()
		{
			ReportSettings rs = new ReportSettings (GlobalValues.DefaultPageSize,reportName,"");
			Assert.AreEqual(rs.ReportName,reportName,"Should be 'ReportName'");
			FileInfo fileInfo = new FileInfo(rs.FileName);
			Assert.AreEqual(GlobalValues.PlainFileName,fileInfo.Name,"Should be 'report1.srd'");
		}
		
		#endregion
		
		#region Collections 
		
		[Test]
		public void CheckDefaultCollections ()
		{
			Assert.IsNotNull (Sut.AvailableFieldsCollection);
			Assert.AreEqual (0,Sut.AvailableFieldsCollection.Count);
			
			Assert.IsNotNull (Sut.GroupColumnsCollection);
			Assert.AreEqual (0,Sut.GroupColumnsCollection.Count);
			
			Assert.IsNotNull (Sut.SortColumnCollection);
			Assert.AreEqual (0,Sut.SortColumnCollection.Count);
			
			Assert.IsNotNull (Sut.ParameterCollection);
			Assert.AreEqual (0,Sut.ParameterCollection.Count);
				
			Assert.IsNotNull(Sut.AvailableFieldsCollection);
			Assert.AreEqual(0,Sut.AvailableFieldsCollection.Count);
		}
		
		#endregion
		
		
		[Test]
		public void CheckDefaultSettings ()
		{
			Assert.AreEqual(true,Sut.UseStandardPrinter,"StandartPrinter should be 'true'");
			Assert.AreEqual (Sut.GraphicsUnit,System.Drawing.GraphicsUnit.Pixel,"GraphicsUnit should be 'millimeter'");
		
			Assert.AreEqual (GlobalEnums.ReportType.FormSheet,Sut.ReportType);
			Assert.AreEqual (GlobalEnums.PushPullModel.FormSheet,Sut.DataModel);
			
			Assert.AreEqual (String.Empty,Sut.ConnectionString);
			Assert.AreEqual (String.Empty,Sut.CommandText);
			Assert.AreEqual (System.Data.CommandType.Text,Sut.CommandType);
			
			Assert.AreEqual ("Microsoft Sans Serif" ,Sut.DefaultFont.Name);
			Assert.AreEqual (10,Sut.DefaultFont.Size);
		}
		
		#region Report - FileName
		
		[Test]
		public void BlankReportNameReturnsDefaultReportName()
		{
			Sut.ReportName = String.Empty;
			Assert.AreEqual(GlobalValues.DefaultReportName,Sut.ReportName);
		}
		
		[Test]
		public void BlankFileNameReturnsDefaultFileName()
		{
			Sut.FileName = String.Empty;
			FileInfo fileInfo = new System.IO.FileInfo(Sut.FileName);
			Assert.AreEqual(GlobalValues.PlainFileName,fileInfo.Name);
		}
		
		#endregion
		
		#region Size and margin
		
		[Test]
		public void DefaultPageSize ()
		{
			Assert.AreEqual(GlobalValues.DefaultPageSize,Sut.PageSize);
		}
		
		[Test]
		public void CustomePageSize ()
		{
			Size customSize = new Size(200,150);
			ReportSettings rs = new ReportSettings(customSize,"aa","bb");
			Assert.AreEqual(customSize,rs.PageSize);
		}
		
		[Test]
		public void DefaultReportMargin ()
		{
			System.Drawing.Printing.Margins margin = new System.Drawing.Printing.Margins(Sut.LeftMargin,Sut.RightMargin,
			                                                                             Sut.TopMargin,Sut.BottomMargin);
			Assert.AreEqual(margin.Left,Sut.LeftMargin);
			Assert.AreEqual(GlobalValues.DefaultPageMargin,margin);
		}
		
		#endregion
		
		
		public override void Setup()
		{
			Sut = new ReportSettings();
		}
	}
}
