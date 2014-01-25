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
