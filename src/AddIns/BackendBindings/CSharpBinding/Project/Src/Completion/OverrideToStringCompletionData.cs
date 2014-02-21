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
using System.Threading;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using CSharpBinding.FormattingStrategy;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using CSharpBinding.Refactoring;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Item for 'override' completion of "ToString()" methods.
	/// </summary>
	class OverrideToStringCompletionData : OverrideCompletionData
	{
		public OverrideToStringCompletionData(int declarationBegin, IMember m, CSharpResolver contextAtCaret)
			: base(declarationBegin, m, contextAtCaret)
		{
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
			
			var body = entityDeclaration.GetChildByRole(Roles.Body);
			Statement baseCallStatement = body.Children.OfType<Statement>().FirstOrDefault();
			
			if (!this.Entity.IsAbstract) {
				// modify body to call the base method
				if (this.Entity.SymbolKind == SymbolKind.Method) {
					var baseCall = new BaseReferenceExpression().Invoke(this.Entity.Name, new Expression[] { });
					if (((IMethod)this.Entity).ReturnType.IsKnownType(KnownTypeCode.Void))
						baseCallStatement = new ExpressionStatement(baseCall);
					else
						baseCallStatement = new ReturnStatement(baseCall);
					
					// Clear body of inserted method
					entityDeclaration.GetChildByRole(Roles.Body).Statements.Clear();
				}
			}
			
			var document = context.Editor.Document;
			StringWriter w = new StringWriter();
			var formattingOptions = CSharpFormattingOptionsPersistence.GetProjectOptions(contextAtCaret.Compilation.GetProject());
			var segmentDict = SegmentTrackingOutputFormatter.WriteNode(
				w, entityDeclaration, formattingOptions.OptionsContainer.GetEffectiveOptions(), context.Editor.Options);
			
			using (document.OpenUndoGroup()) {
				InsertionContext insertionContext = new InsertionContext(context.Editor.GetService(typeof(TextArea)) as TextArea, declarationBegin);
				insertionContext.InsertionPosition = context.Editor.Caret.Offset;
				
				string newText = w.ToString().TrimEnd();
				document.Replace(declarationBegin, context.EndOffset - declarationBegin, newText);
				var throwStatement = entityDeclaration.Descendants.FirstOrDefault(n => n is ThrowStatement);
				if (throwStatement != null) {
					var segment = segmentDict[throwStatement];
					context.Editor.Select(declarationBegin + segment.Offset, segment.Length);
				}
				CSharpFormatterHelper.Format(context.Editor, declarationBegin, newText.Length, formattingOptions.OptionsContainer);
				
				var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
				var typeResolveContext = refactoringContext.GetTypeResolveContext();
				if (typeResolveContext == null) {
					return;
				}
				var resolvedCurrent = typeResolveContext.CurrentTypeDefinition;
				var entities = FindFieldsAndProperties(resolvedCurrent).ToList();
				if (entities.Any()) {
					IEditorUIService uiService = context.Editor.GetService(typeof(IEditorUIService)) as IEditorUIService;
					
					ITextAnchor endAnchor = context.Editor.Document.CreateAnchor(context.Editor.Caret.Offset);
					endAnchor.MovementType = AnchorMovementType.AfterInsertion;
					
					ITextAnchor startAnchor = context.Editor.Document.CreateAnchor(context.Editor.Caret.Offset);
					startAnchor.MovementType = AnchorMovementType.BeforeInsertion;
					
					ITextAnchor insertionPos = context.Editor.Document.CreateAnchor(endAnchor.Offset);
					insertionPos.MovementType = AnchorMovementType.BeforeInsertion;

					AbstractInlineRefactorDialog dialog = new OverrideToStringMethodDialog(insertionContext, context.Editor, insertionPos, entities, baseCallStatement);
					dialog.Element = uiService.CreateInlineUIElement(insertionPos, dialog);
					
					insertionContext.RegisterActiveElement(new InlineRefactorSnippetElement(cxt => null, ""), dialog);
				} else {
					if (baseCallStatement != null) {
						// Add default base call
						MethodDeclaration insertedOverrideMethod = refactoringContext.GetNode().PrevSibling as MethodDeclaration;
						if (insertedOverrideMethod == null)
						{
							// We are not inside of a method declaration
							return;
						}
						using (Script script = refactoringContext.StartScript()) {
							script.AddTo(insertedOverrideMethod.Body, baseCallStatement);
						}
					}
				}
				
				insertionContext.RaiseInsertionCompleted(EventArgs.Empty);
			}
		}
		
		IEnumerable<PropertyOrFieldWrapper> FindFieldsAndProperties(IType sourceType)
		{
			int i = 0;
			
			foreach (var f in sourceType.GetFields().Where(field => !field.IsConst
			                                               && field.IsStatic == sourceType.GetDefinition().IsStatic
			                                               && field.DeclaringType.FullName == sourceType.FullName
			                                               && field.ReturnType != null)) {
				yield return new PropertyOrFieldWrapper(f) { Index = i };
				i++;
			}
			
			foreach (var p in sourceType.GetProperties().Where(prop => prop.CanGet && !prop.IsIndexer
			                                                   && prop.IsAutoImplemented()
			                                                   && prop.IsStatic == sourceType.GetDefinition().IsStatic
			                                                   && prop.DeclaringType.FullName == sourceType.FullName
			                                                   && prop.ReturnType != null)) {
				yield return new PropertyOrFieldWrapper(p) { Index = i };
				i++;
			}
		}
	}
}
