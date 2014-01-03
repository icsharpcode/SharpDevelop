/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.05.2013
 * Time: 19:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			MakeCangGrow(container);
			Measure(container);
			strategy.Arrange(container);
			
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			
			var arrangedRect = CreateItemRectangle(container);
			Assert.That(containerRect.Contains(arrangedRect));
		}
		
		
		[Test]
		public void ResizedContainerExeed5Points() {
			var container = CreateContainer();
			MakeCangGrow(container);
			Measure(container);
			strategy.Arrange(container);
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var item = container.ExportedItems[0];
			
			var arrangedRect = CreateItemRectangle(container);
			
			Assert.That(containerRect.Bottom,Is.EqualTo(arrangedRect.Bottom));
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
		
		
		static void MakeCangGrow(IExportContainer container)
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
