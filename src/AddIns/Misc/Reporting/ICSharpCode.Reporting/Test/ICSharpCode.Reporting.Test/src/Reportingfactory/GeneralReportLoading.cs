/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.09.2013
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;
using ICSharpCode.Reporting.Globals;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Reportingfactory
{
	[TestFixture]
	public class GeneralReportLoading
	{
		private Stream stream;
		
		[Test]
		public void CanLoadFromResource()
		{
			Assert.IsNotNull(stream);
		}
		
		
		[Test]
		public void LoadPlainModel()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.IsNotNull(model);
		}
		
		
		[Test]
		public void ReportSettingsFromPlainModel()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings,Is.Not.Null);
		}
		
		
		[Test]
		public void ReportSettingsReportName()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings.ReportName,Is.EqualTo(GlobalValues.DefaultReportName));
		}
		
		
		[Test]
		public void ReportSettingsDataModelFormSheet()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings.DataModel,Is.EqualTo(GlobalEnums.PushPullModel.FormSheet));
		}
		
		
		[Test]
		public void ReportSettingsPageSize()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model.ReportSettings.PageSize,Is.EqualTo(Globals.GlobalValues.DefaultPageSize));
		}
		
		
		
		[SetUp]
		public void LoadFromStream()
		{
			var asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
		}	
	}
}
