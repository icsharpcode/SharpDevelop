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
	public class ResolverContextGetImportAllModulesTests
	{
		[Test]
		public void GetModulesThatImportEverythingReturnsEmptyCollectionIfNotImportAll()
		{
			string code = "from math import tan";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			
			string[] expectedModules = new string[0];
			Assert.AreEqual(expectedModules, resolverContext.GetModulesThatImportEverything());
		}
		
		[Test]
		public void GetModulesThatImportEverythingReturnsSysForFromSysImportAllStatement()
		{
			string code = "from sys import *";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			
			string[] expectedModules = new string[] { "sys" };
			Assert.AreEqual(expectedModules, resolverContext.GetModulesThatImportEverything());
		}
		
		[Test]
		public void GetModulesThatImportEverythingReturnsSysAndMathForFromSysImportAllStatement()
		{
			string code =
				"from sys import *\r\n" +
				"from math import *";
			
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			
			string[] expectedModules = new string[] { "sys", "math" };
			Assert.AreEqual(expectedModules, resolverContext.GetModulesThatImportEverything());
		}
		
		[Test]
		public void GetModulesThatImportEverythingIgnoresNonFromImportStatement()
		{
			string code = 
				"import math\r\n" +
				"from sys import *";
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			
			PythonResolverContext resolverContext = new PythonResolverContext(parseInfo);
			
			string[] expectedModules = new string[] { "sys" };
			Assert.AreEqual(expectedModules, resolverContext.GetModulesThatImportEverything());
		}

	}
}
