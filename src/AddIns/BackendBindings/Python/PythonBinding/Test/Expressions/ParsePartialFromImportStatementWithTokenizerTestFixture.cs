// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Providers;
using Microsoft.Scripting.Runtime;
using ICSharpCode.PythonBinding;
using IronPython;
using IPyCompiler = IronPython.Compiler;
using IronPython.Compiler.Ast;
using IronPython.Hosting;
using IronPython.Runtime;
using NUnit.Framework;

namespace PythonBinding.Tests.Expressions
{
	[TestFixture]
	public class ParsePartialFromImportStatementWithTokenizerTestFixture
	{
		IPyCompiler.Tokenizer tokenizer;
		
		public IPyCompiler.Tokenizer CreateTokenizer(string text)
		{
			ScriptEngine engine = Python.CreateEngine();
			PythonContext context = HostingHelpers.GetLanguageContext(engine) as PythonContext;
			
			StringTextContentProvider textProvider = new StringTextContentProvider(text);
			SourceUnit source = context.CreateSourceUnit(textProvider, String.Empty, SourceCodeKind.SingleStatement);

			PythonCompilerSink sink = new PythonCompilerSink();
			IPyCompiler.PythonCompilerOptions options = new IPyCompiler.PythonCompilerOptions();

			tokenizer = new IPyCompiler.Tokenizer(sink, options);
			tokenizer.Initialize(source);
			return tokenizer;
		}
		
		[Test]
		public void FirstTokenIsFrom()
		{
			string text = "from";
			tokenizer = CreateTokenizer(text);
			IPyCompiler.Token token = tokenizer.GetNextToken();
			Assert.AreEqual(IPyCompiler.TokenKind.KeywordFrom, token.Kind);
		}
		
		[Test]
		public void TokenAfterFromIsEof()
		{
			FirstTokenIsFrom();
			IPyCompiler.Token token = tokenizer.GetNextToken();

			Assert.AreEqual(IPyCompiler.TokenKind.EndOfFile, token.Kind);
		}
		
		[Test]
		public void FromSystemImportSecondTokenIsModule()
		{
			string text = "from System";
			IPyCompiler.Token token = GetSecondToken(text);
			
			Assert.AreEqual(IPyCompiler.TokenKind.Name, token.Kind);
		}
		
		IPyCompiler.Token GetSecondToken(string text)
		{
			return GetToken(text, 2);
		}
		
		IPyCompiler.Token GetToken(string text, int tokenNumber)
		{
			tokenizer = CreateTokenizer(text);
			
			IPyCompiler.Token token = null;
			for (int i = 0; i < tokenNumber; ++i) {
				token = tokenizer.GetNextToken();
			}
			return token;
		}
		
		[Test]
		public void FromSystemImportSecondTokenValueIsSystem()
		{
			string text = "from System";
			IPyCompiler.Token token = GetSecondToken(text);
			
			Assert.AreEqual("System", token.Value as String);
		}
		
		[Test]
		public void FromSystemConsoleImportSecondTokenValueIsSystem()
		{
			string text = "from System.Console";
			IPyCompiler.Token token = GetSecondToken(text);
			
			Assert.AreEqual("System", token.Value as String);
		}
		
		[Test]
		public void FromSystemConsoleImportThirdTokenValueIsDotCharacter()
		{
			FromSystemConsoleImportSecondTokenValueIsSystem();
			IPyCompiler.Token token = tokenizer.GetNextToken();
			
			Assert.AreEqual(IPyCompiler.TokenKind.Dot, token.Kind);
		}
		
		[Test]
		public void FromSystemImportThirdTokenIsImportToken()
		{
			string text = "from System import";
			IPyCompiler.Token token = GetThirdToken(text);
			
			Assert.AreEqual(IPyCompiler.TokenKind.KeywordImport, token.Kind);
		}
		
		IPyCompiler.Token GetThirdToken(string text)
		{
			return GetToken(text, 3);
		}
		
		[Test]
		public void FromSystemImportIdentifierFourthTokenIsIndentifierToken()
		{
			string text = "from System import abc";
			IPyCompiler.Token token = GetFourthToken(text);
			
			Assert.AreEqual("abc", token.Value as String);
		}
		
		IPyCompiler.Token GetFourthToken(string text)
		{
			return GetToken(text, 4);
		}
	}
}
