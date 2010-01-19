// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			string[] usings = new string[] { "abc" };
			ParseInformation parseInfo = PythonResolverHelper.CreateParseInfoWithUsings(usings);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsFalse(resolverContext.HasImport("Unknown"));
		}
		
		[Test]
		public void HasImportReturnsTrueForImportThatDoesExistInCompilationUnit()
		{
			string[] usings = new string[] { "abc" };
			ParseInformation parseInfo = PythonResolverHelper.CreateParseInfoWithUsings(usings);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsTrue(resolverContext.HasImport("abc"));
		}
		
		[Test]
		public void HasImportReturnsTrueForImportThatDoesExistInCompilationUnitWithSingleUsingWithMultipleNamespaces()
		{
			string[] usings = new string[] { "abc", "def" };
			ParseInformation parseInfo = PythonResolverHelper.CreateParseInfoWithUsings(usings);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsTrue(resolverContext.HasImport("def"));
		}
	}
}
