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
			var unresolvedMember = file.GetMember(context.Location);
			if (unresolvedMember == null)
				return "";
			var member = unresolvedMember.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
			if (member == null)
				return "";
			var builder = new TypeSystemAstBuilder();
			MethodDeclaration decl;
			if (unresolvedMember is IMethod) {
				// If it's a method, convert it directly (including parameters + type parameters)
				decl = (MethodDeclaration)builder.ConvertEntity(member);
			} else {
				// Otherwise, create a method anyways, and copy the parameters
				decl = new MethodDeclaration();
				if (member is IParameterizedMember) {
					foreach (var p in ((IParameterizedMember)member).Parameters) {
						decl.Parameters.Add(builder.ConvertParameter(p));
					}
				}
			}
			decl.Name = "__DebuggerStub__";
			decl.ReturnType = builder.ConvertType(member.ReturnType);
			decl.Modifiers = unresolvedMember.IsStatic ? Modifiers.Static : Modifiers.None;
			// Make the method look like an explicit interface implementation so that it doesn't appear in CC
			decl.PrivateImplementationType = new SimpleType("__DummyType__");
			decl.Body = GenerateBodyFromContext(builder, context);
			return WrapInType(unresolvedMember.DeclaringTypeDefinition, decl).ToString();
		}

		static BlockStatement GenerateBodyFromContext(TypeSystemAstBuilder builder, DebuggerCompletionContext context)
		{
			var body = new BlockStatement();
			foreach (var v in context.Variables)
				body.Statements.Add(new VariableDeclarationStatement(builder.ConvertType(v.Type), v.Name));
			body.Statements.Add(new ExpressionStatement(new IdentifierExpression("$__Caret_Point__$")));
			return body;
		}

		static AstNode WrapInType(IUnresolvedTypeDefinition entity, EntityDeclaration decl)
		{
			if (entity == null)
				return decl;
			// Wrap decl in TypeDeclaration
			decl = new TypeDeclaration {
				ClassType = GetClassType(entity),
				Modifiers = Modifiers.Partial,
				Name = entity.Name,
				Members = { decl }
			};
			if (entity.DeclaringTypeDefinition != null) {
				// Handle nested types
				return WrapInType(entity.DeclaringTypeDefinition, decl);
			} 
			if (string.IsNullOrEmpty(entity.Namespace))
				return decl;
			return new NamespaceDeclaration(entity.Namespace) {
				Members = {
					decl
				}
			};
		}

		static ClassType GetClassType(IUnresolvedTypeDefinition entity)
		{
			switch (entity.Kind) {
				case TypeKind.Interface:
					return ClassType.Interface;
				case TypeKind.Struct:
					return ClassType.Struct;
				default:
					return ClassType.Class;
			}
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

