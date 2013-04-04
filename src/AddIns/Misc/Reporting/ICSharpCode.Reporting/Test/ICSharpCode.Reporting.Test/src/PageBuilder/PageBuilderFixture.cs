/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;

using ICSharpCode.Reporting.BaseClasses;
using ICSharpCode.Reporting.PageBuilder;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class PageBuilderFixture
	{
		private Stream stream;
		
		[Test]
		public void CanCreateFormsPageBuilder()
		{
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator(stream);
			Assert.IsNotNull(reportCreator);
		}
		
		
		[Test]
		public void PagesCountIsZero () {
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator(stream);
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(0));
		}
		
		
		[Test]
		public void BuildExportPagesCountIsOne() {
			var reportingFactory = new ReportingFactory();
			var reportCreator = reportingFactory.ReportCreator(stream);
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(1));
		}
			
		
		[Test]
		public void CurrentPageIsSet() {
			var reportingFactory = new ReportingFactory();
			var reportCreator = (FormPageBuilder)reportingFactory.ReportCreator(stream);
			reportCreator.BuildExportList();
			Assert.That(reportCreator.CurrentPage,Is.Not.Null);
		}
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
		}
	}
}
