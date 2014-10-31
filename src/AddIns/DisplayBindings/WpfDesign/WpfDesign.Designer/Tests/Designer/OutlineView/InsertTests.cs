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
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer.OutlineView;

namespace ICSharpCode.WpfDesign.Tests.Designer.OutlineView
{
	[TestFixture]
	public class CollectionElementsInsertTests : ModelTestHelper
	{
		private DesignItem _grid;
		private IOutlineNode _outline;

		private DesignItem _gridButton;
		private DesignItem _stackPanel;
		private DesignItem _stackPanelButton;

		private IOutlineNode _gridButtonNode;
		private IOutlineNode _stackPanelNode;
		private IOutlineNode _stackPanelButtonNode;

		[SetUp]
		public void Intialize()
		{
			_grid = CreateGridContextWithDesignSurface("<Button/><StackPanel><Button/></StackPanel>");
			_outline = OutlineNode.Create(_grid);
			Assert.IsNotNull(_outline);

			_gridButton = _grid.ContentProperty.CollectionElements[0];
			_stackPanel = _grid.ContentProperty.CollectionElements[1];
			_stackPanelButton = _stackPanel.ContentProperty.CollectionElements[0];

			_gridButtonNode = _outline.Children[0];
			_stackPanelNode = _outline.Children[1];
			_stackPanelButtonNode = _stackPanelNode.Children[0];
		}

		[Test]
		public void CanInsertIntoGrid()
		{
			// Check if StackPanel's button can be inserted after Grid's first button.
			Assert.IsTrue(_outline.CanInsert(new[] { _stackPanelButtonNode }, _gridButtonNode, false));
		}

		[Test]
		public void CanInsertIntoStackPanel()
		{
			// Move parent into one of it's children, Grid into StackPanel.
			Assert.IsTrue(_stackPanelNode.CanInsert(new[] { _outline }, _outline, false));
		}

		#region Insert by cut operation
		public void InsertIntoGridByCut()
		{
			// Insert StackPanel's button after Grid's first button.
			_outline.Insert(new[] { _outline.Children[1].Children[0] }, _outline.Children[0], false);
		}

		[Test]
		public void CheckGridChildrenCountWhenCut()
		{
			InsertIntoGridByCut();
			Assert.AreEqual(3, _outline.Children.Count);
			Assert.AreEqual(3, _grid.ContentProperty.CollectionElements.Count);
		}

		[Test]
		public void CheckStackPanelChildrenCountWhenCut()
		{
			InsertIntoGridByCut();
			Assert.AreEqual(0, _outline.Children[2].Children.Count);
			Assert.AreEqual(0, _stackPanel.ContentProperty.CollectionElements.Count);
		}

		[Test]
		public void CheckElementsInOutlineWhenCut()
		{
			InsertIntoGridByCut();
			Assert.AreEqual(_gridButtonNode, _outline.Children[0]);
			Assert.AreEqual(_stackPanelButtonNode, _outline.Children[1]);
			Assert.AreEqual(_stackPanelNode, _outline.Children[2]);
		}

		[Test]
		public void CheckElementsInDesignerWhenCut()
		{
			InsertIntoGridByCut();
			Assert.AreEqual(_gridButton, _grid.ContentProperty.CollectionElements[0]);
			Assert.AreEqual(_stackPanelButton, _grid.ContentProperty.CollectionElements[1]);
			Assert.AreEqual(_stackPanel, _grid.ContentProperty.CollectionElements[2]);
		}
		#endregion

		#region Insert by copy operation

		/* Cloning DesignItem is not yet supported */
		
		public void InsertIntoGridByCopy()
		{
			// Insert StackPanel's button after Grid's first button.
			_outline.Insert(new[] { _outline.Children[1].Children[0] }, _outline.Children[0], true);
		}

		[Test]
		//[Ignore]
		public void CheckGridChildrenCountWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(3, _outline.Children.Count);
			Assert.AreEqual(3, _grid.ContentProperty.CollectionElements.Count);
		}

		[Test]
		//[Ignore]
		public void CheckStackPanelChildrenCountWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(1, _outline.Children[2].Children.Count);
			Assert.AreEqual(1, _stackPanel.ContentProperty.CollectionElements.Count);
		}

		[Test]
		//[Ignore]
		public void CheckElementsInOutlineWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(_gridButtonNode.DesignItem.Component.GetType(), _outline.Children[0].DesignItem.Component.GetType());
			Assert.AreEqual(_stackPanelButtonNode.DesignItem.Component.GetType(), _outline.Children[1].DesignItem.Component.GetType());
			Assert.AreEqual(_stackPanelNode.DesignItem.Component.GetType(), _outline.Children[2].DesignItem.Component.GetType());			
		}

		[Test]
		//[Ignore]
		public void CheckElementsInDesignerWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(_gridButton, _grid.ContentProperty.CollectionElements[0]);
			Assert.AreEqual(_stackPanelButton.Component.GetType(), _grid.ContentProperty.CollectionElements[1].Component.GetType());
			Assert.AreEqual(_stackPanel.Component.GetType(), _grid.ContentProperty.CollectionElements[2].Component.GetType());
			Assert.AreEqual(_stackPanelButton.Component.GetType(), _stackPanel.ContentProperty.CollectionElements[0].Component.GetType());
		}
		#endregion
	}

	public class ContentControlInsertTests : ModelTestHelper
	{
		private DesignItem _grid;
		private IOutlineNode _outline;

		private DesignItem _gridButton;
		private DesignItem _stackPanel;
		private DesignItem _stackPanelImage;

		private IOutlineNode _gridButtonNode;
		private IOutlineNode _stackPanelNode;
		private IOutlineNode _stackPanelImageNode;

		[SetUp]
		public void Intialize()
		{
			_grid = CreateGridContextWithDesignSurface("<Button/><StackPanel><Image/></StackPanel>");
			_outline = OutlineNode.Create(_grid);
			Assert.IsNotNull(_outline);

			_gridButton = _grid.ContentProperty.CollectionElements[0];
			_stackPanel = _grid.ContentProperty.CollectionElements[1];
			_stackPanelImage = _stackPanel.ContentProperty.CollectionElements[0];

			_gridButtonNode = _outline.Children[0];
			_stackPanelNode = _outline.Children[1];
			_stackPanelImageNode = _stackPanelNode.Children[0];
		}

		[Test]
		public void CanInsertIntoButton()
		{
			/* Insert Image into the Grid's button. This has now to be true because a button can now
			 * add element's by moving elements
			 * See DefaultPlacementBehavior.CanContentControlAdd() */

			Assert.IsTrue(_gridButtonNode.CanInsert(new[] {_stackPanelImageNode}, null, false));
		}

		#region Insert element by Cut operation.
		public void InsertIntoButtonByCut()
		{
			_gridButtonNode.Insert(new[] {_stackPanelImageNode}, null, false);
		}

		[Test]
		public void CheckStackPanelChildrenCountWhenCut()
		{
			InsertIntoButtonByCut();
			Assert.AreEqual(0, _stackPanelNode.Children.Count);
			Assert.AreEqual(0, _stackPanel.ContentProperty.CollectionElements.Count);
		}

		[Test]
		public void CheckElementInOutlineWhenCut()
		{
			InsertIntoButtonByCut();
			Assert.AreEqual(_gridButtonNode, _outline.Children[0]);
			Assert.AreEqual(_stackPanelImageNode, _outline.Children[0].Children[0]);
			Assert.AreEqual(_stackPanelNode, _outline.Children[1]);
		}

		[Test]
		public void CheckElementInDesignerWhenCut()
		{
			InsertIntoButtonByCut();
			Assert.AreEqual(_gridButton, _grid.ContentProperty.CollectionElements[0]);
			Assert.AreEqual(_stackPanelImage, _grid.ContentProperty.CollectionElements[0].ContentProperty.Value);
			Assert.AreEqual(_stackPanel, _grid.ContentProperty.CollectionElements[1]);
		}
		#endregion

		#region Insert element by Copy operation

		/* Cloning DesignItem is not yet supported */

		public void InsertIntoButtonByCopy()
		{
			_gridButtonNode.Insert(new[] { _stackPanelImageNode }, null, true);
		}

		[Test]
		public void CheckStackPanelChildrenCountWhenCopy()
		{
			InsertIntoButtonByCopy();
			Assert.AreEqual(1, _stackPanelNode.Children.Count);
			Assert.AreEqual(1, _stackPanel.ContentProperty.CollectionElements.Count);
		}

		[Test]
		public void CheckElementInOutlineWhenCopy()
		{
			InsertIntoButtonByCopy();
			Assert.AreEqual(_gridButtonNode, _outline.Children[0]);
            Assert.AreEqual(_stackPanelImageNode.DesignItem.ComponentType, _outline.Children[0].Children[0].DesignItem.ComponentType);
			Assert.AreEqual(_stackPanelNode, _outline.Children[1]);
			Assert.AreEqual(_stackPanelImageNode, _outline.Children[1].Children[0]);
		}

		[Test]
		public void CheckElementInDesignerWhenCopy()
		{
			InsertIntoButtonByCopy();
			Assert.AreEqual(_gridButton, _grid.ContentProperty.CollectionElements[0]);
            Assert.AreEqual(_stackPanelImage.ComponentType, _grid.ContentProperty.CollectionElements[0].ContentProperty.Value.ComponentType);
            Assert.AreEqual(_stackPanel.ComponentType, _grid.ContentProperty.CollectionElements[1].ComponentType);
            Assert.AreEqual(_stackPanelImage, _grid.ContentProperty.CollectionElements[1].ContentProperty.CollectionElements[0]);
		}

		#endregion
	}
}
