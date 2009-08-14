// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the SecondaryDisplayBinding detects that the form
	/// is designable when the base class is fully qualified.
	/// </summary>
	[TestFixture]
	public class IsFullyQualifiedBaseClassFormDesignableTestFixture : IsFormDesignableTestFixture
	{						
		protected override string GetPythonCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" + 
					"class MainForm(System.Windows.Forms.Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n" +
					"\r\n";
		}
		
		[Test]
		public void MainFormClassBaseTypeIsForm()
		{
			IReturnType baseType = null;
			foreach (IReturnType returnType in mainFormClass.BaseTypes) {
				if (returnType.Name == "Form") {
					baseType = returnType;
					break;
				}
			}
			Assert.IsNotNull(baseType);
		}
		
		[Test]
		public void MainFormClassBaseTypeFullNameIsSystemWindowsFormsForm()
		{
			IReturnType baseType = null;
			foreach (IReturnType returnType in mainFormClass.BaseTypes) {
				if (returnType.FullyQualifiedName == "System.Windows.Forms.Form") {
					baseType = returnType;
					break;
				}
			}
			Assert.IsNotNull(baseType);
		}
	}
}
