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
