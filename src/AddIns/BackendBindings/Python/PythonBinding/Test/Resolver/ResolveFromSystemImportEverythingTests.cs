// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	public class ResolveFromSystemImportEverythingTests : ResolveTestsBase
	{
		MockClass consoleClass;
		
		protected override ExpressionResult GetExpressionResult()
		{
			consoleClass = new MockClass(projectContent, "System.Console");
			projectContent.SetClassToReturnFromGetClass("System.Console", consoleClass);
			
			List<ICompletionEntry> namespaceItems = new List<ICompletionEntry>();
			projectContent.AddExistingNamespaceContents("System", namespaceItems);
			
			return new ExpressionResult("Console", ExpressionContext.Default);
		}
		
		protected override string GetPythonScript()
		{
			return 
				"from System import *\r\n" +
				"Console\r\n" +
				"\r\n";
		}
		
		[Test]
		public void ResolveResultResolvedClassIsConsoleClass()
		{
			Assert.AreEqual(consoleClass, TypeResolveResult.ResolvedClass);
		}
		
		TypeResolveResult TypeResolveResult {
			get { return (TypeResolveResult)resolveResult; }
		}
		
		[Test]
		public void ProjectContentNamespaceExistsReturnsTrueForSystem()
		{
			Assert.IsTrue(projectContent.NamespaceExists("System"));
		}
	}
}
