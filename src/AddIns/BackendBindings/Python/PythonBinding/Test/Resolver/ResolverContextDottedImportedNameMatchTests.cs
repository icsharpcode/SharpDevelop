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
	public class ResolverContextDottedImportedNameMatchTests
	{
		[Test]
		public void HasDottedImportNameReturnsTrueForSystemWithSystemWindowsFormsImported()
		{
			string code = "import System.Windows.Forms";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsTrue(resolverContext.HasDottedImportNameThatStartsWith("System"));
		}
		
		[Test]
		public void HasDottedImportNameReturnsFalseForMyNamespaceWithMyNamespaceTestImportedWithoutDtso()
		{
			string code = "import MyNamespaceTest";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			Assert.IsFalse(resolverContext.HasDottedImportNameThatStartsWith("MyNamespace"));
		}
	}
}
