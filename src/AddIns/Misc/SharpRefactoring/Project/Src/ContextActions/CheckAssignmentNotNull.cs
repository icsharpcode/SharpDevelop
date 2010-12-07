// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			var doc = editor.Document;
			
			using (var undoGroup = editor.Document.OpenUndoGroup()) {
				editor.InsertCodeAfter(cache.Element, ifStatement);
				// set caret to the position of the special marker
				var caretPos = doc.Text.IndexOf(caretMarker, doc.GetLineEndOffset(cache.Element.EndLocation));
				editor.Caret.Offset = caretPos;
				editor.Document.RemoveRestOfLine(caretPos);
			}
		}
		
		AbstractNode GenerateAstToInsert(string variableName)
		{
			var block = new BlockStatement();
			// mark the place where to put caret
			block.AddChild(new ExpressionStatement(new IdentifierExpression(caretMarker)));
			return new IfElseStatement(
				new BinaryOperatorExpression(new IdentifierExpression(variableName), BinaryOperatorType.InEquality, new PrimitiveExpression(null)),
				block);
		}
	}
}
