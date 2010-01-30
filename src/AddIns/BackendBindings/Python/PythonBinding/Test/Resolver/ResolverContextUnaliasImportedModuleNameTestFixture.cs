// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
