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
	public class ResolverContextGetModuleForIdentifierImportedAsDifferentNameTestFixture
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
		public void ResolverContextGetModuleForIdentifierReturnsSysForMyExitIdentifier()
		{
			Assert.AreEqual("sys", resolverContext.GetModuleForIdentifier("myexit"));
		}
		
		[Test]
		public void ResolverContextGetModuleForIdentifierReturnsNullForExitIdentifier()
		{
			Assert.IsNull(resolverContext.GetModuleForIdentifier("exit"));
		}
		
		[Test]
		public void ResolverContextUnaliasIdentiferReturnsMyExitForExitIdentifier()
		{
			Assert.AreEqual("exit", resolverContext.UnaliasIdentifier("myexit"));
		}
		
		[Test]
		public void ResolverContextUnaliasIdentifierReturnsOriginalNameWhenNoAliasIsFound()
		{
			Assert.AreEqual("unknown", resolverContext.UnaliasIdentifier("unknown"));
		}
	}
}
