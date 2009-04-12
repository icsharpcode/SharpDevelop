// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using IronPython.Compiler.Ast;
using Microsoft.Scripting;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the PythonFormWalker throws a PythonFormWalkerException if a unknown type is used in the
	/// form.
	/// </summary>
	[TestFixture]
	public class UnknownTypeTestFixture
	{		
		string pythonCode = "from System.Windows.Forms import Form\r\n" +
							"\r\n" +
							"class MainForm(System.Windows.Forms.Form):\r\n" +
							"    def __init__(self):\r\n" +
							"        self.InitializeComponent()\r\n" +
							"\r\n" +
							"    def InitializeComponent(self):\r\n" +
							"        self.ClientSize = Unknown.Type(10)\r\n";
		
		[Test]
		[ExpectedException(typeof(PythonFormWalkerException))]
		public void PythonFormWalkerExceptionThrown()
		{
			PythonFormWalker walker = new PythonFormWalker(new MockComponentCreator());
			walker.CreateForm(pythonCode);
			Assert.Fail("Exception should have been thrown before this.");
		}
	}
}
