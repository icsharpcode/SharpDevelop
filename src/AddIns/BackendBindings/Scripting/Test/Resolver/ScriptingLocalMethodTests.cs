// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Resolver
{
	[TestFixture]
	public class ScriptingLocalMethodTests
	{
		ScriptingLocalMethod method;
		
		void CreateLocalMethod(string code)
		{
			method = new ScriptingLocalMethod(code);
		}
		
		[Test]
		public void GetCode_EndLineIsOneAndTwoLinesOfCode_ReturnsFirstLineOfCode()
		{
			string fullCode =
				"first\r\n" +
				"second";
			
			CreateLocalMethod(fullCode);
			
			int endLine = 1;
			string code = method.GetCode(endLine);
			
			string expectedCode = "first\r\n";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void GetCode_EndLineIsTwoAndThreeLinesOfCode_ReturnsFirstTwoLinesOfCode()
		{
			string fullCode =
				"first\r\n" +
				"second\r\n" +
				"third";
			
			CreateLocalMethod(fullCode);
			
			int endLine = 2;
			string code = method.GetCode(endLine);
			
			string expectedCode =
				"first\r\n" +
				"second\r\n";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void GetCode_EndLineIsTwoAndTwoLinesOfCode_ReturnsFirstTwoLinesOfCode()
		{
			string fullCode =
				"first\r\n" +
				"second";
			
			CreateLocalMethod(fullCode);
			
			int endLine = 2;
			string code = method.GetCode(endLine);
			
			string expectedCode =
				"first\r\n" +
				"second";
			
			Assert.AreEqual(expectedCode, code);
		}
		
		[Test]
		public void GetCode_EndLineIsTwoAndCodeIsNull_ReturnsEmptyString()
		{
			string fullCode = null;
			
			CreateLocalMethod(fullCode);
			
			int endLine = 2;
			string code = method.GetCode(endLine);
			
			string expectedCode = String.Empty;
			
			Assert.AreEqual(expectedCode, code);
		}
	}
}
