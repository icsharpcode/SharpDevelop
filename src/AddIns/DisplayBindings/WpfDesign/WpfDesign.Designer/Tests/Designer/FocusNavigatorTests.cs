// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class FocusNavigationTests : ModelTestHelper
	{
		[Test]
		public void SingleChildHierarchyFocusForward()
		{
			var grid = CreateGridContextWithDesignSurface("<Button><Button/></Button>");
			var firstButton = grid.ContentProperty.CollectionElements[0];
			var secondButton = firstButton.ContentProperty.Value;

			var selection = grid.Services.Selection;
			selection.SetSelectedComponents(new[] {grid});
			var focusNavigator = new MockFocusNavigator(grid.Context);

			/* Move focus forward Tab */
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(firstButton, selection.PrimarySelection);
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(secondButton, selection.PrimarySelection);
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(grid, selection.PrimarySelection);
		}

		[Test]
		public void SingleChildHierarchyFocusBack()
		{
			var grid = CreateGridContextWithDesignSurface("<Button><Button/></Button>");
			var firstButton = grid.ContentProperty.CollectionElements[0];
			var secondButton = firstButton.ContentProperty.Value;

			var selection = grid.Services.Selection;
			selection.SetSelectedComponents(new[] {grid});
			var focusNavigator = new MockFocusNavigator(grid.Context);

			/* Move focus back Shift + Tab */
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(secondButton, selection.PrimarySelection);
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(firstButton, selection.PrimarySelection);

		}

		[Test]
		public void MultipleChildHierarchyFocusForward()
		{
			var grid = CreateGridContextWithDesignSurface("<Button/><Grid><Button/></Grid>");
			var firstButton = grid.ContentProperty.CollectionElements[0];
			var innerGrid = grid.ContentProperty.CollectionElements[1];
			var innerGridButton = innerGrid.ContentProperty.CollectionElements[0];

			var selection = grid.Services.Selection;
			selection.SetSelectedComponents(new[] { grid });
			var focusNavigator = new MockFocusNavigator(grid.Context);

			/* Move focus forward Tab */
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(firstButton, selection.PrimarySelection);
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(innerGrid, selection.PrimarySelection);
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(innerGridButton, selection.PrimarySelection);
			focusNavigator.MoveFocusForward();
			Assert.AreEqual(grid, selection.PrimarySelection);
		}

		[Test]
		public void MultipleChildHierarchyFocusBack()
		{
			var grid = CreateGridContextWithDesignSurface("<Button/><Grid><Button/></Grid>");
			var firstButton = grid.ContentProperty.CollectionElements[0];
			var innerGrid = grid.ContentProperty.CollectionElements[1];
			var innerGridButton = innerGrid.ContentProperty.CollectionElements[0];

			var selection = grid.Services.Selection;
			selection.SetSelectedComponents(new[] { grid });
			var focusNavigator = new MockFocusNavigator(grid.Context);

			/* Move focus back Shift + Tab */
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(innerGridButton, selection.PrimarySelection);
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(innerGrid, selection.PrimarySelection);
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(firstButton, selection.PrimarySelection);
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(grid, selection.PrimarySelection);
			focusNavigator.MoveFocusBack();
			Assert.AreEqual(innerGridButton, selection.PrimarySelection);
		}
	}
}
