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
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// Tests that if one part of a class is invalid the parser
	/// still returns a class as part of the compilation unit. 
	/// This is better than returning nothing.
	/// </summary>
	[TestFixture]
	public class InvalidClassTestFixture
	{
		ICompilationUnit compilationUnit;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			string python = "import System\r\n" +
							"\r\n" +
							"class Class1:\r\n" +
							"	def __init__(self):\r\n" +
							"		Console.\r\n" +
							"		pass\r\n";

			DefaultProjectContent projectContent = new DefaultProjectContent();
			PythonParser parser = new PythonParser();
			compilationUnit = parser.Parse(projectContent, @"C:\test.py", python);			
		}
		
		[Test]
		public void OneClassFound()
		{
			Assert.AreEqual(1, compilationUnit.Classes.Count);
		}
				
		[Test]
		public void FileNameSet()
		{
			Assert.AreEqual(@"C:\test.py", compilationUnit.FileName);
		}
	}
}
