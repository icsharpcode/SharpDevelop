// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
