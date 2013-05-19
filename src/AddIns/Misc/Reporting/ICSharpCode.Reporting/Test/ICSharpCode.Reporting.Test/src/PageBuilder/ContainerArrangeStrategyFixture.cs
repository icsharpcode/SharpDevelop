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
			var c = CreateContainer();
			c.ExportedItems[0].Location = c.Location;
			strategy.Arrange(c);
			
			var containerRect = new Rectangle(c.Location,c.DesiredSize);
			var itemRect = new Rectangle(c.ExportedItems[0].Location,c.ExportedItems[0].Size);
			
			Console.WriteLine("{0} - {1} - {2} - {3}",containerRect,containerRect.Bottom,itemRect,itemRect.Bottom);
			Assert.That(containerRect.Contains(itemRect));
			
		}
			
		
		[Test]
		public void ItemAtTopBottomOfContainer() {
			var c = CreateContainer();
			c.ExportedItems[0].Location = new Point (c.Location.X,
			                                         c.Location.Y + c.DesiredSize.Height - c.ExportedItems[0].Size.Height);
			strategy.Arrange(c);
		
			var containerRect = new Rectangle(c.Location,c.DesiredSize);
			var itemRect = new Rectangle(c.ExportedItems[0].Location,c.ExportedItems[0].Size);
			
			Console.WriteLine("{0} - {1} - {2} - {3}",containerRect,containerRect.Bottom,itemRect,itemRect.Bottom);
			Assert.That(containerRect.Contains(itemRect));
		}
		
		
		[Test]
		public void FindBiggestRectangle () {
			var c = CreateContainer();
			var secondItem =  new ExportText(){
				Name = "Item1",
				Location = new Point(10,10),
				Size = new Size (60,70)
			};
			c.ExportedItems.Add(secondItem);
			
			strategy.Arrange(c);
			var expected = new Rectangle(secondItem.Location,secondItem.Size);
			Assert.That(strategy.BiggestRectangle,Is.EqualTo(expected));
		}
		
		
		[Test]
		public void ContainerCanGrow () {
			var c = CreateContainer();
			var secondItem =  new ExportText(){
				
				Name = "Item1",
				Location = new Point(10,10),
				Size = new Size (20,70)
			};
			c.ExportedItems.Add(secondItem);
			strategy.Arrange(c);
			var containerRect = new Rectangle(c.Location,c.DesiredSize);
			Console.WriteLine("{0} - {1} - {2} - {3}",containerRect,containerRect.Bottom,strategy.BiggestRectangle,strategy.BiggestRectangle.Bottom);
			Assert.That(containerRect.Contains(strategy.BiggestRectangle));
		}
		
		
		private IExportContainer CreateContainer () {
				
			var container = new ExportContainer(){
				Size = new Size (720,60),
				Location = new Point(50,50),
				Name ="Section"
			};
				
			var item1 = new ExportText(){
				Name = "Item1",
				Location = new Point(55,55),
				Size = new Size (60,20)
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
