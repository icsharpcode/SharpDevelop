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

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolverContextHasImportTests
	{
		[Test]
		public void HasImportReturnsFalseForImportThatDoesNotExistInCompilationUnit()
		{
			string code = "import abc";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsFalse(resolverContext.HasImport("Unknown"));
		}
		
		[Test]
		public void HasImportReturnsTrueForImportThatDoesExistInCompilationUnit()
		{
			string code = "import abc";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsTrue(resolverContext.HasImport("abc"));
		}
		
		[Test]
		public void HasImportReturnsTrueForImportThatDoesExistInCompilationUnitWithSingleUsingWithMultipleNamespaces()
		{
			string code = "import abc, ghi";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsTrue(resolverContext.HasImport("ghi"));
		}
		
		[Test]
		public void HasImportReturnsFalseForFromImport()
		{
			string code = "from import sys";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsFalse(resolverContext.HasImport("sys"));
		}
		
		[Test]
		public void HasImportReturnsTrueForImportedAsName()
		{
			string code = "import sys as something";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsTrue(resolverContext.HasImport("something"));
		}
	}
}
