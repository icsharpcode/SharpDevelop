// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder.Converter;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class SectionWithContainerFixture
	{
		IReportContainer section;
		Graphics graphics;
		
		[Test]
		public void SectionContainsRowWithName() {
			Assert.That(section.Items[0],Is.AssignableTo(typeof(BaseRowItem)));
			Assert.That(section.Items[0].Name,Is.EqualTo("Row1"));
		}
		
		
		[Test]
		public void SectionContainsOneItemThatIsRow() {
			var converter = new ContainerConverter(section.Location);
			var list = converter.CreateConvertedList(section.Items);
			Assert.That(list.Count,Is.EqualTo(1));
		}
		
		
		[Test]
		public void RowContainsOneItem() {
			var converter = new ContainerConverter(section.Location);
			var list = converter.CreateConvertedList(section.Items);
			var item = list[0] as ExportContainer;
			var text = item.ExportedItems[0];
			Assert.That(text,Is.AssignableTo(typeof(ExportText)));
		}
		
		
		[SetUp]
		public void Init()
		{
			section = new BaseSection(){
				Size = new Size (720,60),
				Location = new Point(50,50),
				Name ="Section"
			};
			
			var row = new BaseRowItem(){
				Name = "Row1"
			};
			
				row.Items.Add(new BaseTextItem(){
				              	Name = "Item1",
				              	Location = new Point(10,10),
				              	Size = new Size (60,20)
				              });
			
			
			section.Items.Add(row);
			Bitmap bitmap = new Bitmap(700,1000);
			graphics = Graphics.FromImage(bitmap);
		}
	}
}
