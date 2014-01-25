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
