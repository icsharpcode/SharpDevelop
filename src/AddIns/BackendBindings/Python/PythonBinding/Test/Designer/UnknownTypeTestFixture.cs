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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
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
