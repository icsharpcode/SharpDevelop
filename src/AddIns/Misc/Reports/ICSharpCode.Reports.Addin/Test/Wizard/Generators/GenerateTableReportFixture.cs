// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	[TestFixture]
	public class GenerateTableReportFixture
	{
		private const string reportName = "TableBasedReport";
		ReportModel reportModel;
		
		#region General generated Properties
		
		[Test]
		public void InitModel()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			Assert.AreEqual(reportName,m.ReportSettings.ReportName);
			Assert.AreEqual(1,m.ReportSettings.AvailableFieldsCollection.Count);
			Assert.AreEqual(GlobalEnums.ReportType.DataReport,m.ReportSettings.ReportType);
		}
		
		
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
		
		#region Sort/Group
		
		[Test]
		public void GroupColumCollection_Grouping_Should_Set()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			var rs = m.ReportSettings;
			
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			rs.GroupColumnsCollection.Add(gc);
			
			Assert.AreEqual(rs.GroupColumnsCollection.Count,1);
		}
		
		
		[Test]
		public void SortColumCollection_Sorting_Should_Set()
		{
			ReportModel m = ReportGenerationHelper.CreateModel(reportName,false);
			var rs = m.ReportSettings;
			
			SortColumn gc = new SortColumn("GroupItem",ListSortDirection.Ascending);
			rs.SortColumnsCollection.Add(gc);
			
			Assert.AreEqual(rs.SortColumnsCollection.Count,1);
		}
		#endregion
		
		
		#region PageHeader
		
//		[Test]
//		public void PageDetailShouldContainRowItem()
//		{
//			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
//			BaseReportItem item = s.Items[0];
//			Assert.IsInstanceOfType(typeof(ICSharpCode.Reports.Core.BaseRowItem),item);
//		}
		#endregion
		
		
		#region PageFooter
		
		[Test]
		public void PageFooterShouldContainOneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.PageFooter;
			Assert.AreEqual(1,s.Items.Count);
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.IsNotNull(item);
		}
		
		
		[Test]
		public void PageFooterContainsPageNumberFunction()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.PageFooter;
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.AreEqual("=Globals!PageNumber",item.Text);
			Assert.AreEqual("PageNumber1",item.Name);
		}
		#endregion
		
		
		#region SetupTearDown
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.reportModel = ReportGenerationHelper.CreateModel(reportName,false);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
		
		#endregion
		
	}
}
