/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.10.2010
 * Time: 17:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	[TestFixture]
	public class GenertaeTableWithGroupFixture
	{
		
		private const string reportName = "TableBasedReport";
		private ReportModel reportModel;
		
		
		[Test]
		public void Table_Should_Contains_GroupedHeader()
		{
			ICSharpCode.Reports.Core.BaseTableItem table = CreateContainer();
			//GroupHeader
			var c =  new Collection<ICSharpCode.Reports.Core.GroupHeader>(table.Items.OfType<ICSharpCode.Reports.Core.GroupHeader>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void Table_Should_Contain_DataRow()
		{
			ICSharpCode.Reports.Core.BaseTableItem table = CreateContainer();
			//DataRow
			var c =  new Collection<ICSharpCode.Reports.Core.BaseRowItem>(table.Items.OfType<ICSharpCode.Reports.Core.BaseRowItem>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void Table_Should_Contain_GroupFooter()
		{
			ICSharpCode.Reports.Core.BaseTableItem table = CreateContainer();
			//GroupFooter
			var c =  new Collection<ICSharpCode.Reports.Core.GroupFooter>(table.Items.OfType<ICSharpCode.Reports.Core.GroupFooter>().ToList());
			Assert.That(c.Count,Is.GreaterThanOrEqualTo(1));
		}
		
		
		[Test]
		public void PageDetail_Should_Contain_Four_Items()
		{
			ICSharpCode.Reports.Core.BaseTableItem table = CreateContainer();
			Assert.That(table.Items.Count.Equals(4));
		}
		
		
		private ICSharpCode.Reports.Core.BaseTableItem CreateContainer ()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.reportModel.DetailSection;
			ICSharpCode.Reports.Core.BaseTableItem table = section.Items[0] as ICSharpCode.Reports.Core.BaseTableItem;
			return table;
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
