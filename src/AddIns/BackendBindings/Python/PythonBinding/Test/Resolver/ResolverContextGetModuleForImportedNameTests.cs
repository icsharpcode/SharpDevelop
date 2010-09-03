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
	public class ResolverContextGetModuleForImportedNameTestFixture
	{
		PythonResolverContext resolverContext;
		ParseInformation parseInfo;
		
		[SetUp]
		public void Init()
		{
			string python =
				"import math\r\n" +
				"from sys import exit";
			
			parseInfo = PythonParserHelper.CreateParseInfo(python);
			resolverContext = new PythonResolverContext(parseInfo);
		}
		
		[Test]
		public void ResolverContextGetModuleForImportedNameReturnsSysForExitImportedName()
		{
			Assert.AreEqual("sys", resolverContext.GetModuleForImportedName("exit"));
		}
		
		[Test]
		public void ResolverContextGetModuleForImportedNameReturnsNullForUnknownImportedName()
		{
			Assert.IsNull(resolverContext.GetModuleForImportedName("unknown"));
		}
		
		[Test]
		public void ResolverContextUnaliasImportedNameReturnsExitForExitImportedName()
		{
			Assert.AreEqual("exit", resolverContext.UnaliasImportedName("exit"));
		}
	}
}
