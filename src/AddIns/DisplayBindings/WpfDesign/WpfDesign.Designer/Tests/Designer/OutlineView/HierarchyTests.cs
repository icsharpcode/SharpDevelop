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
