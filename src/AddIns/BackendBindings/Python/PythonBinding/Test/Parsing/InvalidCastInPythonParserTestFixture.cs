// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		string code = "class Project(id): \r\n" +
					  "    def __init__ Project_ID(): \r\n" +
					  "        #i\r\n";
		
		/// <summary>
		/// Check that IronPython bug has been fixed exists.
		/// </summary>
		[Test]
		public void CreateAstShouldNotThrowInvalidCastException()
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"d:\projects\test\test.py", code);
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
