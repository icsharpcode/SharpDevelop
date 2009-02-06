// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace CSharpBinding
{
	public class EventHandlerCompletitionDataProvider : AbstractCompletionDataProvider
	{
		string expression;
		ResolveResult resolveResult;
		IReturnType resolvedReturnType;
		IClass resolvedClass;
		
		public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
			this.resolvedReturnType = resolveResult.ResolvedType;
			this.resolvedClass = resolvedReturnType.GetUnderlyingClass();
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// </summary>
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			List<ICompletionData> completionData = new List<ICompletionData>();
			
			// delegate {  }
			completionData.Add(new DelegateCompletionData("delegate {  };", 3,
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
				completionData.Add(new DelegateCompletionData(anonMethodWithParametersBuilder.ToString(), 3,
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
				completionData.Add(new NewEventHandlerCompletionData(
					newHandlerTextBuilder.ToString(),
					2+newHandlerName.Length,
					newHandlerName.Length,
					"new " + eventHandlerFullyQualifiedTypeName + 
					"(" + newHandlerName + StringParser.Parse(")\n${res:CSharpBinding.GenerateNewHandlerInstructions}\n")
					+ CodeCompletionData.ConvertDocumentation(resolvedClass.Documentation),
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
							completionData.Add(new CodeCompletionData(method));
						}
					}
				}
			}
			return completionData.ToArray();
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
		
		private class DelegateCompletionData : DefaultCompletionData
		{
			int cursorOffset;
			
			public DelegateCompletionData(string text, int cursorOffset, string documentation)
				: base(text, StringParser.Parse(documentation), ClassBrowserIconService.DelegateIndex)
			{
				this.cursorOffset = cursorOffset;
			}
			
			public override bool InsertAction(TextArea textArea, char ch)
			{
				bool r = base.InsertAction(textArea, ch);
				textArea.Caret.Column -= cursorOffset;
				return r;
			}
		}
		
		private class NewEventHandlerCompletionData : DefaultCompletionData
		{
			int selectionBeginOffset;
			int selectionLength;
			TextArea textArea;
			ResolveResult resolveResult;
			string newHandlerCode;
			
			public NewEventHandlerCompletionData(string text, int selectionBeginOffset, int selectionLength, string documentation, ResolveResult resolveResult, string newHandlerCode)
				: base(text, StringParser.Parse(documentation), ClassBrowserIconService.DelegateIndex)
			{
				this.selectionBeginOffset = selectionBeginOffset;
				this.selectionLength = selectionLength;
				this.resolveResult = resolveResult;
				this.newHandlerCode = newHandlerCode;
			}
			
			public override bool InsertAction(TextArea textArea, char ch)
			{
				bool r = base.InsertAction(textArea, ch);
				
				// select suggested name
				textArea.Caret.Column -= this.selectionBeginOffset;
				int selectBegin = textArea.Caret.Offset;
				textArea.SelectionManager.SetSelection(
					textArea.Document.OffsetToPosition(selectBegin),
					textArea.Document.OffsetToPosition(selectBegin + this.selectionLength));

				// TODO: refactor ToolTip architecture to allow for showing a tooltip relative to the current caret position so that we can show our "press TAB to create this method" text as a text-based tooltip
				
				// TODO: skip the auto-insert step if the method already exists, or change behavior so that it moves the caret inside the existing method.

				// attatch our keydown filter to catch the next character pressed
				textArea.PreviewKeyDown += new PreviewKeyDownEventHandler(NewEventHandlerPreviewKeyDown);

				// save a reference to the relevant textArea so that we can remove our keydown filter after the next keystroke
				this.textArea = textArea;
				
				return r;
			}
			
			public void NewEventHandlerPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
			{
				if (e.KeyCode == Keys.Tab
				    || e.KeyCode == Keys.Enter
				    || e.KeyCode == Keys.Return) {

					textArea.BeginUpdate();
					textArea.Document.UndoStack.StartUndoGroup();
					
					textArea.SelectionManager.ClearSelection();
					
					// is there a better way to calculate the optimal insertion point?
					DomRegion region = resolveResult.CallingMember.BodyRegion;
					textArea.Caret.Line = region.EndLine - 1;
					textArea.Caret.Column = region.EndColumn;
					
					textArea.InsertString(this.newHandlerCode);

					textArea.Document.FormattingStrategy.IndentLines(textArea, region.EndLine, textArea.Caret.Line);
					
					textArea.Caret.Line -= 1;

					LineSegment segment = textArea.Document.GetLineSegment(textArea.Caret.Line);

					textArea.SelectionManager.SetSelection(
						textArea.Document.OffsetToPosition(TextUtilities.GetFirstNonWSChar(textArea.Document, segment.Offset)),
						textArea.Document.OffsetToPosition(segment.Offset+segment.Length));

					textArea.Document.UndoStack.EndUndoGroup();
					textArea.EndUpdate();
					
					textArea.DoProcessDialogKey += new DialogKeyProcessor(IgnoreNextDialogKey);
				}
				
				// detatch our keydown filter to return to the normal processing state
				this.textArea.PreviewKeyDown -= new PreviewKeyDownEventHandler(NewEventHandlerPreviewKeyDown);
				
			}
			
			bool IgnoreNextDialogKey(Keys keyData) {
				this.textArea.DoProcessDialogKey -= new DialogKeyProcessor(IgnoreNextDialogKey);
				return true; // yes, we've processed this key
			}
		}
	}
}







