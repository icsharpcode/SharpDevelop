/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.12.2008
 * Zeit: 19:22
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	
	[TestFixture]
	public class GenerateFormsSheetReportFixture
	{
		
		ReportModel model ;
		
		[Test]
		public void ModelContainFiveSections ()
		{
			Assert.AreEqual(5,this.model.SectionCollection.Count);
		}
		
		
		[Test]
		public void PageHeaderShouldContainOneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.model.ReportHeader;
			ReportItemCollection c = s.Items;
			Assert.AreEqual(1,s.Items.Count);
			ICSharpCode.Reports.Core.BaseReportItem item = s.Items[0];
			Assert.IsNotNull(item);
		}
		
		
		[Test]
		public void DetailShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.model.DetailSection;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		
		[Test]
		public void PageFooterShouldContainOneItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.model.PageFooter;
			Assert.AreEqual(1,s.Items.Count);
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.IsNotNull(item);
		}
		
		
		[Test]
		public void PageFooterContainsPageNumberFunction()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.model.PageFooter;
			ICSharpCode.Reports.Core.BaseTextItem item = (ICSharpCode.Reports.Core.BaseTextItem)s.Items[0];
			Assert.AreEqual("=Globals!PageNumber",item.Text);
			Assert.AreEqual("PageNumber1",item.Name);
		}
		
		
		
		private ReportModel CreateModel ()
		{
			ReportModel m = ReportModel.Create();
			Properties customizer = new Properties();
			
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GenerateFormSheetReport(m,customizer);
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
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.model = this.CreateModel();
		}
	}
}
