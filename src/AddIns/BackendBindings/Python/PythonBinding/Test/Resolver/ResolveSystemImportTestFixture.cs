// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class ResolveSystemImportTestFixture : ResolveTestFixtureBase
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
		public void ExpressionResultContextShowItemReturnsTrueForString()
		{
			Assert.IsTrue(expressionResult.Context.ShowEntry("abc"));
		}
		
		[Test]
		public void ExpressionResultContextShowItemReturnsFalseForProjectContent()
		{
			MockProjectContent projectContent = new MockProjectContent();
			Assert.IsFalse(expressionResult.Context.ShowEntry(projectContent));
		}
	}
}
