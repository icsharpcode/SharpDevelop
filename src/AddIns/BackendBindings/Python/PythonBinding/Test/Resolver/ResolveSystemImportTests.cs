// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
