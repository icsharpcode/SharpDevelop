// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using NUnit.Framework;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Extensions;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class PlacementTests : ModelTestHelper
	{
		internal static void Move(Vector v, params DesignItem[] items)
		{
			PlacementOperation operation = PlacementOperation.Start(items, PlacementType.Move);
			foreach (PlacementInformation info in operation.PlacedItems) {
				info.Bounds = new Rect(info.OriginalBounds.Left + v.X,
				                       info.OriginalBounds.Top + v.Y,
				                       info.OriginalBounds.Width,
				                       info.OriginalBounds.Height);
				operation.CurrentContainerBehavior.SetPosition(info);
			}
			operation.Commit();
		}
		
		internal static void Resize(Rect rect,params DesignItem[] items)
		{
			PlacementOperation operation = PlacementOperation.Start(items, PlacementType.Resize);
			foreach(var info in operation.PlacedItems) {
				info.Bounds = rect;
				operation.CurrentContainerBehavior.SetPosition(info);
			}
			operation.Commit();
		}
		
		[Test]
		[Ignore("Currently bounds calculated using visuals")]
		public void MoveFixedWidthButton()
		{
			DesignItem button = CreateCanvasContext("<Button Width='100' Height='200'/>");
			Move(new Vector(50, 25), button);
			AssertCanvasDesignerOutput(@"<Button Width=""100"" Height=""200"" Canvas.Left=""50"" Canvas.Top=""25"" />", button.Context);
		}
		
		[Test]
		public void MoveAutoWidthButton()
		{
			DesignItem button = CreateCanvasContext("<Button/>");
			Move(new Vector(50, 25), button);
			AssertCanvasDesignerOutput(@"<Button Canvas.Left=""50"" Canvas.Top=""25"" />", button.Context);
		}
		
		#region Grid Placement Tests
		private DesignItem _buttonInGridWithAutoSize;
		private DesignItem _buttonIsGridWithFixedSize;
		
		[TestFixtureSetUp]
		public void Intialize()
		{
			_buttonInGridWithAutoSize=CreateGridContext("<Button/>");
			Move(new Vector(50,25),_buttonInGridWithAutoSize);
			
			_buttonIsGridWithFixedSize=CreateGridContext("<Button HorizontalAlignment=\"Left\" VerticalAlignment=\"Top\" Width=\"50\" Height=\"50\"/>");
			Move(new Vector(50,25),_buttonIsGridWithFixedSize);
		}
		
		[Test]
		public void AssertAlignmentsForAutoSize()
		{
			Assert.AreEqual(HorizontalAlignment.Stretch,_buttonInGridWithAutoSize.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance);
			Assert.AreEqual(VerticalAlignment.Stretch,_buttonInGridWithAutoSize.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance);
		}
		
		[Test]
		public void AssertAlignmentForFixedSize()
		{
			Assert.AreEqual(HorizontalAlignment.Left,_buttonIsGridWithFixedSize.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance);
			Assert.AreEqual(VerticalAlignment.Top,_buttonIsGridWithFixedSize.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance);
		}
		
		[Test]
		public void AssertMarginForAutoSize()
		{
			Assert.AreEqual(new Thickness(50,25,-50,-25),_buttonInGridWithAutoSize.Properties[FrameworkElement.MarginProperty].ValueOnInstance);
		}
		
		[Test]
		public void AssertMarginForFixedSize()
		{
			Assert.AreEqual(new Thickness(50,25,0,0),_buttonIsGridWithFixedSize.Properties[FrameworkElement.MarginProperty].ValueOnInstance);
		}
		
		[Test]
		public void AssertRowColumnForAutoSize()
		{
			Assert.AreEqual(0,_buttonInGridWithAutoSize.Properties.GetAttachedProperty(Grid.RowProperty).ValueOnInstance);
			Assert.AreEqual(1,_buttonInGridWithAutoSize.Properties.GetAttachedProperty(Grid.RowSpanProperty).ValueOnInstance);
			Assert.AreEqual(0,_buttonInGridWithAutoSize.Properties.GetAttachedProperty(Grid.ColumnProperty).ValueOnInstance);
			Assert.AreEqual(1,_buttonInGridWithAutoSize.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).ValueOnInstance);
		}
		
		[Test]
		public void AssetRowColumnForFixedSize()
		{
			Assert.AreEqual(0, _buttonIsGridWithFixedSize.Properties.GetAttachedProperty(Grid.RowProperty).ValueOnInstance);
			Assert.AreEqual(1, _buttonIsGridWithFixedSize.Properties.GetAttachedProperty(Grid.RowSpanProperty).ValueOnInstance);
			Assert.AreEqual(0, _buttonIsGridWithFixedSize.Properties.GetAttachedProperty(Grid.ColumnProperty).ValueOnInstance);
			Assert.AreEqual(1, _buttonIsGridWithFixedSize.Properties.GetAttachedProperty(Grid.ColumnSpanProperty).ValueOnInstance);
		}
		
		[Test]
		public void AssertSizeForAutoSize()
		{
			Assert.AreEqual(double.NaN,_buttonInGridWithAutoSize.Properties[FrameworkElement.HeightProperty].ValueOnInstance);
			Assert.AreEqual(double.NaN,_buttonInGridWithAutoSize.Properties[FrameworkElement.WidthProperty].ValueOnInstance);
		}
		
		[Test]
		[Ignore("Bounds calculated using Visuals")]
		public void AssertSizeForFixedSize()
		{
			Assert.AreEqual(50,_buttonIsGridWithFixedSize.Properties[FrameworkElement.HeightProperty].ValueOnInstance);
			Assert.AreEqual(50,_buttonIsGridWithFixedSize.Properties[FrameworkElement.WidthProperty].ValueOnInstance);
		}
		#endregion
	}
}
