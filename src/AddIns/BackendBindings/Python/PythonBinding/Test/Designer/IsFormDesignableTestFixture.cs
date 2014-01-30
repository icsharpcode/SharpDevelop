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
using ICSharpCode.FormsDesigner;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the SecondaryDisplayBinding detects that the form
	/// is designable:
	/// 
	/// 1) Has an InitializeComponents method.
	/// 2) Has a base class of Form.
	/// </summary>
	[TestFixture]
	public class IsFormDesignableTestFixture
	{
		IMethod initializeComponentsMethod;
		protected IClass mainFormClass;
		ParseInformation parseInfo;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonParser parser = new PythonParser();
			MockProjectContent mockProjectContent = new MockProjectContent();
			ICompilationUnit compilationUnit = parser.Parse(mockProjectContent, @"C:\Projects\Test\MainForm.py", GetPythonCode());

			parseInfo = new ParseInformation(compilationUnit);
			
			if (compilationUnit.Classes.Count > 0) {
				mainFormClass = compilationUnit.Classes[0];
				initializeComponentsMethod = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(mainFormClass);
			}
		}
		
		[Test]
		public void InitializeComponentsIsNotNull()
		{
			Assert.IsNotNull(initializeComponentsMethod);
		}
		
		[Test]
		public void BaseClassIsForm()
		{
			Assert.IsTrue(FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(mainFormClass));
		}
		
		[Test]
		public void IsDesignable()
		{
			Assert.IsTrue(FormsDesignerSecondaryDisplayBinding.IsDesignable(parseInfo));
		}
				
		protected virtual string GetPythonCode()
		{
			return "from System.Windows.Forms import Form\r\n" +
					"\r\n" + 
					"class MainForm(Form):\r\n" +
					"\tdef __init__(self):\r\n" +
					"\t\tself.InitializeComponent()\r\n" +
					"\r\n" +
					"\tdef InitializeComponent(self):\r\n" +
					"\t\tpass\r\n" +
					"\r\n";
		}
	}
}
