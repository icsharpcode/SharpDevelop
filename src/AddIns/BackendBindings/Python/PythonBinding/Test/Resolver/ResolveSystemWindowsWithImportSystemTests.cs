// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveSystemWindowsWithImportSystemTestFixture : ResolveTestsBase
	{
		protected override ExpressionResult GetExpressionResult()
		{
			MockProjectContent referencedContent = new MockProjectContent();
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			referencedContent.AddExistingNamespaceContents("System.Windows.Forms", namespaceItems);
			projectContent.ReferencedContents.Add(referencedContent);
			
			return new ExpressionResult("System.Windows");
		}
		
		protected override string GetPythonScript()
		{
			return
				"import System\r\n" +
				"System.Windows\r\n";
		}
		
		NamespaceResolveResult NamespaceResolveResult {
			get { return resolveResult as NamespaceResolveResult; }
		}
		
		[Test]
		public void NamespaceResolveResultHasSystemWindowsNamespace()
		{
			Assert.AreEqual("System.Windows", NamespaceResolveResult.Name);
		}
	}
}
