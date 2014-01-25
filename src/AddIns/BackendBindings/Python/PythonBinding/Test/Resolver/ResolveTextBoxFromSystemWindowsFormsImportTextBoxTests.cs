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
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveTextBoxFromSystemWindowsFormsImportTextBoxTests : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			MockClass textBoxClass = new MockClass(projectContent, "System.Windows.Forms.TextBox");
			projectContent.SetClassToReturnFromGetClass("System.Windows.Forms.TextBox", textBoxClass);
			
			return new ExpressionResult("TextBox", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from System.Windows.Forms import TextBox\r\n" +
				"TextBox\r\n" +
				"\r\n";
		}
			
		[Test]
		public void ResolveResultIsTypeResolveResult()
		{
			Assert.IsTrue(resolveResult is TypeResolveResult);
		}
		
		[Test]
		public void ResolveResultResolveClassNameIsTextBox()
		{
			Assert.AreEqual("TextBox", TypeResolveResult.ResolvedClass.Name);
		}
		
		TypeResolveResult TypeResolveResult {
			get { return (TypeResolveResult)resolveResult; }
		}
	}
}
