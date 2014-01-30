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

//using System;
//using ICSharpCode.PackageManagement.EnvDTE;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeImportTests
//	{
//		CodeImport codeImport;
//		UsingHelper helper;
//		
//		void CreateCodeImport(string namespaceName)
//		{
//			helper = new UsingHelper();
//			helper.AddNamespace(namespaceName);
//			codeImport = new CodeImport(helper.Using);
//		}
//		
//		void CreateCodeImportWithNoNamespace()
//		{
//			helper = new UsingHelper();
//			codeImport = new CodeImport(helper.Using);
//		}
//		
//		[Test]
//		public void Kind_SystemXmlNamespace_ReturnsImport()
//		{
//			CreateCodeImport("System.Xml");
//			
//			global::EnvDTE.vsCMElement kind = codeImport.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementImportStmt, kind);
//		}
//		
//		[Test]
//		public void Namespace_UsingHasNoNamespacesAndAliasesIsNull_ReturnsEmptyString()
//		{
//			CreateCodeImportWithNoNamespace();
//			
//			string name = codeImport.Namespace;
//			
//			Assert.AreEqual(String.Empty, name);
//		}
//	}
//}
