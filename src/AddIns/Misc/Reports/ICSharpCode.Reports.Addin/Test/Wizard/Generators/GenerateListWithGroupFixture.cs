/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 31.08.2010
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	/// <summary>
	/// Description of GenerateListWithGroupFixture.
	/// </summary>
	[TestFixture]
	public class GenerateListWithGroupFixture
	{
		
		private const string reportName = "ListBasedReport";
		private ReportModel reportModel;
		
	
		[Test]
		public void PageDetail_First_Item_Should_GroupedRow()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			BaseReportItem item = s.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseGroupedRow)));
		}
		
		
		[Test]
		public void GroupHeader_Should_Contain_DataItem()
		{
				ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
				ICSharpCode.Reports.Core.BaseGroupedRow groupedRow = (ICSharpCode.Reports.Core.BaseGroupedRow)s.Items[0];
				var item = groupedRow.Items[0];
				Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseDataItem)));
		}
		
		[Test]
		public void PageDetail_Second_Item_Should_Row()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			BaseReportItem item = s.Items[1];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseRowItem)));
		}
		
		[Test]
		public void PageDetail_Should_Contain_Two_items()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			Assert.That(s.Items.Count.Equals(2));
		}
		
		
		[Test]
		public void PageDetail_First_Item_Should_Contain_GroupedRow()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			Assert.That(s.Items.Count.Equals(2));
		}
		
		
		[Test]
		public void PageDetail_Row_Should_Contain_DataItems()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseRowItem rowItem = (ICSharpCode.Reports.Core.BaseRowItem)s.Items[0];
			Assert.IsTrue(rowItem.Items.Count > 0);	
			BaseReportItem item = rowItem.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseDataItem)));
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
		
		private static ReportModel CreateModel (string reportName)
		{
			
			ReportStructure structure = CreateReportStructure(reportName);
			
			AvailableFieldsCollection abstractColumns = new AvailableFieldsCollection();
			AbstractColumn a1 = new AbstractColumn("Field1",typeof(System.String));
			structure.AvailableFieldsCollection.Add(a1);
			
			ICSharpCode.Reports.Core.BaseDataItem bri = new ICSharpCode.Reports.Core.BaseDataItem();
			bri.Name ="Field1";
			structure.ReportItemCollection.Add(bri);
			
			structure.Grouping = "group";
			
			ReportModel m = structure.CreateAndFillReportModel();
			ICSharpCode.Core.Properties customizer = new ICSharpCode.Core.Properties();
			
			customizer.Set("Generator", structure);
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GeneratePushDataReport(m,customizer);
		
			generator.GenerateReport();
			
			ReportLoader rl = new ReportLoader();
			object root = rl.Load(generator.XmlReport.DocumentElement);
			ReportModel model = root as ReportModel;
			if (model != null) {
				model.ReportSettings.FileName = GlobalValues.PlainFileName;
				FilePathConverter.AdjustReportName(model);
			} else {
				throw new InvalidReportModelException();
			}
			return model;
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
