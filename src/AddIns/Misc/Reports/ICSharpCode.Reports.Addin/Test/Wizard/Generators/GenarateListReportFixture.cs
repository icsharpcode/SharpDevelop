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
using System.ComponentModel;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	
	[TestFixture]
	public class GenerateListReportFixture
	{
		
		private const string reportName = "ListBasedReport";
		ReportModel reportModel;
		
		[Test]
		public void InitModel()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			Assert.AreEqual(reportName,m.ReportSettings.ReportName);
			Assert.AreEqual(1,m.ReportSettings.AvailableFieldsCollection.Count);
			Assert.AreEqual(GlobalEnums.ReportType.DataReport,m.ReportSettings.ReportType);
			
		}
		
		#region General generated Properties
		
		[Test]
		public void Datamodel_Should_PushModel()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			Assert.AreEqual(GlobalEnums.PushPullModel.PushData,m.ReportSettings.DataModel);
		}
		
		
		[Test]
		public void GroupColumCollection_Should_Empty ()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			Assert.That(m.ReportSettings.GroupColumnsCollection,Is.Empty);
		}
		
			
		[Test]
		public void SortColumnCollection_Should_Empty ()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			Assert.That(m.ReportSettings.SortColumnsCollection,Is.Empty);
		}
		
		#endregion
		
		#region Sort_Group
		
		[Test]
		public void CanAddGroupColumn()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			var rs = m.ReportSettings;
			
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			rs.GroupColumnsCollection.Add(gc);
			
			Assert.AreEqual(rs.GroupColumnsCollection.Count,1);
		}
		

		[Test]
		public void CanAddSortColumn()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			var rs = m.ReportSettings;
			
			SortColumn gc = new SortColumn("GroupItem",ListSortDirection.Ascending);
			rs.SortColumnsCollection.Add(gc);
			
			Assert.AreEqual(rs.SortColumnsCollection.Count,1);
		}
		#endregion
		
		
		#region ReportHeader
	
		[Test]
		public void ReportHeader_ShouldContain_OneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.ReportHeader;
			Assert.AreEqual(1,s.Items.Count);
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.IsNotNull(item);
		}
		#endregion
		
		
		#region PageHeader
		
		[Test]
		public void PageHeader_Should_Contain_RowItem()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.PageHeader;
			BaseReportItem item = s.Items[0];
			Assert.IsInstanceOf<ICSharpCode.Reports.Core.BaseRowItem>(item,
			                                                          "Wrong object, should be 'ICSharpCode.Reports.Core.BaseRowItem");
		}
		
		
		[Test]
		public void PageHeader_Row_Should_Contain_TextItems()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.PageHeader;
			BaseReportItem item = section.Items[0];
			Assert.IsInstanceOf<ICSharpCode.Reports.Core.BaseRowItem>(item,
			                                                          "Wrong object, should be 'ICSharpCode.Reports.Core.BaseRowItem");
		}
		
		#endregion
		
		
		#region Detail
		
		[Test]
		public void PageDetail_Should_Contain_Row()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			BaseReportItem item = s.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseRowItem)));
		}
		
		
		[Test]
		public void Row_Should_Contain_Items()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseRowItem dataRow = (ICSharpCode.Reports.Core.BaseRowItem)s.Items[0];
			var item = dataRow.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseDataItem)));
		}
		
		
		#endregion
		
		
		#region PageFooter
		
		[Test]
		public void PageFooter_Should_Contain_OneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.PageFooter;
			Assert.AreEqual(1,s.Items.Count);
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.IsNotNull(item);
		}
		
		
		[Test]
		public void PageFooter_Should_Contain_PageNumberFunction()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.PageFooter;
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.AreEqual("=Globals!PageNumber",item.Text);
			Assert.AreEqual("PageNumber1",item.Name);
		}
		
		#endregion
		
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			bool createGrouping = false;
			this.reportModel = ReportGenerationHelper.CreateModel(reportName,createGrouping);
			                                             
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
		
		#endregion
		
	}
}
