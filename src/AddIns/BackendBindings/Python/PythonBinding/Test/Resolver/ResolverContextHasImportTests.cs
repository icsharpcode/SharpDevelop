// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
