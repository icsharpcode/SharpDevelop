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
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Item for 'partial' completion.
	/// </summary>
	class PartialCompletionData : EntityCompletionData
	{
		protected readonly int declarationBegin;
		protected readonly CSharpResolver contextAtCaret;
		
		public PartialCompletionData(int declarationBegin, IMember m, CSharpResolver contextAtCaret)
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
			b.GenerateBody = true;
			
			var entityDeclaration = b.ConvertEntity(this.Entity);
			entityDeclaration.Modifiers &= ~Modifiers.VisibilityMask; // remove visiblity
			entityDeclaration.Modifiers |= Modifiers.Partial;
			
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
