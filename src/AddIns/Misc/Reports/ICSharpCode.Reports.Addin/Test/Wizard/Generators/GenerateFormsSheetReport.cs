// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	
	[TestFixture]
	public class GenerateFormsSheetReportFixture
	{
		
		ReportModel createdReportModel ;
		
		[Test]
		public void ModelContainFiveSections ()
		{
			Assert.AreEqual(5,this.createdReportModel.SectionCollection.Count);
		}
		
		
		[Test]
		public void PageHeaderShouldContainOneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.createdReportModel.ReportHeader;
			ReportItemCollection c = section.Items;
			Assert.AreEqual(1,section.Items.Count);
			ICSharpCode.Reports.Core.BaseReportItem item = section.Items[0];
			Assert.IsNotNull(item);
		}
		
		
		[Test]
		public void DetailShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.createdReportModel.DetailSection;
			Assert.AreEqual(0,section.Items.Count);
		}
		
		
		[Test]
		public void PageFooterShouldContainOneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.createdReportModel.PageFooter;
			Assert.AreEqual(1,section.Items.Count);
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)section.Items[0];
			Assert.IsNotNull(item);
		}
		
		
		[Test]
		public void PageFooterContainsPageNumberFunction()
		{
			ICSharpCode.Reports.Core.BaseSection section = this.createdReportModel.PageFooter;
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)section.Items[0];
			Assert.AreEqual("=Globals!PageNumber",item.Text);
			Assert.AreEqual("PageNumber1",item.Name);
		}
		
		
		
		private ReportModel CreateModel ()
		{
			ReportModel m = ReportModel.Create();

			ReportStructure reportStructure = new ReportStructure()
			{
				ReportLayout = GlobalEnums.ReportLayout.ListLayout
			};
			
			IReportGenerator generator = new GenerateFormSheetReport(m,reportStructure);
			generator.GenerateReport();
			
			ReportLoader loader = new ReportLoader();
			object root = loader.Load(generator.XmlReport.DocumentElement);
			
			ReportModel model = root as ReportModel;
			
			model.ReportSettings.FileName = GlobalValues.PlainFileName;
			FilePathConverter.AdjustReportName(model);
			return model;
		}
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.createdReportModel = this.CreateModel();
		}

	}
	
}
