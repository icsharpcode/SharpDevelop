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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

/*
namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	/// <summary>
	/// Description of GenerateListWithGroupFixture.
	/// </summary>
	[TestFixture]
//	[Ignore]
	public class GenerateListWithGroupFixture
	{
		
		private const string reportName = "ListBasedReport";
		private ReportModel reportModel;
		
	
		[Test]
		public void PageDetail_Should_Contains_GroupedHeader()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			
			//GroupHeader
			var c =  new Collection<ICSharpCode.Reports.Core.GroupHeader>(section.Items.OfType<ICSharpCode.Reports.Core.GroupHeader>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void Section_Should_Contain_DataRow()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			//DatatRow
			var c =  new Collection<ICSharpCode.Reports.Core.BaseRowItem>(section.Items.OfType<ICSharpCode.Reports.Core.BaseRowItem>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
	
		[Test]
		public void DataRow_Should_Contain_GroupFooter()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			
			//GroupFooter
			var c =  new Collection<ICSharpCode.Reports.Core.GroupFooter>(section.Items.OfType<ICSharpCode.Reports.Core.GroupFooter>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void PageDetail_Should_Contain_Three_Items()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			Assert.That(section.Items.Count.Equals(3));
		}
		
		
		
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			bool createGrouping = true;
			this.reportModel = ReportGenerationHelper.CreateModel(reportName,createGrouping);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	
		private static ReportStructure CreateReportStructure (string reportName)
		{
			ReportStructure structure = new ReportStructure();
			structure.ReportName = reportName;
			structure.DataModel = GlobalEnums.PushPullModel.PushData;
			structure.ReportType = GlobalEnums.ReportType.DataReport;
			return structure;
		}
		
		
		#endregion
		
	}
}
*/
