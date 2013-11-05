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
	public class ReportTwoItemsFixture
	{

		private ReportModel model;
		
		[Test]
		public void LoadModelWithItems()
		{
			Assert.That(model,Is.Not.Null);
		}
		
		
		[Test]
		public void ReportHeaderOneItem () {
			var section = model.ReportHeader;
			Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void PageHeaderOneItem () {
			var section = model.ReportHeader;
			Assert.That(section.Items.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void ItemIsTextItem() {
			var item = model.ReportHeader.Items[0];
			Assert.That(item,Is.AssignableFrom(typeof(BaseTextItem)));
		}
		

		[Test]
		public void IsLocationSet() {
			var item = model.ReportHeader.Items[0];
			Assert.That(item.Location,Is.Not.EqualTo(Point.Empty));                                   
		}
		
		
		[SetUp]
		public void LoadModelFromStream()
		{
			System.Reflection.Assembly asm = Assembly.GetExecutingAssembly();
			var stream = asm.GetManifestResourceStream(TestHelper.RepWithTwoItems);
			var rf = new ReportingFactory();
			model = rf.LoadReportModel(stream);
		}	
	}
}
