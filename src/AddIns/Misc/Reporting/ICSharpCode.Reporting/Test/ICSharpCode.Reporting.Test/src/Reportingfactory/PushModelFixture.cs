/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.06.2013
 * Time: 23:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;
using System.Linq;

using ICSharpCode.Reporting.Interfaces;
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
		
		
		[SetUp]
		public void LoadFromStream()
		{
			var contributorList = new ContributorsList();
			var list = contributorList.ContributorCollection;
			
			
			
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.ReportFromList);
			
//			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
//			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
//			
			var reportingFactory  = new ReportingFactory();
			var model =  reportingFactory.LoadReportModel (stream);
			reportCreator = new DataPageBuilder(model,list);
		}	
	}
}
