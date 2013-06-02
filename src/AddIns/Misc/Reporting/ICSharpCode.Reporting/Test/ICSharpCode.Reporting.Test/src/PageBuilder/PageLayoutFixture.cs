/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.04.2013
 * Time: 18:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

using ICSharpCode.Reporting.Exporter;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class PageLayoutFixture
	{
		private IReportCreator reportCreator;
		
		[Test]
		public void PageContainsFourExportContainer()
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
		
		[Test]
		public void SectionsInPageDoNotOverlap()
		{
			Point p = Point.Empty;
			reportCreator.BuildExportList();
			foreach (var item in reportCreator.Pages[0].ExportedItems) {
				var p2 = new Point(item.Location.X,item.Location.Y);
				Console.WriteLine("{0} - {1} - {2}- {3}",p2,item.Size.Height,item.Name,item.DisplayRectangle);
				if (item.Name != "ReportFooter") {
					Assert.That(p2.Y,Is.GreaterThan(p.Y),item.Name);
				}
				p = new Point(item.Location.X,item.Location.Y + item.Size.Height);
			}
		}
		
		
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
