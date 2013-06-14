/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2013
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
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
		public void DetailStartIsSetToOneBelowPageHeader() {
			var reportModel = new ReportModel();
			foreach (GlobalEnums.ReportSection sec in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
				reportModel.SectionCollection.Add (SectionFactory.Create(sec.ToString()));
			}
			var formPageBuilder = new FormPageBuilder(reportModel);
			formPageBuilder.BuildExportList();
			var page = formPageBuilder.Pages[0];
			var pageHeader = page.ExportedItems[1];
			Assert.That(formPageBuilder.DetailStart,
			            Is.EqualTo(new Point(pageHeader.Location.X,
			                                pageHeader.Location.Y + pageHeader.Size.Height + 1)));
		}
		
		
		[Test]
		public void DetailEndsIsOneAbovePageFooter () {
			var reportModel = new ReportModel();
			foreach (GlobalEnums.ReportSection sec in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
				reportModel.SectionCollection.Add (SectionFactory.Create(sec.ToString()));
			}
			var formPageBuilder = new FormPageBuilder(reportModel);
			formPageBuilder.BuildExportList();
			var page = formPageBuilder.Pages[0];
			var pageFooter = page.ExportedItems[3];
				Assert.That(formPageBuilder.DetailEnds,
			            Is.EqualTo(new Point(pageFooter.Location.X,
			                                pageFooter.Location.Y - 1)));
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
		
		
		[Test]
		public void ParentOfSectionsIsPage() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			foreach (var element in page.ExportedItems) {
				Assert.That(element.Parent,Is.Not.Null);
				Assert.That(element.Parent,Is.AssignableTo(typeof(IPage)));
			}
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			var asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var reportingFactory = new ReportingFactory();
			reportCreator = reportingFactory.ReportCreator(stream);
		}
	}
}
