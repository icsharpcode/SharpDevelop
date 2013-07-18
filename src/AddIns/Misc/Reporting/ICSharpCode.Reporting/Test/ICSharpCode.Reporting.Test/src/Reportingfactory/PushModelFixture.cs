/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.06.2013
 * Time: 23:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using ICSharpCode.Reporting.Test.DataSource;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Reportingfactory
{
	[TestFixture]
	public class PushModelFixture
	{

		private IReportCreator reportCreator;
		
		[Test]
		public void InitPushModelReport()
		{
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages.Count,Is.GreaterThan(0));
		}
		
		
		[Test]
		public void FirstPageContains_4_Sections()
		{
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[0].ExportedItems;
			var sections = from s in exporteditems
				where s.GetType() == typeof(ExportContainer)
				select s;
			Assert.That(sections.ToList().Count,Is.EqualTo(4));
			var ex = new DebugExporter(reportCreator.Pages);
			ex.Run();
		}
		
		
		[Test]
		public void LastPageContains_3_Sections()
		{
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[1].ExportedItems;
			var sections = from s in exporteditems
				where s.GetType() == typeof(ExportContainer)
				select s;
			Assert.That(sections.ToList().Count,Is.EqualTo(3));
		}
	
		
		[Test]
		public void DetailContainsOneDataItem() {
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[0].ExportedItems;
			var sections = from s in exporteditems
				where s.GetType() == typeof(ExportContainer)
				select s;
			var section = sections.ToList()[2] as ExportContainer;
			var result = section.ExportedItems[0];
			Assert.That(result,Is.AssignableFrom(typeof(ExportContainer)));
		}
		
		
		[Test]
		public void ParentOfSectionsIsPage() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			foreach (var element in page.ExportedItems) {
				Assert.That(element.Parent,Is.Not.Null);
				Assert.That(element.Parent,Is.AssignableTo(typeof(IPage)));
			}
		}
		
		
		[Test]
		public void HandleEmptyList () {
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.ReportFromList);

			var reportingFactory  = new ReportingFactory();
			var model =  reportingFactory.LoadReportModel (stream);
			reportCreator = new DataPageBuilder(model,typeof(string),new List<string>());
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages[0].ExportedItems.Count,Is.EqualTo(5));
		}
		
		[SetUp]
		public void LoadFromStream()
		{
			var contributorList = new ContributorsList();
			var list = contributorList.ContributorCollection;
			
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.ReportFromList);

			var reportingFactory  = new ReportingFactory();
			var model =  reportingFactory.LoadReportModel (stream);
			reportCreator = new DataPageBuilder(model,typeof(Contributor),list);
		}	
	}
}
