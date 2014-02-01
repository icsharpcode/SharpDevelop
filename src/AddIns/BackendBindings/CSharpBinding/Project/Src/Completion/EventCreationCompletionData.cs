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
using System.Threading;
using ICSharpCode.SharpDevelop;
using CSharpBinding.Parser;
using CSharpBinding.Refactoring;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Completion item that creates an event handler for an event.
	/// </summary>
	class EventCreationCompletionData : CompletionData
	{
		IEvent eventDefinition;
		string varName;
		IType delegateType;
		string parameterList;
		IUnresolvedMember callingMember;
		IUnresolvedTypeDefinition declaringType;
		CSharpResolver contextAtCaret;

		public EventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterList, IUnresolvedMember callingMember, IUnresolvedTypeDefinition declaringType, CSharpResolver contextAtCaret)
		{
			if (string.IsNullOrEmpty(varName)) {
				this.DisplayText = "<Create handler for " + (evt != null ? evt.Name : "") + ">";
			}
			else {
				this.DisplayText = "Handle" + char.ToUpper(varName[0]) + varName.Substring(1) + (evt != null ? evt.Name : "");
			}
			
			this.varName = varName;
			this.eventDefinition = evt;
			this.delegateType = delegateType;
			this.parameterList = parameterList;
			this.callingMember = callingMember;
			this.declaringType = declaringType;
			this.contextAtCaret = contextAtCaret;
		}

		public override void Complete(CompletionContext context)
		{
			var invokeSignature = delegateType.GetMethods(m => m.Name == "Invoke").Single();
			var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
			var builder = refactoringContext.CreateTypeSystemAstBuilder();
			string handlerName;
			bool isStatic;
			if (eventDefinition != null) {
				handlerName = eventDefinition.Name;
				isStatic = eventDefinition.IsStatic;
			} else {
				handlerName = varName;
				isStatic = callingMember.IsStatic;
			}
			
			var throwStatement = new ThrowStatement();
			var decl = new MethodDeclaration {
				ReturnType = refactoringContext.CreateShortType(invokeSignature.ReturnType),
				Name = handlerName,
				Body = new BlockStatement {
					throwStatement
				}
			};
			
			decl.Parameters.AddRange(invokeSignature.Parameters.Select(builder.ConvertParameter));
			
			if (isStatic)
				decl.Modifiers |= Modifiers.Static;
			
			throwStatement.Expression = new ObjectCreateExpression(refactoringContext.CreateShortType("System", "NotImplementedException"));
			
			// begin insertion
			using (context.Editor.Document.OpenUndoGroup()) {
				context.Editor.Document.Replace(context.StartOffset, context.Length, handlerName + ";");
				context.EndOffset = context.StartOffset + handlerName.Length;
				var loc = context.Editor.Document.GetLocation(context.StartOffset + handlerName.Length / 2 + 1);
				
				var parseInfo = SD.ParserService.Parse(context.Editor.FileName, context.Editor.Document) as CSharpFullParseInformation;
				if (parseInfo == null) return;
				
				using (var script = refactoringContext.StartScript()) {
					var node = parseInfo.SyntaxTree.GetNodeAt(loc, n => n is Identifier || n is IdentifierExpression);
					if (node == null) return;
					script.InsertWithCursor(this.DisplayText, Script.InsertPosition.Before, decl)
						.ContinueScript(() => script.Link(decl.NameToken, node).ContinueScript(() => script.Select(throwStatement)));
				}
			}
		}
	}
}
