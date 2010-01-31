// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the PythonResolver does not return a namespace resolve result for
	/// an unknown namespace.
	/// </summary>
	[TestFixture]
	public class ResolveUnknownNamespaceTestFixture : ResolveTestFixtureBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			projectContent.AddExistingNamespaceContents("System", new List<ICompletionEntry>());
			
			return new ExpressionResult("Unknown", new DomRegion(3, 2), null, null);
		}
		
		protected override string GetPythonScript()
		{
			return
				"import System\r\n" +
				"class Test:\r\n" +
				"    def __init__(self):\r\n" +
				"        Unknown\r\n";
		}
		
		[Test]
		public void ResolveResultDoesNotExist()
		{
			Assert.IsNull(resolveResult);
		}
		
		[Test]
		public void NamespaceExistsCalled()
		{
			Assert.IsTrue(projectContent.NamespaceExistsCalled);
		}	
	}
}
