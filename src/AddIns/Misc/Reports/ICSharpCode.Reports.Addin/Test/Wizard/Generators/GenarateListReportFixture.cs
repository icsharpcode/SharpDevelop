/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 01.04.2009
 * Zeit: 19:32
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;

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
			ReportModel m = ReportGenerationHelper.CreateModel(reportName);
			Assert.AreEqual(reportName,m.ReportSettings.ReportName);
			Assert.AreEqual(1,m.ReportSettings.AvailableFieldsCollection.Count);
			Assert.AreEqual(GlobalEnums.ReportType.DataReport,m.ReportSettings.ReportType);
			Assert.AreEqual(GlobalEnums.PushPullModel.PushData,m.ReportSettings.DataModel);
		}
		
		[Test]
		public void ReportHeaderShouldContainOneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.ReportHeader;
			Assert.AreEqual(1,s.Items.Count);
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.IsNotNull(item);
		}
		
		#region PageHeader
		
		[Test]
		public void PageHeaderShouldContainRowItem()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.PageHeader;
			BaseReportItem item = s.Items[0];
			Assert.IsInstanceOf<ICSharpCode.Reports.Core.BaseRowItem>(item,
			                                                          "Wrong object, should be 'ICSharpCode.Reports.Core.BaseRowItem");
		}
		
		
		[Test]
		public void PageHeaderRowShouldContainTextItems()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.PageHeader;
			BaseReportItem item = section.Items[0];
			Assert.IsInstanceOf<ICSharpCode.Reports.Core.BaseRowItem>(item,
			                                                          "Wrong object, should be 'ICSharpCode.Reports.Core.BaseRowItem");
		}
		
		#endregion
		
		#region Detail
		
		[Test]
		public void PageDetailShouldContainRowItem()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			BaseReportItem item = s.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseRowItem)));
		}
		
		
		[Test]
		public void PageDetailShouldContainDataItems()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseRowItem rowItem = (ICSharpCode.Reports.Core.BaseRowItem)s.Items[0];
			Assert.IsTrue(rowItem.Items.Count > 0);	
			BaseReportItem item = rowItem.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseDataItem)));
		}
		
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
		
		#region Setup/Teardown
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.reportModel = ReportGenerationHelper.CreateModel(reportName);
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
		
		#endregion
		
	}
}
