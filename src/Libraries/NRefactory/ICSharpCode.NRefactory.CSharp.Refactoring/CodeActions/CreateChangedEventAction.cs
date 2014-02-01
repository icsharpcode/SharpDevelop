//
// CreateChangedEvent.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Create changed event for property", Description = "Creates a changed event for an property.")]
	public class CreateChangedEventAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var property = context.GetNode<PropertyDeclaration>();
			if (property == null || !property.NameToken.Contains(context.Location))
				yield break;

			var field = RemoveBackingStoreAction.GetBackingField(context, property);
			if (field == null)
				yield break;
			var resolvedType = ReflectionHelper.ParseReflectionName ("System.EventHandler").Resolve (context.Compilation);
			if (resolvedType == null)
				yield break;
			var type = (TypeDeclaration)property.Parent;

			yield return new CodeAction(context.TranslateString("Create changed event"), script => {
				var eventDeclaration = CreateChangedEventDeclaration (context, property);
				var methodDeclaration = CreateEventInvocatorAction.CreateEventInvocator (context, type, eventDeclaration, eventDeclaration.Variables.First (), resolvedType.GetDelegateInvokeMethod (), false);
				var stmt = new ExpressionStatement (new InvocationExpression (
					new IdentifierExpression (methodDeclaration.Name),
					context.CreateShortType("System", "EventArgs").Member("Empty")
				));
				script.InsertWithCursor(
					context.TranslateString("Create event invocator"),
					Script.InsertPosition.After,
					new AstNode[] { eventDeclaration, methodDeclaration }
				).ContinueScript(delegate {
					script.InsertBefore (property.Setter.Body.RBraceToken, stmt);
					script.FormatText (stmt);
				});
			}, property.NameToken);
		}

		EventDeclaration CreateChangedEventDeclaration (RefactoringContext context, PropertyDeclaration propertyDeclaration)
		{
			return new EventDeclaration {
				Modifiers = propertyDeclaration.HasModifier (Modifiers.Static) ? Modifiers.Public | Modifiers.Static : Modifiers.Public,
				ReturnType = context.CreateShortType("System", "EventHandler"),
				Variables = {
					new VariableInitializer {
						Name = propertyDeclaration.Name + "Changed"
					}
				}
			};
		}
	}
}

