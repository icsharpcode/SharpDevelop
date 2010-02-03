// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveFromSystemImportEverythingFixture : ResolveTestFixtureBase
	{
		MockClass consoleClass;
		
		protected override ExpressionResult GetExpressionResult()
		{
			consoleClass = new MockClass(projectContent, "System.Console");
			projectContent.ClassToReturnFromGetClass = consoleClass;
			projectContent.ClassNameForGetClass = "System.Console";
			
			projectContent.AddExistingNamespaceContents("System", new ArrayList());
			
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
