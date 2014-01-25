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
