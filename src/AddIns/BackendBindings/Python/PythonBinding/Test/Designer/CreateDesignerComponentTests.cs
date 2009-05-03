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
				Assert.IsInstanceOfType(typeof(PythonListViewComponent), PythonDesignerComponentFactory.CreateDesignerComponent(listView));
			}
		}
		
		[Test]
		public void TextBoxComponent()
		{
			using (TextBox textBox = new TextBox()) {
				Assert.IsInstanceOfType(typeof(PythonDesignerComponent), PythonDesignerComponentFactory.CreateDesignerComponent(textBox));
			}
		}
		
		[Test]
		public void CreateRootDesigner()
		{
			using (TextBox textBox = new TextBox()) {
				Assert.IsInstanceOfType(typeof(PythonDesignerRootComponent), PythonDesignerComponentFactory.CreateDesignerRootComponent(textBox));
			}
		}
	}
}
