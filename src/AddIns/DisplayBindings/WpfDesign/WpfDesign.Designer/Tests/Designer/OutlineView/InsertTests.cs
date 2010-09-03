// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer.OutlineView;

namespace ICSharpCode.WpfDesign.Tests.Designer.OutlineView
{
	[TestFixture]
	public class CollectionElementsInsertTests : ModelTestHelper
	{
		private DesignItem _grid;
		private OutlineNode _outline;

		private DesignItem _gridButton;
		private DesignItem _stackPanel;
		private DesignItem _stackPanelButton;

		private OutlineNode _gridButtonNode;
		private OutlineNode _stackPanelNode;
		private OutlineNode _stackPanelButtonNode;

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
		[Ignore]
		public void CheckGridChildrenCountWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(3, _outline.Children.Count);
			Assert.AreEqual(3, _grid.ContentProperty.CollectionElements.Count);
		}

		[Test]
		[Ignore]
		public void CheckStackPanelChildrenCountWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(1, _outline.Children[2].Children.Count);
			Assert.AreEqual(1, _stackPanel.ContentProperty.CollectionElements.Count);
		}

		[Test]
		[Ignore]
		public void CheckElementsInOutlineWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(_gridButtonNode, _outline.Children[0]);
			Assert.AreEqual(_stackPanelButtonNode, _outline.Children[1]);
			Assert.AreEqual(_stackPanelNode, _outline.Children[2]);
			Assert.AreEqual(_stackPanelButtonNode, _stackPanelButtonNode.Children[0]);
		}

		[Test]
		[Ignore]
		public void CheckElementsInDesignerWhenCopy()
		{
			InsertIntoGridByCopy();
			Assert.AreEqual(_gridButton, _grid.ContentProperty.CollectionElements[0]);
			Assert.AreEqual(_stackPanelButton, _grid.ContentProperty.CollectionElements[1]);
			Assert.AreEqual(_stackPanel, _grid.ContentProperty.CollectionElements[2]);
			Assert.AreEqual(_stackPanelButton, _stackPanel.ContentProperty.CollectionElements[0]);
		}
		#endregion
	}

	public class ContentControlInsertTests : ModelTestHelper
	{
		private DesignItem _grid;
		private OutlineNode _outline;

		private DesignItem _gridButton;
		private DesignItem _stackPanel;
		private DesignItem _stackPanelImage;

		private OutlineNode _gridButtonNode;
		private OutlineNode _stackPanelNode;
		private OutlineNode _stackPanelImageNode;

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
			/* Insert Image into the Grid's button. This has to be false since some of the
			 * ContentControl are not allowed to add element's by moving elements
			 * See DefaultPlacementBehavior.CanContentControlAdd() */

			Assert.IsFalse(_gridButtonNode.CanInsert(new[] {_stackPanelImageNode}, null, false));
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
		[Ignore]
		public void CheckStackPanelChildrenCountWhenCopy()
		{
			InsertIntoButtonByCopy();
			Assert.AreEqual(1, _stackPanelNode.Children.Count);
			Assert.AreEqual(1, _stackPanel.ContentProperty.CollectionElements.Count);
		}

		[Test]
		[Ignore]
		public void CheckElementInOutlineWhenCopy()
		{
			InsertIntoButtonByCopy();
			Assert.AreEqual(_gridButtonNode, _outline.Children[0]);
			Assert.AreEqual(_stackPanelImageNode, _outline.Children[0].Children[0]);
			Assert.AreEqual(_stackPanelNode, _outline.Children[1]);
			Assert.AreEqual(_stackPanelImageNode, _outline.Children[1].Children[0]);
		}

		[Test]
		[Ignore]
		public void CheckElementInDesignerWhenCopy()
		{
			InsertIntoButtonByCopy();
			Assert.AreEqual(_gridButton, _grid.ContentProperty.CollectionElements[0]);
			Assert.AreEqual(_stackPanelImage, _grid.ContentProperty.CollectionElements[0].ContentProperty.ValueOnInstance);
			Assert.AreEqual(_stackPanel, _grid.ContentProperty.CollectionElements[1]);
			Assert.AreEqual(_stackPanel, _grid.ContentProperty.CollectionElements[1].ContentProperty.CollectionElements[0]);
		}

		#endregion
	}
}
