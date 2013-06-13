/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 19:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;

using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Test;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Model
{
	[TestFixture]
	public class LoadPlainModelFixture
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
			Assert.That(model.ReportSettings.ReportName,Is.EqualTo(Globals.GlobalValues.DefaultReportName));
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
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
		}	
	}
}
