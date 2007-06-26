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
		IClass resolvedClass;
		
		public EventHandlerCompletitionDataProvider(string expression, ResolveResult resolveResult)
		{
			this.expression = expression;
			this.resolveResult = resolveResult;
			this.resolvedClass = resolveResult.ResolvedType.GetUnderlyingClass();
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
			ambience.ConversionFlags = ConversionFlags.ShowParameterNames;
			IMethod invoke = resolvedClass.SearchMember("Invoke", LanguageProperties.CSharp) as IMethod;
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
				MemberResolveResult mrr = resolveResult as MemberResolveResult;
				DefaultEvent eventMember = (mrr != null ? mrr.ResolvedMember as DefaultEvent : null);
				
				// build the new handler name
				StringBuilder newHandlerNameBuilder = new StringBuilder(callingClass.Name).Append("_").Append(eventMember != null ? eventMember.Name : "eventName");

				// build the completion text
				StringBuilder newHandlerTextBuilder = new StringBuilder("new ").Append(resolveResult.ResolvedType.Name).Append("(");
				newHandlerTextBuilder.Append(newHandlerNameBuilder.ToString()).Append(");");

				// build the optional new method text
				StringBuilder newHandlerCodeBuilder = new StringBuilder();
				newHandlerCodeBuilder.AppendLine().AppendLine();
				newHandlerCodeBuilder.Append(ambience.Convert(invoke.ReturnType)).Append(" ").Append(newHandlerNameBuilder.ToString());
				newHandlerCodeBuilder.Append("(").Append(parameterString.ToString()).AppendLine(")");
				newHandlerCodeBuilder.AppendLine("{");
				newHandlerCodeBuilder.Append("throw new NotImplementedException(\"").Append(ResourceService.GetString("CSharpBinding.MethodIsNotImplemented")).AppendLine("\");");
				newHandlerCodeBuilder.Append("}");
				completionData.Add(new NewEventHandlerCompletionData(
					newHandlerTextBuilder.ToString(),
				    2+newHandlerNameBuilder.Length,
				    newHandlerNameBuilder.Length,
				    "delegate " + resolvedClass.FullyQualifiedName + "(" + newHandlerNameBuilder.ToString() + ")" +"\n"+ResourceService.GetString("CSharpBinding.GenerateNewHandlerInstructions") + "\n" + CodeCompletionData.GetDocumentation(resolvedClass.Documentation),
				    resolveResult,
				    newHandlerCodeBuilder.ToString()
				   ));
				
				IClass eventReturnType = invoke.ReturnType.GetUnderlyingClass();
				IClass[] eventParameters = new IClass[invoke.Parameters.Count];
				for (int i = 0; i < eventParameters.Length; i++) {
					eventParameters[i] = invoke.Parameters[i].ReturnType.GetUnderlyingClass();
					if (eventParameters[i] == null) {
						eventReturnType = null;
						break;
					}
				}
				if (callingClass != null && eventReturnType != null) {
					bool inStatic = false;
					if (resolveResult.CallingMember != null)
						inStatic = resolveResult.CallingMember.IsStatic;
					foreach (IMethod method in callingClass.DefaultReturnType.GetMethods()) {
						if (inStatic && !method.IsStatic)
							continue;
						if (!method.IsAccessible(callingClass, true))
							continue;
						if (method.Parameters.Count != invoke.Parameters.Count)
							continue;
						// check return type compatibility:
						IClass c2 = method.ReturnType.GetUnderlyingClass();
						if (c2 == null || !c2.IsTypeInInheritanceTree(eventReturnType))
							continue;
						bool ok = true;
						for (int i = 0; i < eventParameters.Length; i++) {
							c2 = method.Parameters[i].ReturnType.GetUnderlyingClass();
							if (c2 == null || !eventParameters[i].IsTypeInInheritanceTree(c2)) {
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
				
				// TODO: skip this step if the method already exists, or change behavior so that it moves the caret inside the existing method.
				// attatch our keydown filter to catch the next character pressed
				this.textArea = textArea;
				// TODO: refactor ToolTip architecture to allow for showing a tooltip relative to the current caret position so that we can show our "press TAB to create this method" text as a text-based tooltip
				textArea.PreviewKeyDown += new PreviewKeyDownEventHandler(NewEventHandlerPreviewKeyDown);
				
				return r;
			}
						
			public void NewEventHandlerPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
			{
				if (e.KeyCode == Keys.Tab
				    || e.KeyCode == Keys.Enter
				    || e.KeyCode == Keys.Return) {

					textArea.BeginUpdate();
					
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

