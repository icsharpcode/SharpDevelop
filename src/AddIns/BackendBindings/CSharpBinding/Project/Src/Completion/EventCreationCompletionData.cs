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
	class EventCreationCompletionData : EntityCompletionData
	{
		IEvent eventDefinition;
		IType delegateType;

		public EventCreationCompletionData(string varName, IType delegateType, IEvent evt, string parameterList, IUnresolvedMember callingMember, IUnresolvedTypeDefinition declaringType, CSharpResolver contextAtCaret) : base(evt)
		{
			if (string.IsNullOrEmpty(varName)) {
				this.DisplayText = "Create handler for " + (evt != null ? evt.Name : "");
			}
			else {
				this.DisplayText = "Create handler for " + char.ToUpper(varName[0]) + varName.Substring(1) + (evt != null ? evt.Name : "");
			}
			
			this.DisplayText = "<" + this.DisplayText + ">";
			this.eventDefinition = evt;
			this.delegateType = delegateType;
		}

		public override void Complete(CompletionContext context)
		{
			var invokeSignature = delegateType.GetMethods(m => m.Name == "Invoke").Single();
			var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
			var builder = refactoringContext.CreateTypeSystemAstBuilder();
			var handlerName = eventDefinition.Name;
			
			var throwStatement = new ThrowStatement();
			var decl = new MethodDeclaration {
				ReturnType = refactoringContext.CreateShortType(invokeSignature.ReturnType),
				Name = handlerName,
				Body = new BlockStatement {
					throwStatement
				}
			};
			
			decl.Parameters.AddRange(invokeSignature.Parameters.Select(builder.ConvertParameter));
			
			if (eventDefinition.IsStatic)
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


