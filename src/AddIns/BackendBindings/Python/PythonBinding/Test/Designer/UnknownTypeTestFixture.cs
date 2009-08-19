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
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
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
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			ResourceManager rm = new ResourceManager("PythonBinding.Tests.Strings", GetType().Assembly);
			ResourceService.RegisterNeutralStrings(rm);			
		}
		
		[Test]
		public void SelfAssignmentWithUnknownTypeRhs()
		{
			string pythonCode = "from System.Windows.Forms import Form\r\n" +
								"\r\n" +
								"class MainForm(System.Windows.Forms.Form):\r\n" +
								"    def __init__(self):\r\n" +
								"        self.InitializeComponent()\r\n" +
								"\r\n" +
								"    def InitializeComponent(self):\r\n" +
								"        self.ClientSize = Unknown.Type(10)\r\n";
			
			try {
				PythonComponentWalker walker = new PythonComponentWalker(new MockComponentCreator());
				walker.CreateComponent(pythonCode);
				Assert.Fail("Exception should have been thrown before this.");
			} catch (PythonComponentWalkerException ex) {
				string expectedMessage = String.Format(StringParser.Parse("${res:ICSharpCode.PythonBinding.UnknownTypeName}"), "Unknown.Type");
				Assert.AreEqual(expectedMessage, ex.Message);
			}
		}
		
		[Test]
		public void LocalVariableAssignmentWithUnknownTypeRhs()
		{
			string pythonCode = "from System.Windows.Forms import Form\r\n" +
								"\r\n" +
								"class MainForm(System.Windows.Forms.Form):\r\n" +
								"    def __init__(self):\r\n" +
								"        self.InitializeComponent()\r\n" +
								"\r\n" +
								"    def InitializeComponent(self):\r\n" +
								"        abc = Unknown.Type(10)\r\n";

			try {
				PythonComponentWalker walker = new PythonComponentWalker(new MockComponentCreator());
				walker.CreateComponent(pythonCode);
				Assert.Fail("Exception should have been thrown before this.");
			} catch (PythonComponentWalkerException ex) {
				string expectedMessage = String.Format(StringParser.Parse("${res:ICSharpCode.PythonBinding.UnknownTypeName}"), "Unknown.Type");
				Assert.AreEqual(expectedMessage, ex.Message);
			}
		}
	}
}
