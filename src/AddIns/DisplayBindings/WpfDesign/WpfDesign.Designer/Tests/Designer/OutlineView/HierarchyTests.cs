// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer.OutlineView
{
	[TestFixture]
	public class HierarchyTests : ModelTestHelper
	{
		private DesignItem _grid;
		private OutlineNode _outline;

		[TestFixtureSetUp]
		public void Intialize()
		{
			_grid = CreateGridContextWithDesignSurface("<Button/><StackPanel><Button/></StackPanel>");
			_outline = OutlineNode.Create(_grid);
			Assert.IsNotNull(_outline);
		}

		[Test]
		public void VerifyGridChildren()
		{
			Assert.AreEqual(2, _outline.Children.Count);
		}

		[Test]
		public void VerifyStackPanelChildren()
		{
			Assert.AreEqual(1, _outline.Children[1].Children.Count);
		}

		[Test]
		public void VerifyButtonInOutline()
		{
			var button = _grid.ContentProperty.CollectionElements[0];
			Assert.AreEqual(button, _outline.Children[0].DesignItem);
		}

		[Test]
		public void VerifyStackPanelInOutline()
		{
			var stackPanel = _grid.ContentProperty.CollectionElements[1];
			var stackPanelButton = stackPanel.ContentProperty.CollectionElements[0];

			Assert.AreEqual(stackPanel, _outline.Children[1].DesignItem);
			Assert.AreEqual(stackPanelButton, _outline.Children[1].Children[0].DesignItem);
		}
	}
}
