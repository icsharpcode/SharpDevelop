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
	public class CheckAssignmentNotNull : ContextAction
	{
		public override string Title {
			get { return "Check for not null"; }
		}
		
		string caretMarker = "<<>>";
		
		public override bool IsAvailable(EditorContext context)
		{
			var cache = context.GetCached<CheckAssignmentCache>();
			return cache.IsActionAvailable;
		}
		
		public override void Execute(EditorContext context)
		{
			var cache = context.GetCached<CheckAssignmentCache>();
			
			var ifStatement = GenerateAstToInsert(cache.VariableName);
			
			var editor = context.Editor;
			string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, editor.Document.GetLineStartOffset(cache.ElementRegion.GetStart()));
			string code = cache.CodeGenerator.GenerateCode(ifStatement, indent);
			int insertOffset = editor.Document.GetLineEndOffset(cache.ElementRegion.GetEnd());
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
