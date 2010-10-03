/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.10.2010
 * Time: 17:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	[TestFixture]
	public class GenertaeTableWithGroupFixture
	{
		
		private const string reportName = "TableBasedReport";
		private ReportModel reportModel;
		
		
		[Test]
		public void PageDetail_First_Item_Should_Table()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			var item = s.Items[0];
			Assert.That(item,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseTableItem)));
		}
		
		
		[Test]
		public void Table_Should_Contain_Three_Rows()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseTableItem table = s.Items[0] as ICSharpCode.Reports.Core.BaseTableItem;
			Assert.That(table.Items.Count,Is.GreaterThanOrEqualTo(3));
		}
		
		
		[Test]
		public void Table_FirstItem_Should_Row ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseTableItem table = s.Items[0] as ICSharpCode.Reports.Core.BaseTableItem;
			var row = table.Items[0];
			Assert.That(row,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseRowItem)));
		}
		
		
		[Test]
		public void Table_SecondItem_Should_GroupRow ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseTableItem table = s.Items[0] as ICSharpCode.Reports.Core.BaseTableItem;
			var row = table.Items[1];
			Assert.That(row,Is.InstanceOf(typeof(ICSharpCode.Reports.Core.BaseGroupedRow)));
		}
		
		#region setup / TearDown
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.reportModel = CreateModel(reportName);
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
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.TableLayout);
			
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
