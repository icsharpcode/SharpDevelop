// 
// CreateEventInvocator.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Mike Krüger <mkrueger@novell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Create event invocator", Description = "Creates a standard OnXXX event method.")]
	public class CreateEventInvocatorAction : CodeActionProvider
	{
		/// <summary>
		/// If <c>true</c> an explicit type will be used for the handler variable; otherwise, 'var' will be used as type.
		/// Default value is <c>false</c>
		/// </summary>
		public bool UseExplictType {
			get;
			set;
		}

		public static MethodDeclaration CreateEventInvocator (RefactoringContext context, TypeDeclaration declaringType, EventDeclaration eventDeclaration, VariableInitializer initializer, IMethod invokeMethod, bool useExplictType)
		{
			bool hasSenderParam = false;
			IEnumerable<IParameter> pars = invokeMethod.Parameters;
			if (invokeMethod.Parameters.Any()) {
				var first = invokeMethod.Parameters [0];
				if (first.Name == "sender" /*&& first.Type == "System.Object"*/) {
					hasSenderParam = true;
					pars = invokeMethod.Parameters.Skip(1);
				}
			}
			const string handlerName = "handler";

			var arguments = new List<Expression>();
			if (hasSenderParam)
				arguments.Add(eventDeclaration.HasModifier (Modifiers.Static) ? (Expression)new PrimitiveExpression (null) : new ThisReferenceExpression());
			bool useThisMemberReference = false;
			foreach (var par in pars) {
				arguments.Add(new IdentifierExpression(par.Name));
				useThisMemberReference |= par.Name == initializer.Name;
			}
			var proposedHandlerName = GetNameProposal(initializer);
			var modifiers = eventDeclaration.HasModifier(Modifiers.Static) ? Modifiers.Static : Modifiers.Protected | Modifiers.Virtual;
			if (declaringType.HasModifier (Modifiers.Sealed)) {
				modifiers = Modifiers.None;
			}
			var methodDeclaration = new MethodDeclaration {
				Name = proposedHandlerName,
				ReturnType = new PrimitiveType ("void"),
				Modifiers = modifiers,
				Body = new BlockStatement {
					new VariableDeclarationStatement (
						useExplictType ? eventDeclaration.ReturnType.Clone () : new PrimitiveType ("var"), handlerName, 
						useThisMemberReference ? 
						(Expression)new MemberReferenceExpression (new ThisReferenceExpression (), initializer.Name) 
						: new IdentifierExpression (initializer.Name)
						),
					new IfElseStatement {
						Condition = new BinaryOperatorExpression (new IdentifierExpression (handlerName), BinaryOperatorType.InEquality, new PrimitiveExpression (null)),
						TrueStatement = new InvocationExpression (new IdentifierExpression (handlerName), arguments)
					}
				}
			};

			foreach (var par in pars) {
				var typeName = context.CreateShortType(par.Type);
				var decl = new ParameterDeclaration(typeName, par.Name);
				methodDeclaration.Parameters.Add(decl);
			}
			return methodDeclaration;
		}

		static string GetNameProposal(VariableInitializer initializer)
		{
			return "On" + char.ToUpper(initializer.Name[0]) + initializer.Name.Substring(1);
		}

		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			VariableInitializer initializer;
			var eventDeclaration = GetEventDeclaration(context, out initializer);
			if (eventDeclaration == null) {
				yield break;
			}
			var type = (TypeDeclaration)eventDeclaration.Parent;
			var proposedHandlerName = GetNameProposal(initializer);
			if (type.Members.Any(m => m is MethodDeclaration && m.Name == proposedHandlerName)) {
				yield break;
			}
			var resolvedType = context.Resolve(eventDeclaration.ReturnType).Type;
			if (resolvedType.Kind == TypeKind.Unknown) {
				yield break;
			}
			var invokeMethod = resolvedType.GetDelegateInvokeMethod();
			if (invokeMethod == null) {
				yield break;
			}
			yield return new CodeAction(context.TranslateString("Create event invocator"), script => {
				var methodDeclaration = CreateEventInvocator (context, type, eventDeclaration, initializer, invokeMethod, UseExplictType);
				script.InsertWithCursor(
					context.TranslateString("Create event invocator"),
					Script.InsertPosition.After,
					methodDeclaration
				);
			}, initializer);
		}

		static EventDeclaration GetEventDeclaration (RefactoringContext context, out VariableInitializer initializer)
		{
			var result = context.GetNode<EventDeclaration> ();
			if (result == null) {
				initializer = null;
				return null;
			}
			initializer = result.Variables.FirstOrDefault (v => v.NameToken.Contains (context.Location));
			return initializer != null ? result : null;
		}
	}
}

