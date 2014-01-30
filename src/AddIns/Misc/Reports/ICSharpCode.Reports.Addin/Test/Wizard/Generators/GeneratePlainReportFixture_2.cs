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
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	[TestFixture]
	[Ignore]
	public class GeneratePlainReportFixture_2
	{
		ReportModel mockReportModel;
		
		[Test]
		public void StandartPrinter ()
		{
			ReportSettings settings = this.mockReportModel.ReportSettings;
			Assert.AreEqual(true,settings.UseStandardPrinter,"Standarprinter has should be 'true");
		}
		
		[Test]
		public void PushPullModel ()
		{
			ReportSettings settings = this.mockReportModel.ReportSettings;
			Assert.AreEqual(GlobalEnums.PushPullModel.FormSheet,settings.DataModel,"Wrong PusPullModel");
		}
		
		[Test]
		public void DataAcessStuffShouldBeEmpty ()
		{
			ReportSettings settings = this.mockReportModel.ReportSettings;
			Assert.IsTrue(String.IsNullOrEmpty(settings.ConnectionString),"ConnectionString should be empty");
			Assert.IsTrue(String.IsNullOrEmpty(settings.CommandText),"CommandText should be empty");
			Assert.AreEqual (System.Data.CommandType.Text,settings.CommandType);
		}
		
		
		[Test]
		public void ModelContainFiveSections ()
		{
			Assert.AreEqual(5,this.mockReportModel.SectionCollection.Count);
		}
		
		
		[Test]
		public void PageHeaderShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.mockReportModel.ReportHeader;
			ReportItemCollection c = s.Items;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		
		[Test]
		public void DetailShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.mockReportModel.DetailSection;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		
		[Test]
		public void PageFooterShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.mockReportModel.PageFooter;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		/*
		private ReportModel CreateModel()
		{
			ReportModel m = ReportModel.Create();
			Properties customizer = new Properties();
			
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GeneratePlainReport(m);
			generator.GenerateReport();
			
			ReportLoader rl = new ReportLoader();
//			Console.WriteLine (generator.XmlReport.DocumentElement);
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
	
		[TestFixtureSetUp]
		public void CreateModels ()
		{
			this.mockReportModel = CreateModel();
		}	*/
	}
}
