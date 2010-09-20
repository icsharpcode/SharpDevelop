// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CodeCoverage;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ICSharpCode.CodeCoverage.Tests.Coverage
{
	[TestFixture]
	public class CodeCoverageNamespaceTestFixture
	{
		CodeCoverageModule module;
		CodeCoverageMethod attributeNameTestFixtureMethod;
		CodeCoverageMethod abstractElementSchemaTestFixtureMethod;
		CodeCoverageMethod differentRootNamespaceTestFixtureMethod;
		
		string sharpDevelopNamespace;
		string differentRootNamespace;
		string xmlEditorNamespace;
		
		[SetUp]
		public void Init()
		{
			module = new CodeCoverageModule("XmlEditor.Tests");
			
			attributeNameTestFixtureMethod = new CodeCoverageMethod("SuccessTest1", "ICSharpCode.XmlEditor.Parser.AttributeNameTestFixture");
			module.Methods.Add(attributeNameTestFixtureMethod);
			
			abstractElementSchemaTestFixtureMethod = new CodeCoverageMethod("FileElementHasAttributeNamedType", "ICSharpCode.XmlEditor.Schema.AbstractElementSchemaTestFixture");
			module.Methods.Add(abstractElementSchemaTestFixtureMethod);
			
			differentRootNamespaceTestFixtureMethod = new CodeCoverageMethod("DifferentRootNamespaceTestMethod", "RootNamespace.XmlEditor.DifferentRootNamespaceTestFixture");
			module.Methods.Add(differentRootNamespaceTestFixtureMethod);
			
			sharpDevelopNamespace = module.RootNamespaces[0];
			differentRootNamespace = module.RootNamespaces[1];
			xmlEditorNamespace = (CodeCoverageMethod.GetChildNamespaces(module.Methods, sharpDevelopNamespace))[0];
		}
		
		[Test]
		public void SharpDevelopRootNamespace()
		{
			Assert.AreEqual("ICSharpCode", sharpDevelopNamespace);
		}
		
		[Test]
		public void DifferentRootNamespace()
		{
			Assert.AreEqual("RootNamespace", differentRootNamespace);
		}
		
		[Test]
		public void GetAllICSharpCodeNamespaceMethods()
		{
			List<CodeCoverageMethod> methods = CodeCoverageMethod.GetAllMethods(module.Methods, "ICSharpCode");
			Assert.AreSame(attributeNameTestFixtureMethod, methods[0]);
		}
		
		[Test]
		public void GetAllICSharpCodeXmlEditorNamespaceMethods()
		{
			List<CodeCoverageMethod> methods = CodeCoverageMethod.GetAllMethods(module.Methods, "ICSharpCode.XmlEditor");
			Assert.AreSame(attributeNameTestFixtureMethod, methods[0]);
		}
		
		[Test]
		public void XmlEditorNamespace()
		{
			Assert.AreEqual("XmlEditor", xmlEditorNamespace);
		}
		
		[Test]
		public void OnlyOneSharpDevelopChildNamespace()
		{
			Assert.AreEqual(1, CodeCoverageMethod.GetChildNamespaces(module.Methods, sharpDevelopNamespace).Count);
		}
		
		[Test]
		public void NoNamespacesBelowXmlEditorSchemaNamespace()
		{
			Assert.AreEqual(0, CodeCoverageMethod.GetChildNamespaces(module.Methods, "ICSharpCode.XmlEditor.Schema").Count);
		}
		
		[Test]
		public void SchemaNamespaceMethods()
		{
			List<CodeCoverageMethod> methods = CodeCoverageMethod.GetMethods(module.Methods, "ICSharpCode.XmlEditor.Schema", "AbstractElementSchemaTestFixture");
			Assert.AreSame(abstractElementSchemaTestFixtureMethod, methods[0]);
		}
		
		[Test]
		public void SchemaNamespaceClassNames()
		{
			List<string> names = CodeCoverageMethod.GetClassNames(module.Methods, "ICSharpCode.XmlEditor.Schema");
			Assert.AreEqual("AbstractElementSchemaTestFixture", names[0]);
		}
	}
}
