// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
