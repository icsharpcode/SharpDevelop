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
using System.Linq;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace Debugger.AddIn.Pads.Controls
{
	static class DebuggerDotCompletion
	{
		public static bool CheckSyntax(string expression, out Error[] errors)
		{
			var p = new CSharpParser();
			p.ParseExpression(expression);
			errors = p.Errors.ToArray();
			return !errors.Any();
		}
		
		public static ICodeCompletionBinding PrepareDotCompletion(string expressionToComplete, DebuggerCompletionContext context)
		{
			var lang = SD.LanguageService.GetLanguageByFileName(context.FileName);
			if (lang == null)
				return null;
			string content = GeneratePartialClassContextStub(context);
			const string caretPoint = "$__Caret_Point__$;";
			int caretOffset = content.IndexOf(caretPoint, StringComparison.Ordinal) + expressionToComplete.Length;
			SD.Log.DebugFormatted("context used for dot completion: {0}", content.Replace(caretPoint, "$" + expressionToComplete + "|$"));
			var doc = new ReadOnlyDocument(content.Replace(caretPoint, expressionToComplete));
			return lang.CreateCompletionBinding(context.FileName, doc.GetLocation(caretOffset), doc.CreateSnapshot());
		}

		static string GeneratePartialClassContextStub(DebuggerCompletionContext context)
		{
			var compilation = SD.ParserService.GetCompilationForFile(context.FileName);
			var file = SD.ParserService.GetExistingUnresolvedFile(context.FileName);
			if (compilation == null || file == null)
				return "";
			var member = file.GetMember(context.Location);
			if (member == null)
				return "";
			var builder = new TypeSystemAstBuilder();
			EntityDeclaration decl = builder.ConvertEntity(member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly)));
			decl.Name = "__DebuggerStub_" + decl.Name;
			decl.Modifiers &= (Modifiers.Static);
			switch (member.SymbolKind) {
				case SymbolKind.Property:
					break;
				case SymbolKind.Indexer:
					break;
				case SymbolKind.Event:
					break;
				case SymbolKind.Method:
					GenerateBodyFromContext(builder, context, (MethodDeclaration)decl);
					break;
				case SymbolKind.Operator:
					break;
				case SymbolKind.Constructor:
					break;
				case SymbolKind.Destructor:
					break;
				case SymbolKind.Accessor:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			return WrapInType(member.DeclaringTypeDefinition, decl.ToString());
		}

		static void GenerateBodyFromContext(TypeSystemAstBuilder builder, DebuggerCompletionContext context, MethodDeclaration methodDeclaration)
		{
			methodDeclaration.Body = new BlockStatement();
			foreach (var v in context.Variables)
				methodDeclaration.Body.Statements.Add(new VariableDeclarationStatement(builder.ConvertType(v.Type), v.Name));
			methodDeclaration.Body.Statements.Add(new ExpressionStatement(new IdentifierExpression("$__Caret_Point__$")));
		}

		static string WrapInType(IUnresolvedTypeDefinition entity, string code)
		{
			if (entity == null)
				return code;
			code = WrapInType(entity.DeclaringTypeDefinition, GetHeader(entity) + code + "\r\n}");
			if (entity.DeclaringTypeDefinition == null) {
				return "namespace " + entity.Namespace + " {\r\n" + code + "\r\n}";
			}
			return code;
		}

		static string GetHeader(IUnresolvedTypeDefinition entity)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("partial ");
			switch (entity.Kind) {
				case TypeKind.Class:
					builder.Append("class ");
					break;
				case TypeKind.Interface:
					builder.Append("interface ");
					break;
				case TypeKind.Struct:
					builder.Append("struct ");
					break;
				default:
					throw new NotSupportedException();
			}
			builder.Append(entity.Name);
			builder.AppendLine(" {");
			return builder.ToString();
		}
	}

	public class LocalVariable
	{
		readonly IType type;

		public IType Type {
			get {
				return type;
			}
		}
		
		readonly string name;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public LocalVariable(IType type, string name)
		{
			this.type = type;
			this.name = name;
		}
	}
	
	public class DebuggerCompletionContext
	{
		readonly FileName fileName;
		TextLocation location;
		readonly LocalVariable[] variables;
		
		public DebuggerCompletionContext(StackFrame frame)
		{
			fileName = new FileName(frame.NextStatement.Filename);
			location = new TextLocation(frame.NextStatement.StartLine, frame.NextStatement.StartColumn);
			variables = frame.GetLocalVariables().Select(v => new LocalVariable(v.Type, v.Name)).ToArray();
		}
		
		public DebuggerCompletionContext(FileName fileName, TextLocation location)
		{
			this.fileName = fileName;
			this.location = location;
			this.variables = SD.ParserService.ResolveContext(fileName, location)
				.LocalVariables.Select(v => new LocalVariable(v.Type, v.Name)).ToArray();
		}
		
		public FileName FileName {
			get {
				return fileName;
			}
		}
		
		public TextLocation Location {
			get {
				return location;
			}
		}

		public LocalVariable[] Variables {
			get {
				return variables;
			}
		}
	}
}

