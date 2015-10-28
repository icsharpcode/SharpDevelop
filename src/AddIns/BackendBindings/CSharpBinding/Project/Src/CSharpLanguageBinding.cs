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
using System.Collections.Generic;

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;
using CSharpBinding.Completion;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Refactoring;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpLanguageBinding.
	/// </summary>
	public class CSharpLanguageBinding : DefaultLanguageBinding
	{
		public CSharpLanguageBinding()
		{
			this.container.AddService(typeof(IFormattingStrategy), new CSharpFormattingStrategy());
			this.container.AddService(typeof(IBracketSearcher), new CSharpBracketSearcher());
			this.container.AddService(typeof(CodeGenerator), new CSharpCodeGenerator());
			this.container.AddService(typeof(System.CodeDom.Compiler.CodeDomProvider), new Microsoft.CSharp.CSharpCodeProvider());
		}
		
		public override ICodeCompletionBinding CreateCompletionBinding(string expressionToComplete, ICodeContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			string content = GeneratePartialClassContextStub(context);
			const string caretPoint = "$__Caret_Point__$;";
			int caretOffset = content.IndexOf(caretPoint, StringComparison.Ordinal) + expressionToComplete.Length;
			SD.Log.DebugFormatted("context used for dot completion: {0}", content.Replace(caretPoint, "$" + expressionToComplete + "|$"));
			var doc = new ReadOnlyDocument(content.Replace(caretPoint, expressionToComplete));
			return new CSharpCompletionBinding(context, doc.GetLocation(caretOffset), doc.CreateSnapshot());
		}
		
		static string GeneratePartialClassContextStub(ICodeContext context)
		{
			var member = context.CurrentMember;
			if (member == null)
				return "";
			var builder = new TypeSystemAstBuilder();
			MethodDeclaration decl;
			if (member.SymbolKind == SymbolKind.Method) {
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
			decl.Modifiers = member.IsStatic ? Modifiers.Static : Modifiers.None;
			// Make the method look like an explicit interface implementation so that it doesn't appear in CC
			decl.PrivateImplementationType = new SimpleType("__DummyType__");
			decl.Body = GenerateBodyFromContext(builder, context.LocalVariables.ToArray());
			return WrapInType(context.CurrentTypeDefinition, decl).ToString();
		}

		static BlockStatement GenerateBodyFromContext(TypeSystemAstBuilder builder, IVariable[] variables)
		{
			var body = new BlockStatement();
			foreach (var v in variables)
				body.Statements.Add(new VariableDeclarationStatement(builder.ConvertType(v.Type), v.Name));
			body.Statements.Add(new ExpressionStatement(new IdentifierExpression("$__Caret_Point__$")));
			return body;
		}

		static AstNode WrapInType(ITypeDefinition entity, EntityDeclaration decl)
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

		static ClassType GetClassType(ITypeDefinition entity)
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
}
