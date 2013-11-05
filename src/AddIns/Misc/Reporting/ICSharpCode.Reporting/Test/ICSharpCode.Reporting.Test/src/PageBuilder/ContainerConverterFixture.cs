/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.04.2013
 * Time: 20:14
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.Converter;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class ContainerConverterFixture
	{
		private IReportContainer container;
		private Graphics graphics;
		
		[Test]
		public void ConverterReturnExportContainer() {
			var converter = new ContainerConverter(graphics,container,new Point(30,30));
			var result = converter.Convert();
			Assert.That(result,Is.InstanceOf(typeof(IExportContainer)));
		}
		
		
		[Test]
		public void ConverterReturnExportContainerwithTwoItems()
		{
			var converter = new ContainerConverter(graphics,container,new Point(30,30));
			var result = converter.Convert();
			Assert.That(result.ExportedItems.Count,Is.EqualTo(2));
		}
		
		
		[Test]
		public void LocationIsAdjusted() {
			var location = new Point(30,30);
			var converter = new ContainerConverter(graphics,container,location);
			var result = converter.Convert();
			Assert.That(result.Location,Is.EqualTo(location));
		}
		
		[Test]
		public void ParentInChildsIsSet () {
			var converter = new ContainerConverter(graphics,container,container.Location);
			var result = converter.Convert();
			foreach (var element in result.ExportedItems) {
				Assert.That(element.Parent,Is.Not.Null);
			}
		}
			
			
		[TestFixtureSetUp]
		public void Init()
		{
			container = new BaseSection(){
				Size = new Size (720,60),
				Location = new Point(50,50),
				Name ="Section"
			};
				
			var item1 = new BaseTextItem(){
				Name = "Item1",
				Location = new Point(10,10),
				Size = new Size (60,20)
			};
			
			var item2 = new BaseTextItem(){
				Name = "Item2",
				Location = new Point(80,10),
				Size = new Size (60,20)
			};
			container.Items.Add(item1);
			container.Items.Add(item2);
			
			Bitmap bitmap = new Bitmap(700,1000);
			graphics = Graphics.FromImage(bitmap);
		}
	}
}
