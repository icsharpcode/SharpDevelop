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
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	/*
	[TestFixture]
	public class ReportStructureFixture
	{
		
		
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
	*/
}
