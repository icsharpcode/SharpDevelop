// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
