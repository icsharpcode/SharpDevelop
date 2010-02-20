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
	[TestFixture]
	public class ResolveSystemWithImportSystemWindowsTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			return new ExpressionResult("System");
		}
		
		protected override string GetPythonScript()
		{
			return
				"import System.Windows\r\n" +
				"System\r\n";
		}
		
		NamespaceResolveResult NamespaceResolveResult {
			get { return resolveResult as NamespaceResolveResult; }
		}
		
		[Test]
		public void NamespaceResolveResultHasSystemNamespace()
		{
			Assert.AreEqual("System", NamespaceResolveResult.Name);
		}
	}
}
