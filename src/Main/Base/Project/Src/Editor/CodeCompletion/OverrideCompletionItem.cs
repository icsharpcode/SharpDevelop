// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	public sealed class OverrideCompletionItem : ICompletionItem
	{
		IMember member;
		
		public IMember Member {
			get { return member; }
		}
		
		string text;
		IImage image;
		
		static readonly List<ICompletionItemHandler> handlers = AddInTree.BuildItems<ICompletionItemHandler>("/SharpDevelop/ViewContent/TextEditor/OverrideCompletionHandler", null, false);
		
		public OverrideCompletionItem(IMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			this.member = member;
			
			this.text = GetName(member, ConversionFlags.ShowParameterList);
			this.image = ClassBrowserIconService.GetIcon(member);
		}
		
		public string Text {
			get { return text; }
		}
		
		public IImage Image {
			get { return image; }
		}
		
		public string Description {
			get {
				return "override " + GetName(member, ConversionFlags.ShowReturnType
				                             | ConversionFlags.ShowParameterList
				                             | ConversionFlags.ShowAccessibility)
					+ "\n\n" + CodeCompletionItem.ConvertDocumentation(member.Documentation);
			}
		}
		
		double ICompletionItem.Priority { get { return 0; } }
		
		static string GetName(IMember member, ConversionFlags flags)
		{
			IAmbience ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = flags | ConversionFlags.ShowParameterNames | ConversionFlags.ShowTypeParameterList;
			return ambience.Convert(member);
		}
		
		public void Complete(CompletionContext context)
		{
			ITextEditor editor = context.Editor;
			ClassFinder classFinder = new ClassFinder(ParserService.GetParseInformation(editor.FileName),
			                                          editor.Caret.Line, editor.Caret.Column);
			int caretPosition = editor.Caret.Offset;
			IDocumentLine line = editor.Document.GetLine(editor.Caret.Line);
			string lineText = editor.Document.GetText(line.Offset, caretPosition - line.Offset);
			foreach (char c in lineText) {
				if (!char.IsWhiteSpace(c) && !char.IsLetterOrDigit(c)) {
					editor.Document.Replace(context.StartOffset, context.Length, this.Text);
					context.EndOffset = context.StartOffset + this.Text.Length;
					return;
				}
			}
			
			string indentation = lineText.Substring(0, lineText.Length - lineText.TrimStart().Length);

			editor.Document.Remove(line.Offset, caretPosition - line.Offset);
			
			foreach (ICompletionItemHandler handler in handlers) {
				if (handler.Handles(this)) {
					editor.Document.Insert(line.Offset, indentation);
					handler.Insert(context, this);
					return;
				}
			}
			
			CodeGenerator codeGen = ParserService.CurrentProjectContent.Language.CodeGenerator;
			
			string text = codeGen.GenerateCode(codeGen.GetOverridingMethod(member, classFinder), indentation);
			text = text.TrimEnd(); // remove newline from end
			
			editor.Document.Insert(line.Offset, text);
			
			int endPos = line.Offset + text.Length;
			line = editor.Document.GetLineForOffset(endPos);
			editor.JumpTo(line.LineNumber, endPos - line.Offset + 1);
		}
	}
}
