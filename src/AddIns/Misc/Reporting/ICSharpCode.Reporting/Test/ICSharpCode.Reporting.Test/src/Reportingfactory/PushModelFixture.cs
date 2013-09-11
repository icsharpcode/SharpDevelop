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
using ICSharpCode.Reporting.Items;
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
		public void DataSourceIsSet() {
			var dataPageBuilder = new DataPageBuilder (new ReportModel(),typeof(string),new System.Collections.Generic.List<string>());
			Assert.That(dataPageBuilder.List,Is.Not.Null);
		}
		
		
		[Test]
		public void CanInitDataPageBuilder()
		{
			var dpb = new DataPageBuilder (new ReportModel(),
			                               typeof(string),
			                               new System.Collections.Generic.List<string>());
			Assert.That(dpb,Is.Not.Null);
		}
		
		
		[Test]
		public void CanCreateReportCreatorFromList () {
			var contributorList = new ContributorsList();
			var list = contributorList.ContributorCollection;
			
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.ReportFromList);

			var reportingFactory  = new ReportingFactory();
			var rc = reportingFactory.ReportCreator(stream,typeof(Contributor),list);
			Assert.That(rc,Is.Not.Null);
				Assert.That(rc,Is.TypeOf(typeof(DataPageBuilder)));
		}
	
		
		[Test]
		public void InitPushModelReport()
		{
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages.Count,Is.GreaterThan(0));
		}
		
		
		[Test]
		[Ignore]
		public void PageContainsContainers()
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
		public void ReportContains_2_Pages () {
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(2));
		}
		
		
		[Test]
		public void LastElementInPageIsPageFooter() {
			reportCreator.BuildExportList();
			 
			var firstPage = reportCreator.Pages[1].ExportedItems;
			var firstElement = firstPage.Last();
			Assert.That(firstElement.Name,Is.EqualTo("ReportFooter"));
			
			var lastPage = reportCreator.Pages[1].ExportedItems;
			var lastElement = lastPage.Last();
			Assert.That(lastElement.Name,Is.EqualTo("ReportFooter"));
		}
		
		
		[Test]
		public void FirstElementOnScoundPageIsReportHeader() {
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[1].ExportedItems;
			var result = exporteditems[0];
			Assert.That(result.Name,Is.EqualTo("ReportPageHeader"));
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
			Assert.That(result,Is.AssignableFrom(typeof(ExportText)));
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
			Assert.That(reportCreator.Pages[0].ExportedItems.Count,Is.EqualTo(4));
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
