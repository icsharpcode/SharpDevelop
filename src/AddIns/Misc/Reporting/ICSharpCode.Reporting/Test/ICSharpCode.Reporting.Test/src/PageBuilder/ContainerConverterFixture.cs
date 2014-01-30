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
