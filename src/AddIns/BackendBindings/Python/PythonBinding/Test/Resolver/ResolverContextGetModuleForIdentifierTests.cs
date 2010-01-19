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
	public class ResolverContextGetModuleForIdentifierTestFixture
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
		public void ResolverContextGetModuleForIdentifierReturnsSysForExitIdentifier()
		{
			Assert.AreEqual("sys", resolverContext.GetModuleForIdentifier("exit"));
		}
		
		[Test]
		public void ResolverContextGetModuleForIdentifierReturnsNullForUnknownIdentifier()
		{
			Assert.IsNull(resolverContext.GetModuleForIdentifier("unknown"));
		}
		
		[Test]
		public void ResolverContextGetImportedIdentifierNameReturnsExitForExitIdentifier()
		{
			Assert.AreEqual("exit", resolverContext.UnaliasIdentifier("exit"));
		}
	}
}
