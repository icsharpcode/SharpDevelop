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
