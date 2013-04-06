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

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class PageBuilderFixture
	{

		private IReportCreator reportCreator;
		
		[Test]
		public void CanCreateFormsPageBuilder()
		{
			Assert.IsNotNull(reportCreator);
		}
		
		
		[Test]
		public void PagesCountIsZero () {
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(0));
		}
		
		
		[Test]
		public void BuildExportPagesCountIsOne() {
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void CurrentPageIsSet() {
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
			var reportingFactory = new ReportingFactory();
			var reportCreator = (FormPageBuilder)reportingFactory.ReportCreator(stream);
			reportCreator.BuildExportList();
			Assert.That(reportCreator.CurrentPage,Is.Not.Null);
		}
		
		
		[Test]
		public void CurrentPageIsFirstPage() {
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
			var reportingFactory = new ReportingFactory();
			var reportCreator = (FormPageBuilder)reportingFactory.ReportCreator(stream);
			reportCreator.BuildExportList();
			Assert.That(reportCreator.CurrentPage.IsFirstPage,Is.True);
			Assert.That(reportCreator.Pages[0].IsFirstPage,Is.True);
		}
		
		
		[Test]
		public void PageInfoPageNumberIsOne() {
			reportCreator.BuildExportList();
			var pi = reportCreator.Pages[0].PageInfo;
			Assert.That(pi.PageNumber,Is.EqualTo(1));
		}

	
		
		[Test]
		public void PageInfoReportName() {
			reportCreator.BuildExportList();
			var pi = reportCreator.Pages[0].PageInfo;
			Assert.That(pi.ReportName,Is.EqualTo("Report1"));
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.PlainReportFileName);
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}
