// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSystemNamespaceWithMissingImportTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			MockClass systemConsoleClass = new MockClass(projectContent, "System.Console");
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			namespaceItems.Add(systemConsoleClass);
			projectContent.AddExistingNamespaceContents("System", namespaceItems);

			return new ExpressionResult("System", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return "System\r\n";
		}
		
		[Test]
		public void ResolveResultIsNullSinceSystemNamespaceIsNotImported()
		{
			Assert.IsNull(resolveResult);
		}
		
		[Test]
		public void ProjectContentNamespaceExistsReturnsTrueForSystemNamespace()
		{
			Assert.IsTrue(projectContent.NamespaceExists("System"));
		}
	}
}
