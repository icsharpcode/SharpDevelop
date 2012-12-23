// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeImportTests
	{
		CodeImport codeImport;
		UsingHelper helper;
		
		void CreateCodeImport(string namespaceName)
		{
			helper = new UsingHelper();
			helper.AddNamespace(namespaceName);
			codeImport = new CodeImport(helper.Using);
		}
		
		void CreateCodeImportWithNoNamespace()
		{
			helper = new UsingHelper();
			codeImport = new CodeImport(helper.Using);
		}
		
		[Test]
		public void Kind_SystemXmlNamespace_ReturnsImport()
		{
			CreateCodeImport("System.Xml");
			
			global::EnvDTE.vsCMElement kind = codeImport.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementImportStmt, kind);
		}
		
		[Test]
		public void Namespace_UsingHasNoNamespacesAndAliasesIsNull_ReturnsEmptyString()
		{
			CreateCodeImportWithNoNamespace();
			
			string name = codeImport.Namespace;
			
			Assert.AreEqual(String.Empty, name);
		}
	}
}
