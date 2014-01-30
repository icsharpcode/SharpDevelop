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
using System.Linq;

using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard.Generators
{
	
	[TestFixture]
	[Ignore]
	public class GenerateTableWithGroupFixture
	{
		/*
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
			
			IReportGenerator generator = new GeneratePushDataReport(m);
		
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
		*/
	}
}
