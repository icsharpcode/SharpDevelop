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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Parser;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Item for 'override' completion.
	/// </summary>
	class OverrideCompletionData : EntityCompletionData
	{
		protected readonly int declarationBegin;
		protected readonly CSharpResolver contextAtCaret;
		
		public OverrideCompletionData(int declarationBegin, IMember m, CSharpResolver contextAtCaret)
			: base(m)
		{
			this.declarationBegin = declarationBegin;
			this.contextAtCaret = contextAtCaret;
			var ambience = new CSharpAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames;
			this.CompletionText = ambience.ConvertSymbol(m);
		}
		
		public override void Complete(CompletionContext context)
		{
			if (declarationBegin > context.StartOffset) {
				base.Complete(context);
				return;
			}
			
			TypeSystemAstBuilder b = new TypeSystemAstBuilder(contextAtCaret);
			b.ShowTypeParameterConstraints = false;
			b.GenerateBody = true;
			
			var entityDeclaration = b.ConvertEntity(this.Entity);
			entityDeclaration.Modifiers &= ~(Modifiers.Virtual | Modifiers.Abstract);
			entityDeclaration.Modifiers |= Modifiers.Override;
			
			if (!this.Entity.IsAbstract) {
				// modify body to call the base method
				if (this.Entity.SymbolKind == SymbolKind.Method) {
					var baseCall = new BaseReferenceExpression().Invoke(this.Entity.Name, ParametersToExpressions(this.Entity));
					var body = entityDeclaration.GetChildByRole(Roles.Body);
					body.Statements.Clear();
					if (((IMethod)this.Entity).ReturnType.IsKnownType(KnownTypeCode.Void))
						body.Statements.Add(new ExpressionStatement(baseCall));
					else
						body.Statements.Add(new ReturnStatement(baseCall));
				} else if (this.Entity.SymbolKind == SymbolKind.Indexer || this.Entity.SymbolKind == SymbolKind.Property) {
					Expression baseCall;
					if (this.Entity.SymbolKind == SymbolKind.Indexer)
						baseCall = new BaseReferenceExpression().Indexer(ParametersToExpressions(this.Entity));
					else
						baseCall = new BaseReferenceExpression().Member(this.Entity.Name);
					var getterBody = entityDeclaration.GetChildByRole(PropertyDeclaration.GetterRole).Body;
					if (!getterBody.IsNull) {
						getterBody.Statements.Clear();
						getterBody.Add(new ReturnStatement(baseCall.Clone()));
					}
					var setterBody = entityDeclaration.GetChildByRole(PropertyDeclaration.SetterRole).Body;
					if (!setterBody.IsNull) {
						setterBody.Statements.Clear();
						setterBody.Add(new AssignmentExpression(baseCall.Clone(), new IdentifierExpression("value")));
					}
				}
			}
			
			var document = context.Editor.Document;
			StringWriter w = new StringWriter();
			var formattingOptions = CSharpFormattingPolicies.Instance.GetProjectOptions(contextAtCaret.Compilation.GetProject());
			var segmentDict = SegmentTrackingOutputFormatter.WriteNode(
				w, entityDeclaration, formattingOptions.OptionsContainer.GetEffectiveOptions(), context.Editor.Options);
			
			using (document.OpenUndoGroup()) {
				string newText = w.ToString().TrimEnd();
				document.Replace(declarationBegin, context.EndOffset - declarationBegin, newText);
				var throwStatement = entityDeclaration.Descendants.FirstOrDefault(n => n is ThrowStatement);
				if (throwStatement != null) {
					var segment = segmentDict[throwStatement];
					context.Editor.Select(declarationBegin + segment.Offset, segment.Length);
				}
				CSharpFormatterHelper.Format(context.Editor, declarationBegin, newText.Length, formattingOptions.OptionsContainer);
			}
		}
		
		IEnumerable<Expression> ParametersToExpressions(IEntity entity)
		{
			foreach (var p in ((IParameterizedMember)entity).Parameters) {
				if (p.IsRef || p.IsOut)
					yield return new DirectionExpression(p.IsOut ? FieldDirection.Out : FieldDirection.Ref, new IdentifierExpression(p.Name));
				else
					yield return new IdentifierExpression(p.Name);
			}
		}
	}
}
