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
using System.Collections.Generic;
using System.Drawing;
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
			var dataPageBuilder = new DataPageBuilder (new ReportModel(),new System.Collections.Generic.List<string>());
			Assert.That(dataPageBuilder.List,Is.Not.Null);
		}
		
		
		[Test]
		public void CanInitDataPageBuilder()
		{
			var dpb = new DataPageBuilder (new ReportModel(),
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
			var rc = reportingFactory.ReportCreator(stream,list);
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
		public void PageContainsContainers()
		{
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[0].ExportedItems;
			var sections = from s in exporteditems
				where s.GetType() == typeof(ExportContainer)
				select s;
			Assert.That(sections.ToList().Count,Is.GreaterThan(0));
//			var ex = new DebugExporter(reportCreator.Pages);
//			ex.Run();
		}
		
		
		[Test]
		public void ReportContains_2_Pages () {
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(2));
		}
		
		
		[Test]
		public void LastElementInEveryPageIsPageFooter() {
			reportCreator.BuildExportList();
			 
			var firstPage = reportCreator.Pages[1].ExportedItems;
			var firstElement = firstPage.Last();
			Assert.That(firstElement.Name,Is.EqualTo("ReportFooter"));
			
			var lastPage = reportCreator.Pages[1].ExportedItems;
			var lastElement = lastPage.Last();
			Assert.That(lastElement.Name,Is.EqualTo("ReportFooter"));
		}
		
		
		[Test]
		public void FirstElementOnSecondPageIsReportHeader() {
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[1].ExportedItems;
			var result = exporteditems[0];
			Assert.That(result.Name,Is.EqualTo("ReportPageHeader"));
		}
		
		
		[Test]
		public void RowsHasGapOfOne () {
			reportCreator.BuildExportList();
			var exporteditems = reportCreator.Pages[0].ExportedItems;
			for (int i = 1; i < exporteditems.Count -1; i++) {
				Assert.That(exporteditems[i].Location.Y,Is.EqualTo(exporteditems[i-1].DisplayRectangle.Bottom +1));
			}
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
			reportCreator = new DataPageBuilder(model,new List<string>());
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
			reportCreator = new DataPageBuilder(model,list);
		}	
	}
}
