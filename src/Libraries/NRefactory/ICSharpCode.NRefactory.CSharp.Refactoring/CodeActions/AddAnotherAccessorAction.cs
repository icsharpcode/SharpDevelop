// 
// AddAnotherAccessor.cs
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
using System.Threading;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	/// <summary>
	/// Add another accessor to a property declaration that has only one.
	/// </summary>
	[ContextAction("Add another accessor", Description = "Adds second accessor to a property.")]
	public class AddAnotherAccessorAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var pdecl = context.GetNode<PropertyDeclaration> ();
			if (pdecl == null || !pdecl.Getter.IsNull && !pdecl.Setter.IsNull || !pdecl.NameToken.Contains(context.Location)) { 
				yield break;
			}

			var type = pdecl.Parent as TypeDeclaration;
			if (type != null && type.ClassType == ClassType.Interface) {
				yield break;
			}
			yield return new CodeAction (pdecl.Setter.IsNull ? context.TranslateString("Add setter") : context.TranslateString("Add getter"), script => {
				Statement accessorStatement = null;
			
				var accessor = new Accessor ();
				if (!pdecl.Getter.IsNull && !pdecl.Getter.Body.IsNull || !pdecl.Setter.IsNull && !pdecl.Setter.Body.IsNull) {
					accessorStatement = BuildAccessorStatement(context, pdecl);
					accessor.Body = new BlockStatement { accessorStatement };
				}

				accessor.Role = pdecl.Setter.IsNull ? PropertyDeclaration.SetterRole : PropertyDeclaration.GetterRole;

				if (pdecl.Setter.IsNull && !pdecl.Getter.IsNull) {
					script.InsertAfter(pdecl.Getter, accessor);
				} else if (pdecl.Getter.IsNull && !pdecl.Setter.IsNull) {
					script.InsertBefore(pdecl.Setter, accessor);
				} else {
					script.InsertBefore(pdecl.Getter, accessor);
				}
				script.FormatText(pdecl);
				if (accessorStatement != null)
					script.Select(accessorStatement);
			}, pdecl.NameToken);
		}
		
		static Statement BuildAccessorStatement (RefactoringContext context, PropertyDeclaration pdecl)
		{
			if (pdecl.Setter.IsNull && !pdecl.Getter.IsNull) {
				var field = RemoveBackingStoreAction.ScanGetter (context, pdecl);
				if (field != null && !field.IsReadOnly && !field.IsConst) 
					return new AssignmentExpression (new IdentifierExpression (field.Name), AssignmentOperatorType.Assign, new IdentifierExpression ("value"));
			}
			
			if (!pdecl.Setter.IsNull && pdecl.Getter.IsNull) {
				var field = RemoveBackingStoreAction.ScanSetter (context, pdecl);
				if (field != null) 
					return new ReturnStatement (new IdentifierExpression (field.Name));
			}
			
			return new ThrowStatement (new ObjectCreateExpression (context.CreateShortType ("System", "NotImplementedException")));
		}
	}
}
