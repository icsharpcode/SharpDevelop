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
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests that the PythonResolver resolves "import System" to 
	/// a namespace.
	/// </summary>
	[TestFixture]
	public class ResolveSystemImportTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			string code = GetPythonScript();
			PythonExpressionFinder finder = new PythonExpressionFinder();
			return finder.FindExpression(code, code.Length);
		}
		
		protected override string GetPythonScript()
		{
			return "import System";
		}
		
		[Test]
		public void NamespaceResolveResultFound()
		{
			Assert.IsNotNull(resolveResult);
		}
		
		[Test]
		public void NamespaceName()
		{
			PythonImportModuleResolveResult importResolveResult = (PythonImportModuleResolveResult)resolveResult;
			Assert.AreEqual("System", importResolveResult.Name);
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsTrueForNamespaceEntry()
		{
			NamespaceEntry entry = new NamespaceEntry("abc");
			Assert.IsTrue(expressionResult.Context.ShowEntry(entry));
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsFalseForNull()
		{
			Assert.IsFalse(expressionResult.Context.ShowEntry(null));
		}
	}
}
