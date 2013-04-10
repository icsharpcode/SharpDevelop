/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 08.04.2013
 * Time: 20:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class BaseConvertFixture
	{
		private IReportCreator reportCreator;
		
		
		[Test]
		public void CurrentPageContainOneItem() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			Assert.That(page.Items.Count, Is.EqualTo(1));
		}
		
		
		[Test]
		public void PageItemIsBaseExportColumn() {
			reportCreator.BuildExportList();
			var page = reportCreator.Pages[0];
			Assert.That(page.Items[0],Is.InstanceOf(typeof(ExportText)));
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
