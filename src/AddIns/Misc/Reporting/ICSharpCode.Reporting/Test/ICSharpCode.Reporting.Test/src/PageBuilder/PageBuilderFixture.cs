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

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
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
		
		
		#region Pages
		
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

			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages[0],Is.Not.Null);
		}
		
		
		[Test]
		public void CurrentPageIsFirstPage() {
			reportCreator.BuildExportList();
			Assert.That(reportCreator.Pages[0].IsFirstPage,Is.True);
		}
		
		#endregion
		
		#region PageInfo
		
		[Test]
		public void PageInfoPageNumberIsOne() {
			reportCreator.BuildExportList();
			var pageInfo = reportCreator.Pages[0].PageInfo;
			Assert.That(pageInfo.PageNumber,Is.EqualTo(1));
		}

	
		[Test]
		public void PageInfoReportName() {
			reportCreator.BuildExportList();
			var pi = reportCreator.Pages[0].PageInfo;
			Assert.That(pi.ReportName,Is.EqualTo("Report1"));
//			Console.WriteLine("----------------");
//			foreach (var page in reportCreator.Pages) {
//				TestHelper.ShowDebug(page);
//			}
		}
		
		/*
		void ShowDebug(IExportContainer container)
		{
			var visitor = new DebugVisitor();
			foreach (var item in container.ExportedItems) {
				if (item is IExportContainer) {
					var a = item as IAcceptor;
					if (a != null) {
						Console.WriteLine("----");
						a.Accept(visitor);
					}
					ShowDebug(item as IExportContainer);
				} else {
					var b = item as IAcceptor;
					if (b != null) {
						b.Accept(visitor);
						
					}
				}
			}
		}
		*/
		
		#endregion
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}
