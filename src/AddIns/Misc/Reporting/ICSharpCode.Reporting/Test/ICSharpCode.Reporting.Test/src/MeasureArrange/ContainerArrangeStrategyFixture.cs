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
using System.Linq;

using ICSharpCode.Reporting.Arrange;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.MeasureArrange
{
	[TestFixture]
	public class ContainerArrangeStrategyFixture
	{
		readonly Graphics graphics = CreateGraphics.FromSize (new Size(1000,1000));
		
		ContainerArrangeStrategy strategy;
		
		[Test]
		public void ContainerNoChildren() {
			var param = new ExportContainer();
			var size = param.Size;
			strategy.Arrange(param);
			Assert.That(param.Size, Is.EqualTo(size));
		}
		
		
		[Test]
		public void ItemAtTopOfContainer() {
			var container = CreateContainer();

			container.ExportedItems[0].Location = Point.Empty;
			Measure(container);
			strategy.Arrange(container);
			
			var containerRect = new Rectangle(container.Location,container.DesiredSize);

			var arrangedRect = CreateItemRectangle(container);
			Assert.That(containerRect.Contains(arrangedRect));
		}
		
		
		[Test]
		public void ContainerNotResizeIfCanGrowEqualFalse () {
			var container = CreateContainer();
			Measure(container);
			strategy.Arrange(container);

			Assert.That(container.Size,Is.EqualTo(container.DesiredSize));
		}
		
		
		[Test]
		public void ContainerResizeIfItemCanGrow () {
			
			var container = CreateContainer();
			SetCanGrowToTrue(container);
			Measure(container);
			strategy.Arrange(container);
			
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			
			var arrangedRect = CreateItemRectangle(container);
			Assert.That(containerRect.Contains(arrangedRect));
		}
		
		
		[Test]
		public void ResizedContainerExeed20Points() {
			var container = CreateContainer();
			SetCanGrowToTrue(container);
			Measure(container);
			strategy.Arrange(container);
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var item = container.ExportedItems[0];
			
			var arrangedRect = CreateItemRectangle(container);
			
			Assert.That(containerRect.Bottom,Is.EqualTo(arrangedRect.Bottom + 20));
		}
		
		
		[Test]
		public void ContainerContainsTwoItems_OneCanGrow () {
			var container = CreateContainer();
			var item1 = new ExportText(){
				CanGrow = true,
				Name = "Item1",
				Location = new Point(80,20),
				Size = new Size (60,70),
				Parent = container
			};
			container.ExportedItems.Add(item1);
			
			Measure(container);
			strategy.Arrange(container);
		
			foreach (var element in container.ExportedItems) {
				var arrangedRect = new Rectangle(container.Location.X + element.Location.X,container.Location.Y + element.Location.Y,
				                             element.Size.Width,element.Size.Height);
			
				Assert.That(container.DisplayRectangle.IntersectsWith(arrangedRect));
				Assert.That(container.DisplayRectangle.Contains(arrangedRect));
			}
		}
		
		
		Rectangle CreateItemRectangle(IExportContainer container)
		{
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var child = container.ExportedItems[0];
			var childLocation = new Point(containerRect.Left + child.Location.X, containerRect.Top + child.Location.Y);
			var childRect = new Rectangle(childLocation, child.DesiredSize);
			return childRect;
		}
		
		
		static void SetCanGrowToTrue(IExportContainer container)
		{
			container.ExportedItems[0].Location = new Point(80, 10);
			container.ExportedItems[0].Size = new Size(20, 70);
			container.ExportedItems[0].CanGrow = true;
		}
		
		
		
		void Measure(IExportColumn container)
		{
			var mes = container.MeasurementStrategy();
			container.DesiredSize = mes.Measure(container, graphics);
		}
		
		
		IExportContainer CreateContainer () {
			
			var container = new ExportContainer(){
				Size = new Size (720,60),
				Location = new Point(50,50),
				Name ="Section"
			};
			
			var item1 = new ExportText(){
				Name = "Item1",
				Location = new Point(10,10),
				Size = new Size (60,20),
				Parent = container
			};
			
			container.ExportedItems.Add(item1);
			return container;
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
			strategy = new ContainerArrangeStrategy();
		}
	}
}
