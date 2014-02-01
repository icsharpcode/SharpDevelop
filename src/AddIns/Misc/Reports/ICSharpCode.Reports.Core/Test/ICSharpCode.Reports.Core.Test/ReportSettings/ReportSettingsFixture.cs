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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test
{
	[TestFixture]
	
	public class ReportSettingsFixture
	{
		const string reportName = "ReportName";	
		const string fileName = "FileName.srd";
		
		#region Constructor
		
		[Test]
		public void DefaultConstructureShouldReturnStandardValues()
		{
			ReportSettings rs = new ReportSettings();
			Assert.IsNotNull(rs,"Should not be 'null'");
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName,"Should be 'Report1'");
			FileInfo fileInfo = new System.IO.FileInfo(rs.FileName);
			Assert.AreEqual(GlobalValues.PlainFileName,fileInfo.Name,"Should be 'Report1.srd");
			Assert.AreEqual(GlobalValues.DefaultPageSize,rs.PageSize);
		}
		
		
		[Test]
		public void ConstructorWithParams ()
		{
			ReportSettings rs = new ReportSettings (GlobalValues.DefaultPageSize,reportName,"FileName");
			Assert.IsNotNull(rs,"Should not be null");
			FileInfo fileInfo = new System.IO.FileInfo(rs.FileName);
			Assert.AreEqual(GlobalValues.DefaultPageSize,rs.PageSize);
			Assert.AreEqual(reportName,rs.ReportName,"Should be 'ReportName'");
			Assert.AreEqual(fileName,fileInfo.Name,"Should be FileName.srd'");
		}
		
		
		[Test]
		public void ConstructorWithEmptyReportName ()
		{
			ReportSettings rs = new ReportSettings (GlobalValues.DefaultPageSize,"","FileName");
			FileInfo fileInfo = new System.IO.FileInfo(rs.FileName);
			Assert.IsNotNull(rs,"Should not be null");
			Assert.AreEqual(GlobalValues.DefaultReportName,rs.ReportName,"Should be 'Report1'");
			Assert.AreEqual(fileName,fileInfo.Name);
		}
		
		
		[Test]
		public void ConstructorWithEmptyFileName ()
		{
			ReportSettings rs = new ReportSettings (GlobalValues.DefaultPageSize,reportName,"");
			Assert.AreEqual(rs.ReportName,reportName,"Should be 'ReportName'");
			FileInfo fileInfo = new System.IO.FileInfo(rs.FileName);
			Assert.AreEqual(GlobalValues.PlainFileName,fileInfo.Name,"Should be 'report1.srd'");
		}
		
		#endregion
		
		#region Collections 
		
		[Test]
		public void CheckDefaultCollections ()
		{
			ReportSettings rs = new ReportSettings();
			Assert.IsNotNull (rs.AvailableFieldsCollection);
			Assert.AreEqual (0,rs.AvailableFieldsCollection.Count);
			
			Assert.IsNotNull (rs.GroupColumnsCollection);
			Assert.AreEqual (0,rs.GroupColumnsCollection.Count);
			
			Assert.IsNotNull (rs.SortColumnsCollection);
			Assert.AreEqual (0,rs.SortColumnsCollection.Count);
			
			Assert.IsNotNull (rs.ParameterCollection);
			Assert.AreEqual (0,rs.ParameterCollection.Count);
				
			Assert.IsNotNull(rs.AvailableFieldsCollection);
			Assert.AreEqual(0,rs.AvailableFieldsCollection.Count);
		}
		
		#endregion
		
		
		[Test]
		public void CheckDefaultSettings ()
		{
			ReportSettings rs = new ReportSettings();
			Assert.AreEqual(true,rs.UseStandardPrinter,"StandartPrinter should be 'true'");
			Assert.AreEqual (rs.GraphicsUnit,System.Drawing.GraphicsUnit.Pixel,"GraphicsUnit should be 'millimeter'");
			
//			Assert.AreEqual (new System.Windows.Forms.Padding(5),rs.Padding);
		
			Assert.AreEqual (GlobalEnums.ReportType.FormSheet,rs.ReportType);
			Assert.AreEqual (GlobalEnums.PushPullModel.FormSheet,rs.DataModel);
			
			Assert.AreEqual (String.Empty,rs.ConnectionString);
			Assert.AreEqual (String.Empty,rs.CommandText);
			Assert.AreEqual (System.Data.CommandType.Text,rs.CommandType);
			
			Assert.AreEqual ("Microsoft Sans Serif" ,rs.DefaultFont.Name);
			Assert.AreEqual (10,rs.DefaultFont.Size);
		}
		
		#region Report - FileName
		
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
			FileInfo fileInfo = new System.IO.FileInfo(rs.FileName);
			Assert.AreEqual(GlobalValues.PlainFileName,fileInfo.Name);
		}
		
		#endregion
		
		#region Size and margin
		
		[Test]
		public void DefaultPageSize ()
		{
			ReportSettings rs = new ReportSettings();
			Assert.AreEqual(GlobalValues.DefaultPageSize,rs.PageSize);
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
			ReportSettings rs = new ReportSettings();
			System.Drawing.Printing.Margins margin = new System.Drawing.Printing.Margins(rs.LeftMargin,rs.RightMargin,
			                                                                             rs.TopMargin,rs.BottomMargin);
			Assert.AreEqual(margin.Left,rs.LeftMargin);
			Assert.AreEqual(GlobalValues.DefaultPageMargin,margin);
		}
		
		#endregion
		
		
		
		
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
