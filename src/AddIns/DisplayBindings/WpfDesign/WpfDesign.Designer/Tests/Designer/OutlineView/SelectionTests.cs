// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WpfDesign.Designer.OutlineView;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer.OutlineView
{
	[TestFixture]
	public class SelectionTests : ModelTestHelper
	{
		private DesignItem _grid;
		private OutlineNode _outline;

		[SetUp]
		public void Intialize()
		{
			_grid = CreateGridContextWithDesignSurface("<Button/><StackPanel><Button/></StackPanel>");
			_outline = OutlineNode.Create(_grid);
			Assert.IsNotNull(_outline);

			var selection = _grid.Services.Selection;
			var stackPanel = _grid.ContentProperty.CollectionElements[1];
			var stackPanelButton = stackPanel.ContentProperty.CollectionElements[0];
			selection.SetSelectedComponents(new[] {stackPanel, stackPanelButton});
		}

		[Test]
		public void CheckIfInOutlineSelected()
		{
			Assert.IsFalse(_outline.IsSelected);
			Assert.IsFalse(_outline.Children[0].IsSelected);
			Assert.IsTrue(_outline.Children[1].IsSelected);
			Assert.IsTrue(_outline.Children[1].Children[0].IsSelected);
		}

		[Test]
		public void SelectNodeInOutlineAndCheckInDesignItem()
		{
			var selection = _grid.Services.Selection;
			selection.SetSelectedComponents(null);
			_outline.IsSelected = true;
			_outline.Children[0].IsSelected = true;

			Assert.AreEqual(_grid, selection.PrimarySelection);
			Assert.IsTrue(selection.SelectedItems.Contains(_grid.ContentProperty.CollectionElements[0]));
		}
	}
}
