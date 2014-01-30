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

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests;

namespace PythonBinding.Tests.Parsing
{
	/// <summary>
	/// The IronPython 2.0.2 parser will throw an invalid cast exception for the following code:
	/// 
	/// class Project(id): 
	///     def __init__ Project_ID(): 
	///         #i
	/// 
	/// System.InvalidCastException: Unable to cast object of type 'IronPython.Compiler.Ast.ErrorExpression' to type 'IronPython.Compiler.Ast.NameExpression'.
    /// at IronPython.Compiler.Parser.ParseParameter(Int32 position, Dictionary`2 names)
    /// at IronPython.Compiler.Parser.ParseVarArgsList(TokenKind terminator)
    /// at IronPython.Compiler.Parser.ParseFuncDef()
    /// at IronPython.Compiler.Parser.ParseStmt()
    /// at IronPython.Compiler.Parser.ParseSuite()
    /// at IronPython.Compiler.Parser.ParseClassDef()
    /// at IronPython.Compiler.Parser.ParseStmt()
    /// at IronPython.Compiler.Parser.ParseFile(Boolean makeModule)
    /// at ICSharpCode.PythonBinding.PythonParser.CreateAst(String fileName, String fileContent)
    /// 
    /// This test just ensures that this bug is fixed with IronPython 2.6
	/// </summary>
	[TestFixture]
	public class InvalidCastInPythonParserTestFixture
	{
		string code =
			"class Project(id): \r\n" +
			"    def __init__ Project_ID(): \r\n" +
			"        #i\r\n";
		
		/// <summary>
		/// Check that IronPython bug has been fixed exists.
		/// </summary>
		[Test]
		public void CreateAstShouldNotThrowInvalidCastException()
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"d:\projects\test\test.py", new StringTextBuffer(code));
		}
		
		[Test]
		public void ParseShouldNotThrowInvalidCastException()
		{
			PythonParser parser = new PythonParser();
			ICompilationUnit unit = parser.Parse(new DefaultProjectContent(), @"d:\projects\test\test.py", code);
			Assert.AreEqual(1, unit.Classes.Count);
		}
	}
}
