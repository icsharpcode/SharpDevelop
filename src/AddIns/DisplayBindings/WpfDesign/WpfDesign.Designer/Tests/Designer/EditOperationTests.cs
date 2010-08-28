// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
using System.Windows;
using NUnit.Framework;
using ICSharpCode.WpfDesign.Designer.Xaml;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Tests.Designer
{
	[TestFixture]
	public class EditOperationTests : ModelTestHelper
	{
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
