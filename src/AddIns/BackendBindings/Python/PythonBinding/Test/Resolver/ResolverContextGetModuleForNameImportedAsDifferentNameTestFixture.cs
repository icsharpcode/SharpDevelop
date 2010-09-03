// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolverContextGetModuleForNameImportedAsDifferentNameTestFixture
	{
		PythonResolverContext resolverContext;
		ParseInformation parseInfo;
		
		[SetUp]
		public void Init()
		{
			string python =
				"from sys import exit as myexit";
			
			parseInfo = PythonParserHelper.CreateParseInfo(python);
			resolverContext = new PythonResolverContext(parseInfo);
		}
		
		[Test]
		public void ResolverContextGetModuleForNameReturnsSysForMyExitName()
		{
			Assert.AreEqual("sys", resolverContext.GetModuleForImportedName("myexit"));
		}
		
		[Test]
		public void ResolverContextGetModuleForImportedNameReturnsNullForExitImportedName()
		{
			Assert.IsNull(resolverContext.GetModuleForImportedName("exit"));
		}
		
		[Test]
		public void ResolverContextUnaliasImportedNameReturnsMyExitForExitImportedName()
		{
			Assert.AreEqual("exit", resolverContext.UnaliasImportedName("myexit"));
		}
		
		[Test]
		public void ResolverContextUnaliasImportedNameReturnsOriginalNameWhenNoAliasIsFound()
		{
			Assert.AreEqual("unknown", resolverContext.UnaliasImportedName("unknown"));
		}
	}
}
