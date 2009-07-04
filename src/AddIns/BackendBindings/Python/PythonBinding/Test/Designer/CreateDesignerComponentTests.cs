// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the PythonControl's CreateDesignerComponent method.
	/// </summary>
	[TestFixture]
	public class CreateDesignerComponentTests
	{
		[Test]
		public void ListViewComponent()
		{
			using (ListView listView = new ListView()) {
				Assert.IsInstanceOf(typeof(PythonListViewComponent), PythonDesignerComponentFactory.CreateDesignerComponent(listView));
			}
		}
		
		[Test]
		public void TextBoxComponent()
		{
			using (TextBox textBox = new TextBox()) {
				Assert.IsInstanceOf(typeof(PythonDesignerComponent), PythonDesignerComponentFactory.CreateDesignerComponent(textBox));
			}
		}
		
		[Test]
		public void CreateRootDesigner()
		{
			using (TextBox textBox = new TextBox()) {
				Assert.IsInstanceOf(typeof(PythonDesignerRootComponent), PythonDesignerComponentFactory.CreateDesignerRootComponent(textBox));
			}
		}
		
		[Test]
		public void ImageListComponent()
		{
			using (ImageList imageList = new ImageList()) {
				Assert.IsInstanceOf(typeof(PythonImageListComponent), PythonDesignerComponentFactory.CreateDesignerComponent(imageList));
			}
		}
		
		[Test]
		public void TreeViewComponent()
		{
			using (TreeView treeView = new TreeView()) {
				Assert.IsInstanceOf(typeof(PythonTreeViewComponent), PythonDesignerComponentFactory.CreateDesignerComponent(treeView));
			}
		}		
	}
}
