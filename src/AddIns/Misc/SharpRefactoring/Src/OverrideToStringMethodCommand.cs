// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

using System;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;
using SharpRefactoring.Gui;

namespace SharpRefactoring
{
	/// <summary>
	/// Description of OverrideToStringMethodCommand
	/// </summary>
	public class OverrideToStringMethodCommand : AbstractRefactoringCommand
	{
		protected override void Run(ITextEditor textEditor, RefactoringProvider provider)
		{
			IEditorUIService uiService = textEditor.GetService(typeof(IEditorUIService)) as IEditorUIService;
			
			if (uiService == null)
				return;
			
			ParseInformation parseInfo = ParserService.GetParseInformation(textEditor.FileName);
			
			if (parseInfo == null)
				return;
			
			CodeGenerator generator = parseInfo.CompilationUnit.Language.CodeGenerator;
			IClass current = parseInfo.CompilationUnit.GetInnermostClass(textEditor.Caret.Line, textEditor.Caret.Column);
			
			if (current == null)
				return;
			
			ITextAnchor anchor = textEditor.Document.CreateAnchor(textEditor.Caret.Offset);
			anchor.MovementType = AnchorMovementType.AfterInsertion;
			
			textEditor.Document.Insert(anchor.Offset, "public override ToString()\n{\n\t");
			
			ITextAnchor parameterListAnchor = textEditor.Document.CreateAnchor(anchor.Offset - ")\n{\n\t".Length);
			parameterListAnchor.SurviveDeletion = true;
			parameterListAnchor.MovementType = AnchorMovementType.AfterInsertion;
			
			textEditor.Document.Insert(anchor.Offset + 1, "\n\t\n}\n");
			
			InlineRefactorDialog dialog = new OverrideToStringMethodDialog(textEditor, anchor, parameterListAnchor, current.Fields);
			
			dialog.Element = uiService.CreateInlineUIElement(anchor, dialog);
		}
	}
}
