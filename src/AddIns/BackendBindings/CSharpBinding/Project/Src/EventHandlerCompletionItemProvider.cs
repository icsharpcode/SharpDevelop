// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System.Windows.Input;

namespace CSharpBinding
{
	public class EventHandlerCompletionItemProvider : AbstractCompletionItemProvider
	{
		string expression;
		ResolveResult resolveResult;
		IReturnType resolvedReturnType;
		IClass resolvedClass;
		
		public EventHandlerCompletionItemProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
			this.resolvedReturnType = resolveResult.ResolvedType;
			this.resolvedClass = resolvedReturnType.GetUnderlyingClass();
		}
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			DefaultCompletionItemList result = new DefaultCompletionItemList();
			result.InsertSpace = true;
			
			// delegate {  }
			result.Items.Add(new DelegateCompletionItem("delegate {  };", 3,
			                                           "${res:CSharpBinding.InsertAnonymousMethod}"));

			CSharpAmbience ambience = new CSharpAmbience();
			// get eventHandler type name incl. type argument list
			ambience.ConversionFlags = ConversionFlags.ShowParameterNames | ConversionFlags.ShowTypeParameterList | ConversionFlags.UseFullyQualifiedTypeNames;
			string eventHandlerFullyQualifiedTypeName = ambience.Convert(resolvedReturnType);
			ambience.ConversionFlags = ConversionFlags.ShowParameterNames | ConversionFlags.ShowTypeParameterList;
			string eventHandlerTypeName = ambience.Convert(resolvedReturnType);
			
			// retrieve Invoke method from resolvedReturnType instead of resolvedClass to get a method where
			// type arguments are substituted.
			IMethod invoke = resolvedReturnType.GetMethods().Find(delegate(IMethod m) { return m.Name == "Invoke"; });
			StringBuilder parameterString = new StringBuilder();
			if (invoke != null) {
				
				// build the parameter string
				for (int i = 0; i < invoke.Parameters.Count; ++i) {
					if (i > 0) {
						parameterString.Append(", ");
					}
					
					parameterString.Append(ambience.Convert(invoke.Parameters[i]));
				}
				
				// delegate(object sender, EventArgs e) {  };
				StringBuilder anonMethodWithParametersBuilder =
					new StringBuilder("delegate(").Append(parameterString.ToString()).Append(") {  };");
				result.Items.Add(new DelegateCompletionItem(anonMethodWithParametersBuilder.ToString(), 3,
				                                            "${res:CSharpBinding.InsertAnonymousMethodWithParameters}"));

				// new EventHandler(ClassName_EventName);
				IClass callingClass = resolveResult.CallingClass;
				bool inStatic = false;
				if (resolveResult.CallingMember != null)
					inStatic = resolveResult.CallingMember.IsStatic;
				
				// ...build the new handler name...
				string newHandlerName = BuildHandlerName();
				if (newHandlerName == null) {
					MemberResolveResult mrr = resolveResult as MemberResolveResult;
					IEvent eventMember = (mrr != null ? mrr.ResolvedMember as IEvent : null);
					newHandlerName =
						((callingClass != null) ? callingClass.Name : "callingClass")
						+ "_"
						+ ((eventMember != null) ? eventMember.Name : "eventMember");
				}

				// ...build the completion text...
				StringBuilder newHandlerTextBuilder = new StringBuilder("new ").Append(eventHandlerTypeName).Append("(");
				newHandlerTextBuilder.Append(newHandlerName).Append(");");

				// ...build the optional new method text...
				StringBuilder newHandlerCodeBuilder = new StringBuilder();
				newHandlerCodeBuilder.AppendLine().AppendLine();
				if (inStatic)
					newHandlerCodeBuilder.Append("static ");
				newHandlerCodeBuilder.Append(ambience.Convert(invoke.ReturnType)).Append(" ").Append(newHandlerName);
				newHandlerCodeBuilder.Append("(").Append(parameterString.ToString()).AppendLine(")");
				newHandlerCodeBuilder.AppendLine("{");
				newHandlerCodeBuilder.AppendLine("throw new NotImplementedException();");
				newHandlerCodeBuilder.Append("}");

				// ...and add it to the completionData.
				result.Items.Add(new NewEventHandlerCompletionItem(
					newHandlerTextBuilder.ToString(),
					2+newHandlerName.Length,
					newHandlerName.Length,
					"new " + eventHandlerFullyQualifiedTypeName + 
					"(" + newHandlerName + StringParser.Parse(")\n${res:CSharpBinding.GenerateNewHandlerInstructions}\n")
					+ CodeCompletionItem.ConvertDocumentation(resolvedClass.Documentation),
					resolveResult,
					newHandlerCodeBuilder.ToString()
				));
				
				if (callingClass != null) {
					foreach (IMethod method in callingClass.DefaultReturnType.GetMethods()) {
						if (inStatic && !method.IsStatic)
							continue;
						if (!method.IsAccessible(callingClass, true))
							continue;
						if (method.Parameters.Count != invoke.Parameters.Count)
							continue;
						// check return type compatibility:
						if (!MemberLookupHelper.ConversionExists(method.ReturnType, invoke.ReturnType))
							continue;
						bool ok = true;
						for (int i = 0; i < invoke.Parameters.Count; i++) {
							if (!MemberLookupHelper.ConversionExists(invoke.Parameters[i].ReturnType, method.Parameters[i].ReturnType)) {
								ok = false;
								break;
							}
						}
						if (ok) {
							result.Items.Add(new CodeCompletionItem(method));
						}
					}
				}
			}
			result.SortItems();
			return result;
		}
		
		string BuildHandlerName()
		{
			if (expression != null)
				expression = expression.Trim();
			if (string.IsNullOrEmpty(expression))
				return null;
			if (!(char.IsLetter(expression[0]) || expression[0] == '_'))
				return null;
			StringBuilder handlerNameBuilder = new StringBuilder();
			for (int i = 0; i < expression.Length; i++) {
				if (char.IsLetterOrDigit(expression[i]) || expression[i] == '_') {
					handlerNameBuilder.Append(expression[i]);
				} else if (expression[i] == '.') {
					if (ICSharpCode.NRefactory.Parser.CSharp.Keywords.IsNonIdentifierKeyword(handlerNameBuilder.ToString())) {
						return null;
					}
					handlerNameBuilder.Append('_');
				} else {
					return null;
				}
			}
			return handlerNameBuilder.ToString();
		}
		
		sealed class DelegateCompletionItem : DefaultCompletionItem
		{
			int cursorOffset;
			
			public DelegateCompletionItem(string text, int cursorOffset, string documentation)
				: base(text)
			{
				this.cursorOffset = cursorOffset;
				this.Description = StringParser.Parse(documentation);
				this.Image = ClassBrowserIconService.Delegate;
			}
			
			public override void Complete(CompletionContext context)
			{
				base.Complete(context);
				context.Editor.Caret.Column -= cursorOffset;
			}
		}
		
		sealed class NewEventHandlerCompletionItem : DefaultCompletionItem
		{
			int selectionBeginOffset;
			int selectionLength;
			ResolveResult resolveResult;
			string newHandlerCode;
			
			ITextEditor editor;
			
			public NewEventHandlerCompletionItem(string text, int selectionBeginOffset, int selectionLength, string documentation, ResolveResult resolveResult, string newHandlerCode)
				: base(text)
			{
				this.selectionBeginOffset = selectionBeginOffset;
				this.selectionLength = selectionLength;
				this.resolveResult = resolveResult;
				this.newHandlerCode = newHandlerCode;
				
				this.Description = StringParser.Parse(documentation);
				this.Image = ClassBrowserIconService.Delegate;
			}
			
			public override void Complete(CompletionContext context)
			{
				base.Complete(context);
				
				// save a reference to the relevant textArea so that we can remove our event handlers after the next keystroke
				editor = context.Editor;
				// select suggested name
				editor.Caret.Column -= this.selectionBeginOffset;
				editor.Select(editor.Caret.Offset, this.selectionLength);

				// TODO: refactor ToolTip architecture to allow for showing a tooltip relative to the current caret position so that we can show our "press TAB to create this method" text as a text-based tooltip
				
				// TODO: skip the auto-insert step if the method already exists, or change behavior so that it moves the caret inside the existing method.

				// attach our keydown filter to catch the next character pressed
				editor.SelectionChanged += EditorSelectionChanged;
				editor.KeyPress += EditorKeyPress;
			}
			
			
			void RemoveEventHandlers()
			{
				if (editor != null) {
					editor.SelectionChanged -= EditorSelectionChanged;
					editor.KeyPress -= EditorKeyPress;
					editor = null;
				}
			}
			
			void EditorSelectionChanged(object sender, EventArgs e)
			{
				RemoveEventHandlers();
			}
			
			void EditorKeyPress(object sender, KeyEventArgs e)
			{
				if (e.Key == Key.Tab || e.Key == Key.Enter || e.Key == Key.Return) {
					using (editor.Document.OpenUndoGroup()) {
					
						// is there a better way to calculate the optimal insertion point?
						DomRegion region = resolveResult.CallingMember.BodyRegion;
						editor.Caret.Line = region.EndLine;
						editor.Caret.Column = region.EndColumn;
						
						editor.Document.Insert(editor.Caret.Offset, this.newHandlerCode);
	
						editor.Language.FormattingStrategy.IndentLines(editor, region.EndLine, editor.Caret.Line);
						
						IDocumentLine line = editor.Document.GetLine(editor.Caret.Line - 1);
						int indentationLength = DocumentUtilitites.GetWhitespaceAfter(editor.Document, line.Offset).Length;
						
						editor.Select(line.Offset + indentationLength, line.Length - indentationLength);
					}
					e.Handled = true;
				}
				// detatch our keydown filter to return to the normal processing state
				RemoveEventHandlers();
			}
		}
	}
}
