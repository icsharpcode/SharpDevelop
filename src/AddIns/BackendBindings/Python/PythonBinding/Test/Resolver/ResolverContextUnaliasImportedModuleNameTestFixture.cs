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
	public class ResolverContextUnaliasImportedModuleNameTestFixture
	{
		PythonResolverContext resolverContext;
		ParseInformation parseInfo;
		
		[SetUp]
		public void Init()
		{
			string python =
				"import math as m\r\n" +
				"import sys as s";
			
			parseInfo = PythonParserHelper.CreateParseInfo(python);
			resolverContext = new PythonResolverContext(parseInfo);
		}
		
		[Test]
		public void ResolverContextUnaliasImportedModuleReturnsSysForImportedAsName()
		{
			Assert.AreEqual("sys", resolverContext.UnaliasImportedModuleName("s"));
		}
		
		[Test]
		public void ResolverContextUnaliasImportedModuleReturnsMathForImportedAsName()
		{
			Assert.AreEqual("math", resolverContext.UnaliasImportedModuleName("m"));
		}
	}
}
