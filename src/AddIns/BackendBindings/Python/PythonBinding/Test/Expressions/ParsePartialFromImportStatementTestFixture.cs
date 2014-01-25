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
using System.IO;
using System.Text;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using ICSharpCode.PythonBinding;
using IronPython;
using IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Hosting;
using IronPython.Runtime;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class ParsePartialFromImportStatementTestFixture
	{
		FromImportStatement fromStatement;
		
		public FromImportStatement ParseStatement(string text)
		{
			ScriptEngine engine = Python.CreateEngine();
			PythonContext context = HostingHelpers.GetLanguageContext(engine) as PythonContext;
			
			StringTextContentProvider textProvider = new StringTextContentProvider(text);
			SourceUnit source = context.CreateSourceUnit(textProvider, String.Empty, SourceCodeKind.SingleStatement);

			PythonCompilerSink sink = new PythonCompilerSink();
			CompilerContext compilerContext = new CompilerContext(source, new PythonCompilerOptions(), sink);

			PythonOptions options = new PythonOptions();
			using (Parser parser = Parser.CreateParser(compilerContext, options)) {
				return parser.ParseSingleStatement().Body as FromImportStatement;
			}
		}
		
		[Test]
		public void FromSystemImportStatementFollowedByIdentifierHasModuleNameOfSystem()
		{
			string text = "from System import abc";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("System", fromStatement.Root.MakeString());
		}
		
		[Test]
		public void FromSystemImportStatementFollowedByIdentifierHasIdentifierInNamesCollection()
		{
			FromSystemImportStatementFollowedByIdentifierHasModuleNameOfSystem();
			Assert.IsTrue(fromStatement.Names.Contains("abc"));
		}
		
		[Test]
		public void FromSystemImportStatementFollowedByEmptySpaceHasModuleNameOfSystem()
		{
			string text = "from System import ";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("System", fromStatement.Root.MakeString());
		}
		
		[Test]
		public void FromSystemStatementWithNoImportHasModuleNameOfSystem()
		{
			string text = "from System";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("System", fromStatement.Root.MakeString());
		}
		
		[Test]
		public void FromStatementFollowedBySpaceCharButNoModuleNameHasModuleNameOfEmptyString()
		{
			string text = "from ";
			fromStatement = ParseStatement(text);
			Assert.AreEqual(String.Empty, fromStatement.Root.MakeString());
		}
		
		[Test]
		[Ignore("Does not work")]
		public void FromSystemImportStatementFollowedByIdentifierAsNameHasIdentifierAsNameInAsNamesCollection()
		{
			string text = "from System import abc as def";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("def", fromStatement.AsNames[0]);
		}
		
		[Test]
		[Ignore("Does not work")]
		public void FromSystemImportStatementFollowedByIdentifierAsNameFollowedBySpaceHasIdentifierAsNameInAsNamesCollection()
		{
			string text = "from System import abc as def ";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("def", fromStatement.AsNames[0]);
		}
		
		[Test]
		[Ignore("Does not work")]
		public void FromSystemImportStatementFollowedByIdentifierAsNameFollowedByNewLineHasIdentifierAsNameInAsNamesCollection()
		{
			string text = "from System import abc as def\r\n";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("def", fromStatement.AsNames[0]);
		}
		
		[Test]
		[Ignore("Does not work")]
		public void FromSystemImportStatementFollowedByMultipleIdentifierAsNameFollowedBySpaceHasIdentifierAsNameInAsNamesCollection()
		{
			string text = "from System import abc as def, ghi as jkl";
			fromStatement = ParseStatement(text);
			Assert.AreEqual("def", fromStatement.AsNames[0]);
		}
	}
}
