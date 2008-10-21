// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3337 $</version>
// </file>

using System;
using System.Windows;
using NUnit.Framework;
using ICSharpCode.WpfDesign;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.WpfDesign.Designer.Extensions;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class PlacementTests : ModelTestHelper
	{
		void Move(Vector v, params DesignItem[] items)
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
		
		[Test]
		[Ignore] //Currently bounds calculated using visuals
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
	}
}
