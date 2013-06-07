/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.06.2013
 * Time: 20:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Reflection;
using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class DataPageBuilderFixture
	{
		private IReportCreator reportCreator;
		
		[Test]
		public void CanInitDataPageBuilder()
		{
			var dpb = new DataPageBuilder (new ReportModel(),new System.Collections.Generic.List<string>());
//			dpb.DataSource(new ReportModel(),new System.Collections.Generic.List<string>());
			Assert.That(dpb,Is.Not.Null);
		}
		
		
		[Test]
		public void PageContainsFiveSections()
		{
			reportCreator.BuildExportList();
			var x = reportCreator.Pages[0].ExportedItems;
			var y = from s in x 
				where s.GetType() == typeof(ExportContainer)
				select s;
			Assert.That(y.ToList().Count,Is.EqualTo(5));
			Console.WriteLine("-------ShowDebug---------");
			var ex = new DebugExporter(reportCreator.Pages);
			ex.Run();
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var reportingFactory = new ReportingFactory();
//			reportCreator = reportingFactory.ReportCreator(stream);
			var model =  reportingFactory.LoadReportModel (stream);
			reportCreator = new DataPageBuilder(model,new System.Collections.Generic.List<string>());
		}
	}
}
