/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.04.2013
 * Time: 18:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

using ICSharpCode.Reporting.Items;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.Model
{
	[TestFixture]
	public class Report_TwoItemsFixture
	{
		Stream stream;
		
		[Test]
		public void LoadModelWithItems()
		{
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			Assert.That(model,Is.Not.Null);
		}
		
		
		[Test]
		public void ReportHeaderOneItem () {
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			var section = model.ReportHeader;
				Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void PageHeaderOneItem () {
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			var section = model.ReportHeader;
			Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void ItemIsTextItem() {
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			var item = model.ReportHeader.Items[0];
			Assert.That(item,Is.AssignableFrom(typeof(BaseTextItem)));
		}
		
		
		[Test]
		public void IsLocationSet() {
			var rf = new ReportingFactory();
			var model = rf.LoadReportModel(stream);
			var item = model.ReportHeader.Items[0];
			Assert.That(item.Location,Is.Not.EqualTo(Point.Empty));                                   
		}
		
		
		[SetUp]
		public void LoadFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
		}	
	}
}
