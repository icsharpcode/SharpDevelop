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
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the SecondaryDisplayBinding detects that the form
	/// is designable when the base class is fully qualified.
	/// </summary>
	[TestFixture]
	public class IsFullyQualifiedBaseClassFormDesignableTestFixture : IsFormDesignableTestFixture
	{						
		protected override string GetRubyCode()
		{
			return "require \"System.Windows.Forms\"\r\n" +
					"\r\n" + 
					"class MainForm < System::Windows::Forms::Form\r\n" +
					"    def initialize()\r\n" +
					"        self.InitializeComponent()\r\n" +
					"    end\r\n" +
					"\r\n" +
					"    def InitializeComponent()\r\n" +
					"    end\r\n" +
					"end\r\n" +
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
