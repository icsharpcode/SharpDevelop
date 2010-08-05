// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Konicek" email="martin.konicek@gmail.com"/>
//     <version>$Revision: $</version>
// </file>
using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.ContextActions
{
	/// <summary>
	/// Description of CheckAssignmentNotNull.
	/// </summary>
	public class CheckAssignmentNotNull : CheckAssignmentAction
	{
		public override string Title {
			get { return "Check for not null"; }
		}
		
		string caretMarker = "<<>>";
		
		public override void Execute(EditorContext context)
		{
			var ifStatement = GenerateAstToInsert(this.VariableName);
			
			var editor = context.Editor;
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, editor.Document.GetLineStartOffset(this.ElementRegion.GetStart()));
			string code = this.CodeGenerator.GenerateCode(ifStatement, indent);
			int insertOffset = editor.Document.GetLineEndOffset(this.ElementRegion.GetEnd());
			using (var undoGroup = editor.Document.OpenUndoGroup()) {
				editor.Document.Insert(insertOffset, code);
				var caretPos = editor.Document.Text.IndexOf(caretMarker, insertOffset);
				editor.Caret.Offset = caretPos;
				editor.Document.RemoveRestOfLine(caretPos);
			}
		}
		
		AbstractNode GenerateAstToInsert(string variableName)
		{
			var block = new BlockStatement();
			block.AddChild(new ExpressionStatement(new IdentifierExpression(caretMarker)));
			return new IfElseStatement(
				new BinaryOperatorExpression(new IdentifierExpression(variableName), BinaryOperatorType.InEquality, new PrimitiveExpression(null)),
				block);
		}
	}
}
