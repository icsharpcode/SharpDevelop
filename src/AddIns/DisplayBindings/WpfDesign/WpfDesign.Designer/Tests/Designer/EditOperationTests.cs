// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Windows;

using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.XamlDom;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class EditOperationTests : ModelTestHelper
	{
		Mutex mutex;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			// Avoid test failure on build server when unit tests for several branches are run concurrently.
			bool createdNew;
			mutex = new Mutex(true, "ClipboardUnitTest", out createdNew);
			if (!createdNew) {
				if (!mutex.WaitOne(10000)) {
					throw new Exception("Could not acquire mutex");
				}
			}
		}
		
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			mutex.ReleaseMutex();
			mutex.Dispose();
		}
		
		[Test]
		public void Cut()
		{
			var grid = CreateGridContextWithDesignSurface("<Button/><Button/>");
			var xamlContext = grid.Context as XamlDesignContext;
			if (xamlContext != null) {
				xamlContext.XamlEditAction.Cut(grid.ContentProperty.CollectionElements);
				var cutXaml = Clipboard.GetText(TextDataFormat.Xaml);
				Assert.AreEqual("<Buttonxmlns=\"" + XamlConstants.PresentationNamespace + "\"/>" + xamlContext.XamlEditAction.Delimeter + "<Buttonxmlns=\"" + XamlConstants.PresentationNamespace + "\"/>" + xamlContext.XamlEditAction.Delimeter, cutXaml.Replace(" ", ""));
				Assert.AreEqual(0, grid.ContentProperty.CollectionElements.Count);
			} else {
				Assert.Fail();
			}
		}

		[Test]
		public void Copy()
		{
			var grid = CreateGridContextWithDesignSurface("<Button/><Button/>");
			var xamlContext = grid.Context as XamlDesignContext;
			if (xamlContext != null) {
				xamlContext.XamlEditAction.Copy(grid.ContentProperty.CollectionElements);
				var cutXaml = Clipboard.GetText(TextDataFormat.Xaml);
				Assert.AreEqual("<Buttonxmlns=\"" + XamlConstants.PresentationNamespace + "\"/>" + xamlContext.XamlEditAction.Delimeter + "<Buttonxmlns=\"" + XamlConstants.PresentationNamespace + "\"/>" + xamlContext.XamlEditAction.Delimeter, cutXaml.Replace(" ", ""));
				Assert.AreEqual(2, grid.ContentProperty.CollectionElements.Count);
			} else {
				Assert.Fail();
			}
		}

		private string _name;

		private DesignItem IntializePasteOperationsTest()
		{
			var grid = CreateGridContextWithDesignSurface("<Button Name=\"TestElement\"/><Grid><Button/></Grid><Window/><ListBox/>");
			Assert.IsNotNull(grid);
			var xamlConxtext = grid.Context as XamlDesignContext;
			if (xamlConxtext != null) {
				_name = grid.ContentProperty.CollectionElements[0].Name;
				xamlConxtext.XamlEditAction.Cut(new[] {grid.ContentProperty.CollectionElements[0]});
			} else
				Assert.Fail();
			return grid;
		}

		[Test]
		public void PasteWhenContentControlSelectedAndCannotAdd()
		{
			var grid = IntializePasteOperationsTest();
			var xamlContext = grid.Context as XamlDesignContext;
			Assert.IsNotNull(xamlContext);

			var selection = grid.Services.Selection;
			var innerGrid = grid.ContentProperty.CollectionElements[0];
			selection.SetSelectedComponents(innerGrid.ContentProperty.CollectionElements);
			xamlContext.XamlEditAction.Paste();
			Assert.AreEqual(_name, innerGrid.ContentProperty.CollectionElements[1].Name);
			Assert.AreEqual(innerGrid.ContentProperty.CollectionElements[1], selection.PrimarySelection);
		}

		[Test]
		public void PasteWhenContentControlSelectedAndCanAdd()
		{
			var grid = IntializePasteOperationsTest();
			var xamlContext = grid.Context as XamlDesignContext;
			Assert.IsNotNull(xamlContext);

			var selection = grid.Services.Selection;
			var window = grid.ContentProperty.CollectionElements[1];
			selection.SetSelectedComponents(new[] {window});
			xamlContext.XamlEditAction.Paste();
			Assert.AreEqual(_name, window.ContentProperty.Value.Name);
			Assert.AreEqual(window.ContentProperty.Value, selection.PrimarySelection);
		}

		[Test]
		public void PasteWhenIAddChildSelectedAndCanAdd()
		{
			var grid = IntializePasteOperationsTest();
			var xamlContext = grid.Context as XamlDesignContext;
			Assert.IsNotNull(xamlContext);

			var selection = grid.Services.Selection;
			selection.SetSelectedComponents(new[] {grid});
			xamlContext.XamlEditAction.Paste();
			Assert.AreEqual(_name, grid.ContentProperty.CollectionElements[3].Name);
			Assert.AreEqual(grid.ContentProperty.CollectionElements[3], selection.PrimarySelection);
		}

		[Test]
		public void PasteWhenIAddChildSelectedAndCannotAdd()
		{
			var grid = IntializePasteOperationsTest();
			var xamlContext = grid.Context as XamlDesignContext;
			Assert.IsNotNull(xamlContext);

			var selection = grid.Services.Selection;
			selection.SetSelectedComponents(new[] {grid.ContentProperty.CollectionElements[2]});
			xamlContext.XamlEditAction.Paste();
			Assert.AreEqual(_name, grid.ContentProperty.CollectionElements[3].Name);
			Assert.AreEqual(grid.ContentProperty.CollectionElements[3], selection.PrimarySelection);
		}
	}
}
