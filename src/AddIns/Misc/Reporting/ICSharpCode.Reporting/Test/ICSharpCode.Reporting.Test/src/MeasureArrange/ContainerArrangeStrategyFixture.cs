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
		Graphics graphics = CreateGraphics.FromSize (new Size(1000,1000));
		
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
			container.ExportedItems[0].Location = container.Location;
			Measure(container);
			strategy.Arrange(container);
			
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var itemRect = new Rectangle(container.ExportedItems[0].Location,container.ExportedItems[0].Size);
			
			Assert.That(containerRect.Contains(itemRect));
		}
			
		
		
		[Test]
		public void ContainerResizeIfItemCanGrow () {
			
			var container = CreateContainer();
//			MakeCangGrow(container);
			Measure(container);
			strategy.Arrange(container);
			strategy.Arrange(container);
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			
			var child = container.ExportedItems[0];
			var childLocation = new Point(containerRect.Left + child.Location.X,containerRect.Top + child.Location.Y);
			var childRect = new Rectangle(childLocation,child.DesiredSize);

			Assert.That(containerRect.Contains(childRect));
		}
		
		
		[Test]
		public void ContainerIs_5_Below_LargestItem() {
			
			var container = CreateContainer();
			MakeCangGrow(container);
			Measure(container);
			strategy.Arrange(container);
			
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			
			var child = container.ExportedItems[0];
			var childLocation = new Point(containerRect.Left + child.Location.X,containerRect.Top + child.Location.Y);
			var childRect = new Rectangle(childLocation,child.DesiredSize);
			Assert.That(containerRect.Bottom,Is.EqualTo(childRect.Bottom + 5));
		}
		
		
		[Test]
		public void ResizedContainerExeed5Points() {
			var container = CreateContainer();
			MakeCangGrow(container);
			Measure(container);
			strategy.Arrange(container);
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var arrangeRect = new Rectangle(new Point(container.Location.X + strategy.BiggestRectangle.Left,
			                                          strategy.BiggestRectangle.Top),
			                                strategy.BiggestRectangle.Size);
			
			Assert.That(containerRect.Bottom,Is.EqualTo(arrangeRect.Bottom + 5));
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
			mes.Measure(container, graphics);
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
