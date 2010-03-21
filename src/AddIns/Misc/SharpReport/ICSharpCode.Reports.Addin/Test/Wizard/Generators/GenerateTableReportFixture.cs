/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 05.04.2009
 * Zeit: 17:56
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
	public class GenerateTableReportFixture
	{
		private const string reportName = "TableBasedReport";
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
