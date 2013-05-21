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
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;
using NUnit.Framework;

namespace ICSharpCode.Reporting.Test.PageBuilder
{
	[TestFixture]
	public class ContainerArrangeStrategyFixture
	{
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
			strategy.Arrange(container);
			
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var itemRect = new Rectangle(container.ExportedItems[0].Location,container.ExportedItems[0].Size);
			
//			Console.WriteLine("{0} - {1} - {2} - {3}",containerRect,containerRect.Bottom,itemRect,itemRect.Bottom);
			Assert.That(containerRect.Contains(itemRect));
			
		}
			
		
		[Test]
		public void ItemAtTopBottomOfContainer() {
			var container = CreateContainer();
			container.ExportedItems[0].Location = new Point (container.Location.X,
			                                         container.Location.Y + container.DesiredSize.Height - container.ExportedItems[0].Size.Height);
			strategy.Arrange(container);
		
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var itemRect = new Rectangle(container.ExportedItems[0].Location,container.ExportedItems[0].Size);
			
//			Console.WriteLine("{0} - {1} - {2} - {3}",containerRect,containerRect.Bottom,itemRect,itemRect.Bottom);
			Assert.That(containerRect.Contains(itemRect));
		}
		
		
		[Test]
		public void FindBiggestRectangle () {
			var container = CreateContainer();
			var secondItem = CreateCanGrowText(container);
			container.ExportedItems.Add(secondItem);
			
			strategy.Arrange(container);
			var expected = new Rectangle(new Point(container.Location.X + secondItem.Location.X,
			                                       container.Location.Y + secondItem.Location.Y),
			                                       secondItem.Size);
			Assert.That(strategy.BiggestRectangle,Is.EqualTo(expected));
		}
		
		
		[Test]
		public void ContainerResizeIfItemCanGrow () {
			var container = CreateContainer();
			
			container.ExportedItems.Add(CreateCanGrowText(container));
			strategy.Arrange(container);
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var arrangeRect = new Rectangle(new Point(container.Location.X + strategy.BiggestRectangle.Left,
			                                          strategy.BiggestRectangle.Top),
			                                          strategy.BiggestRectangle.Size);
			
//			Console.WriteLine("{0} - {1} - {2} - {3}",containerRect,containerRect.Bottom,strategy.BiggestRectangle,strategy.BiggestRectangle.Bottom);
			Assert.That(containerRect.Contains(arrangeRect));
			Assert.That(containerRect.Bottom,Is.EqualTo(arrangeRect.Bottom + 5));
		}
		
		[Test]
		public void ResizedContainerExeed5Points() {
			var container = CreateContainer();
			container.ExportedItems.Add(CreateCanGrowText(container));
			strategy.Arrange(container);
			var containerRect = new Rectangle(container.Location,container.DesiredSize);
			var arrangeRect = new Rectangle(new Point(container.Location.X + strategy.BiggestRectangle.Left,
			                                          strategy.BiggestRectangle.Top),
			                                strategy.BiggestRectangle.Size);
			
			Assert.That(containerRect.Bottom,Is.EqualTo(arrangeRect.Bottom + 5));
		}
		
		
		private IExportText CreateCanGrowText(IExportContainer container) {
			var secondItem =  new ExportText(){
				Name = "Item1",
				Location = new Point(80,10),
				Size = new Size (20,70),
				DesiredSize = new Size (20,70),
				CanGrow = true,
				Parent = container
			};
			return secondItem;
		}
		
		
		private IExportContainer CreateContainer () {
				
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
			container.DesiredSize = container.Size;
			return container;
		}
			
		[TestFixtureSetUp]
		public void Init()
		{
			strategy = new ContainerArrangeStrategy();
		}
	}
}
