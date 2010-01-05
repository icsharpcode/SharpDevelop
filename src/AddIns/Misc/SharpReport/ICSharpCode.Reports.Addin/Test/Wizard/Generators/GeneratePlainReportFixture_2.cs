/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.12.2008
 * Zeit: 18:25
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
	
	public class GeneratePlainReportFixture_2
	{
		ReportModel modelForStandardSettings;
		
		[Test]
		public void FileName()
		{
			
			ReportSettings settings = this.modelForStandardSettings.ReportSettings;
			Assert.AreEqual(GlobalValues.PlainFileName,settings.FileName,"Wrong FileName");
		}
		
		[Test]
		public void StandartPrinter ()
		{
			ReportSettings settings = this.modelForStandardSettings.ReportSettings;
			Assert.AreEqual(true,settings.UseStandardPrinter,"Standarprinter has should be 'true");
		}
		
		[Test]
		public void PushPullModel ()
		{
			ReportSettings settings = this.modelForStandardSettings.ReportSettings;
			Assert.AreEqual(GlobalEnums.PushPullModel.FormSheet,settings.DataModel,"Wrong PusPullModel");
		}
		
		[Test]
		public void DataAcessStuffShouldBeEmpty ()
		{
			ReportSettings settings = this.modelForStandardSettings.ReportSettings;
			Assert.IsTrue(String.IsNullOrEmpty(settings.ConnectionString),"ConnectionString should be empty");
			Assert.IsTrue(String.IsNullOrEmpty(settings.CommandText),"CommandText should be empty");
			Assert.AreEqual (System.Data.CommandType.Text,settings.CommandType);
		}
		
		
		[Test]
		public void ModelContainFiveSections ()
		{
			Assert.AreEqual(5,this.modelForStandardSettings.SectionCollection.Count);
		}
		
		
		[Test]
		public void PageHeaderShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.modelForStandardSettings.ReportHeader;
			ReportItemCollection c = s.Items;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		
		[Test]
		public void DetailShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.modelForStandardSettings.DetailSection;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		
		[Test]
		public void PageFooterShouldContainNoItem ()
		{
			ICSharpCode.Reports.Core.BaseSection s = this.modelForStandardSettings.PageFooter;
			Assert.AreEqual(0,s.Items.Count);
		}
		
		
		private ReportModel CreateModel()
		{
			ReportModel m = ReportModel.Create();
			Properties customizer = new Properties();
			
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GeneratePlainReport(m,customizer);
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
			this.modelForStandardSettings = CreateModel();
		}
	}
}
