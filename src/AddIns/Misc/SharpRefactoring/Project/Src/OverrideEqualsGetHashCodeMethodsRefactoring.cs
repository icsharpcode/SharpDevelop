// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Refactoring;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	public class OverrideEqualsGetHashCodeMethodsRefactoring : ICompletionItemHandler
	{
		public void Insert(CompletionContext context, ICompletionItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			
			if (!(item is OverrideCompletionItem))
				throw new ArgumentException("item is not an OverrideCompletionItem");
			
			OverrideCompletionItem completionItem = item as OverrideCompletionItem;
			
			ITextEditor textEditor = context.Editor;
			
			IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			
			if (uiService == null)
				return;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			
			if (parseInfo == null)
				return;
			
			CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
			IClass current = parseInfo.CompilationUnit.GetInnermostClass(textEditor.Caret.Line, textEditor.Caret.Column);
			ClassFinder finder = new ClassFinder(current, textEditor.Caret.Line, textEditor.Caret.Column);
			
			if (current == null)
				return;
			
			ITextAnchor start = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
			start.MovementType = AnchorMovementType.BeforeInsertion;
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
			anchor.MovementType = AnchorMovementType.AfterInsertion;
			
			IAmbience ambience = parseInfo.CompilationUnit.Language.GetAmbience();
			MethodDeclaration member = (MethodDeclaration)generator.GetOverridingMethod(completionItem.Member, finder);
			
			string indent = DocumentUtilitites.GetWhitespaceBefore(textEditor.Document, textEditor.Caret.Offset);
			
			string codeForBaseCall = generator.GenerateCode(member.Body.Children.OfType<AbstractNode>().First(), "");
			
			string code = generator.GenerateCode(member, indent);
			
			int marker = code.IndexOf(codeForBaseCall);
			
			textEditor.Document.Insert(start.Offset, code.Substring(0, marker).TrimStart());
			
			ITextAnchor insertionPos = textEditor.Document.CreateAnchor(anchor.Offset);
			insertionPos.MovementType = AnchorMovementType.BeforeInsertion;
			
			AbstractInlineRefactorDialog dialog = new OverrideEqualsGetHashCodeMethodsDialog(textEditor, start, anchor, insertionPos, current, completionItem.Member as IMethod, codeForBaseCall.Trim());
			dialog.Element = uiService.CreateInlineUIElement(insertionPos, dialog);
			
			textEditor.Document.InsertNormalized(anchor.Offset, Environment.NewLine + code.Substring(marker + codeForBaseCall.Length));
		}
		
		public bool Handles(ICompletionItem item)
		{
			return item is OverrideCompletionItem && (item.Text == "GetHashCode()" || item.Text == "Equals(object obj)");
		}
	}
}
