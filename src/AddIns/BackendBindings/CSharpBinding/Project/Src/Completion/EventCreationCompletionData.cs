// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
//
using System;
using System.Linq;
using System.Threading;
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
				context.Editor.Document.Replace(context.StartOffset, context.Length, handlerName);
				context.EndOffset = context.StartOffset + handlerName.Length;
				
				using (var script = refactoringContext.StartScript()) {
					script.InsertWithCursor(this.DisplayText, Script.InsertPosition.Before, decl)
						// TODO : replace with Link, once that is implemented
						.ContinueScript(() => script.Select(throwStatement));
				}
			}
		}
	}
}