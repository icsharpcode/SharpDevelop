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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding
{
	/// <summary>
	/// Description of ExtendSelection.
	/// </summary>
	public class CodeManipulation : IDisposable
	{
		enum MoveStatementDirection { Up, Down };
		
		ITextEditor editor;
		
		public static readonly RoutedCommand ExtendSelectionCommand = new RoutedCommand(
			"ExtendSelection", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.W, ModifierKeys.Control) }
		);
		
		public static readonly RoutedCommand ShrinkSelectionCommand = new RoutedCommand(
			"ShrinkSelection", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.W, ModifierKeys.Control | ModifierKeys.Shift) }
		);
		
		public static readonly RoutedCommand MoveStatementUpCommand = new RoutedCommand(
			"MoveStatementUp", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.Up, ModifierKeys.Alt) }
		);
		
		public static readonly RoutedCommand MoveStatementDownCommand = new RoutedCommand(
			"MoveStatementDown", typeof(CodeManipulation),
			new InputGestureCollection { new KeyGesture(Key.Down, ModifierKeys.Alt) }
		);
		
		IEnumerable<CommandBinding> bindings;
		
		public CodeManipulation(ITextEditor editor)
		{
			this.editor = editor;
			this.editor.SelectionChanged += CodeManipulationSelectionChanged;
			
			TextArea area = (TextArea)editor.GetService(typeof(TextArea));
			
			bindings = new List<CommandBinding> {
				new CommandBinding(ExtendSelectionCommand, ExtendSelectionExecuted),
				new CommandBinding(ShrinkSelectionCommand, ShrinkSelectionExecuted),
				new CommandBinding(MoveStatementUpCommand, MoveStatementUpExecuted),
				new CommandBinding(MoveStatementDownCommand, MoveStatementDownExecuted)
			};
			
			area.DefaultInputHandler.CommandBindings.AddRange(bindings);
		}
		
		bool internalSelectionChange;
		
		void ExtendSelectionExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			internalSelectionChange = true;
			try {
				ExtendSelection();
			} finally {
				internalSelectionChange = false;
			}
		}
		
		void ShrinkSelectionExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			internalSelectionChange = true;
			try {
				ShrinkSelection();
			} finally {
				internalSelectionChange = false;
			}
		}
		
		void MoveStatementUpExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			MoveStatementUp();
		}
		
		void MoveStatementDownExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			MoveStatementDown();
		}
		
		readonly Stack<ISegment> previousSelections = new Stack<ISegment>();

		class Selection
		{
			public TextLocation Start { get; set; }
			public TextLocation End { get; set; }
		}
		
		void CodeManipulationSelectionChanged(object sender, EventArgs e)
		{
			if (!internalSelectionChange)
				previousSelections.Clear();
		}
		
		public void Dispose()
		{
			this.editor.SelectionChanged -= CodeManipulationSelectionChanged;
			TextArea area = (TextArea)editor.GetService(typeof(TextArea));
			
			foreach (var b in bindings)
				area.DefaultInputHandler.CommandBindings.Remove(b);
		}
		
		public void MoveStatementUp()
		{
			MoveStatement(editor, MoveStatementDirection.Up);
			editor.ClearSelection();
		}
		
		public void MoveStatementDown()
		{
			MoveStatement(editor, MoveStatementDirection.Down);
			editor.ClearSelection();
		}
		
		// move selection - find outermost node in selection, swap selection with closest child of its parent to the selection
		void MoveStatement(ITextEditor editor, MoveStatementDirection direction)
		{
			IList<AstNode> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null)	return;
			
			// Find the Statement or Definition containing caret -> Extend selection to Statement or Definition
			
			AstNode currentStatement;
			Selection statementSelection = ExtendSelection(editor, parsedCU, commentsBlankLines, out currentStatement, new Type[] {
			                                               	typeof(Statement),
			                                               	typeof(EntityDeclaration) });
			if (currentStatement == null)
				return;
			statementSelection = TryExtendSelectionToComments(editor.Document, statementSelection, commentsBlankLines);
			// Take its sibling
			if (currentStatement.Parent == null)
				return;
			var siblings = currentStatement.Parent.Children.Where(c => (c.Role.GetType() == currentStatement.Role.GetType())).ToList();
			int currentStatementStartPos = siblings.IndexOf(currentStatement);
			int currentStatementEndPos = currentStatementStartPos;
			
			AstNode swapStartSibling = null;
			AstNode swapEndSibling = null;
			
			// Expand selection to full line, if there is more than one statement in it
			AstNode currentSelectionStartNode = currentStatement;
			while ((currentSelectionStartNode.PrevSibling != null)
			       && !(currentSelectionStartNode.PrevSibling is NewLineNode)
			       && (currentSelectionStartNode.Parent == currentSelectionStartNode.PrevSibling.Parent)) {
				currentSelectionStartNode = currentSelectionStartNode.PrevSibling;
				if (currentSelectionStartNode.EndLocation.Line >= statementSelection.Start.Line) {
					statementSelection.Start = currentSelectionStartNode.StartLocation;
					if (!(currentSelectionStartNode is Comment))
						currentStatementStartPos--;
				} else {
					// This node won't belong to current selection, so go back to next element
					currentSelectionStartNode = currentSelectionStartNode.NextSibling;
					break;
				}
			}
			AstNode currentSelectionEndNode = currentStatement;
			while ((currentSelectionEndNode.NextSibling != null)
			       && !(currentSelectionEndNode.NextSibling is NewLineNode)
			       && (currentSelectionEndNode.Parent == currentSelectionEndNode.NextSibling.Parent)) {
				currentSelectionEndNode = currentSelectionEndNode.NextSibling;
				if (currentSelectionEndNode.StartLocation.Line <= statementSelection.End.Line) {
					statementSelection.End = currentSelectionEndNode.EndLocation;
					if (!(currentSelectionEndNode is Comment))
						currentStatementEndPos++;
				} else {
					// This node won't belong to current selection, so go back to next element
					currentSelectionEndNode = currentSelectionEndNode.NextSibling;
					break;
				}
			}
			
			int swapIndex = 0;
			if (direction == MoveStatementDirection.Down) {
				swapIndex = currentStatementEndPos + 1;
			} else {
				swapIndex = currentStatementStartPos - 1;
			}
			Func<AstNode, bool> isAllowedGrandParentNode =
				(n => (n is IfElseStatement) || (n is ForStatement) || (n is ForeachStatement) || (n is WhileStatement) || (n is DoWhileStatement));
			if (swapIndex < 0) {
				// This is the 1st statement in block, so swap it with beginning of the block to get it outside of it
				var parentNode = currentStatement.Parent as BlockStatement;
				if (parentNode != null) {
					var grandParentNode = parentNode.Parent;
					if ((grandParentNode != null) && isAllowedGrandParentNode(grandParentNode)) {
						// Swap with head of grandparent statement
						swapStartSibling = grandParentNode;
						swapEndSibling = ((BlockStatement) parentNode).LBraceToken;
					}
				}
			} else if (swapIndex >= siblings.Count) {
				// This is the last statement in block, so swap it with block end to get the statement outside of it
				var parentNode = currentStatement.Parent as BlockStatement;
				if (parentNode != null) {
					var grandParentNode = parentNode.Parent;
					if ((grandParentNode != null) && isAllowedGrandParentNode(grandParentNode)) {
						// Swap with rest of grandparent control statement
						swapStartSibling = ((BlockStatement) parentNode).RBraceToken;
						swapEndSibling = grandParentNode;
					}
				}
			} else {
				// In the middle of current block
				swapStartSibling = siblings[swapIndex];
				swapEndSibling = swapStartSibling;
				
				// Special handling for swap nodes containing blocks: Move current statement into it
				if (swapStartSibling is IfElseStatement) {
					var ifElseStatement = swapStartSibling as IfElseStatement;
					if (direction == MoveStatementDirection.Up) {
						BlockStatement swappedIfElseBlock = ifElseStatement.FalseStatement as BlockStatement;
						if (swappedIfElseBlock == null)
							swappedIfElseBlock = ifElseStatement.TrueStatement as BlockStatement;
						if (swappedIfElseBlock != null) {
							swapStartSibling = swappedIfElseBlock.RBraceToken;
						}
					} else {
						BlockStatement swappedIfElseBlock = ifElseStatement.TrueStatement as BlockStatement;
						if (swappedIfElseBlock == null)
							swappedIfElseBlock = ifElseStatement.TrueStatement as BlockStatement;
						if (swappedIfElseBlock != null) {
							swapEndSibling = swappedIfElseBlock.LBraceToken;
						}
					}
				} else {
					BlockStatement innerBlockStatement = GetInnerBlockOfControlNode(swapStartSibling);
					if (innerBlockStatement != null) {
						if (direction == MoveStatementDirection.Up) {
							swapStartSibling = innerBlockStatement.RBraceToken;
						} else {
							swapEndSibling = innerBlockStatement.LBraceToken;
						}
					}
				}
			}

			if ((swapStartSibling == null) || (swapEndSibling == null))
				return;
			
			Selection swapSiblingSelection = ExtendSelectionToComments(editor.Document, swapStartSibling.StartLocation, swapEndSibling.EndLocation, commentsBlankLines);
			if (swapSiblingSelection == null)
				swapSiblingSelection = new Selection() { Start = swapStartSibling.StartLocation, End = swapEndSibling.EndLocation };
			
			// Expand swapSiblingSelection, too, if there are > 1 statements in line
			if (direction == MoveStatementDirection.Up) {
				AstNode tempNode = swapStartSibling;
				while ((tempNode.PrevSibling != null) && !(tempNode.PrevSibling is NewLineNode)) {
					tempNode = tempNode.PrevSibling;
					if (tempNode.EndLocation.Line >= swapSiblingSelection.Start.Line) {
						swapSiblingSelection.Start = tempNode.StartLocation;
					} else {
						break;
					}
				}
			} else {
				AstNode tempNode = swapEndSibling;
				while ((tempNode.NextSibling != null) && !(tempNode.NextSibling is NewLineNode)) {
					tempNode = tempNode.NextSibling;
					if (tempNode.StartLocation.Line <= swapSiblingSelection.End.Line) {
						swapSiblingSelection.End = tempNode.EndLocation;
					} else {
						break;
					}
				}
			}
			
			// Preserve the indentation of moved statement
			if (statementSelection.Start.Column > swapSiblingSelection.Start.Column) {
				statementSelection = new Selection {
					Start = new TextLocation(statementSelection.Start.Line, swapSiblingSelection.Start.Column),
					End = statementSelection.End
				};
			} else if (statementSelection.Start.Column < swapSiblingSelection.Start.Column) {
				swapSiblingSelection = new Selection {
					Start = new TextLocation(swapSiblingSelection.Start.Line, statementSelection.Start.Column),
					End = swapSiblingSelection.End
				};
			}
			
			// Swap them
			string currentNodeText = editor.Document.GetText(statementSelection.Start, statementSelection.End);
			SwapText(editor.Document, statementSelection.Start, statementSelection.End, swapSiblingSelection.Start, swapSiblingSelection.End);
			// Move caret to the start of moved statement
			TextLocation upperLocation = new TextLocation[] {statementSelection.Start, swapSiblingSelection.Start}.Min();
			if (direction == MoveStatementDirection.Up)
				editor.Caret.Location = upperLocation;
			else {
				// look where current statement ended up because it is hard to calculate it correctly
				int currentMovedOffset = editor.Document.Text.IndexOf(currentNodeText, editor.Document.GetOffset(upperLocation));
				editor.Caret.Offset = currentMovedOffset;
			}
		}
		
		BlockStatement GetInnerBlockOfControlNode(AstNode node)
		{
			if (node is ForStatement) {
				return ((ForStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is ForeachStatement) {
				return ((ForeachStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is WhileStatement) {
				return ((WhileStatement) node).EmbeddedStatement as BlockStatement;
			}
			if (node is DoWhileStatement) {
				return ((DoWhileStatement) node).EmbeddedStatement as BlockStatement;
			}
			
			return null;
		}
		
		Selection TryExtendSelectionToComments(IDocument document, Selection selection, IList<AstNode> commentsBlankLines)
		{
			var extendedToComments = ExtendSelectionToComments(document, selection, commentsBlankLines);
			if (extendedToComments != null)
				return extendedToComments;
			return selection;
		}
		
		public void ExtendSelection()
		{
			AstNode selectedNode = null;
			IList<AstNode> commentsBlankLines;
			var parsedCU = ParseDocument(editor, out commentsBlankLines);
			if (parsedCU == null)	return;
			
			ISegment oldSelection = new TextSegment { StartOffset = editor.SelectionStart, Length = editor.SelectionLength };
			Selection extendedSelection = ExtendSelection(editor, parsedCU, commentsBlankLines, out selectedNode, new Type[] { typeof(AstNode) });	// any node type
			
			SelectText(extendedSelection, editor);
			
			if (previousSelections.Count == 0 || !(previousSelections.Peek().Offset == oldSelection.Offset && previousSelections.Peek().EndOffset == oldSelection.EndOffset)) {
				previousSelections.Push(oldSelection);
				LoggingService.Debug("pushed: " + oldSelection);
			} else {
				LoggingService.Debug("not accepted: " + oldSelection);
			}
		}
		
		public void ShrinkSelection()
		{
			if (previousSelections.Count < 1)
				return;
			var selection = previousSelections.Pop();
			editor.Select(selection.Offset, selection.Length);
			LoggingService.Debug("popped: " + selection);
		}
		
		// could work to extend selection to set of adjacent statements separated by blank lines
		Selection ExtendSelection(ITextEditor editor, SyntaxTree parsedCU, IList<AstNode> commentsBlankLines, out AstNode selectedResultNode, Type[] interestingNodeTypes)
		{
			selectedResultNode = null;
			
			var selectionStart = editor.Document.GetLocation(editor.SelectionStart);
			var selectionEnd = editor.Document.GetLocation(editor.SelectionStart + editor.SelectionLength);
			
			AstNode currentNode = parsedCU.GetNodeContaining(selectionStart, selectionEnd);
			if (currentNode == null) return null;
			if (!IsNodeTypeInteresting(currentNode, interestingNodeTypes)) {
				// ignore uninteresting nodes in the AST
				currentNode = GetInterestingParent(currentNode, interestingNodeTypes);
			}
			if (currentNode == null) return null;
			selectedResultNode = currentNode;

			// whole node already selected -> expand selection
			if (currentNode.StartLocation == selectionStart && currentNode.EndLocation == selectionEnd) {
				
				bool extendToComments = false;
				if (IsNodeTypeInteresting(currentNode, interestingNodeTypes)) {
					// if interesting node already selected, we can try to also add comments
					var selectionExtendedToComments = ExtendSelectionToComments(editor.Document, selectionStart, selectionEnd, commentsBlankLines);
					if (selectionExtendedToComments != null) {
						// Can be extended to comments -> extend
						selectionStart = selectionExtendedToComments.Start;
						selectionEnd = selectionExtendedToComments.End;
						extendToComments = true;
					}
				}
				if (!extendToComments) {
					var parent = GetInterestingParent(currentNode, interestingNodeTypes);
					// it can happen that parent region exactly matches child region - in this case we need to advance even to the next parent
					// bc otherwise the selection would never move
					while (parent != null && parent.StartLocation == selectionStart && parent.EndLocation == selectionEnd) {
						parent = GetInterestingParent(parent, interestingNodeTypes);
					}
					if (parent == null) {
						return new Selection { Start = selectionStart, End = selectionEnd };
					}
					// Select the parent
					var extendedSelectionStart = parent.StartLocation;
					var extendedLocationEnd = parent.EndLocation;
					selectedResultNode = parent;
					
					// if the extended selection would contain blank lines, extend the selection only to the blank lines/comments on both sides (use siblings)
					//   if the selection contains blank lines or comments on both sides, dont do this
					// var blankLines = commentsBlankLines.Where(s => s is BlankLine).Cast<BlankLine>().ToList();
					//if (SelectionContainsBlankLines(extendedSelectionStart, extendedLocationEnd, blankLines)) {
					if (false) { // blank line separators - implement later
						
					} else {
						selectionStart = extendedSelectionStart;
						selectionEnd = extendedLocationEnd;
					}
				}
			} else {
				// select current node
				selectionStart = currentNode.StartLocation;
				selectionEnd = currentNode.EndLocation;
				selectedResultNode = currentNode;
			}
			return new Selection { Start = selectionStart, End = selectionEnd };
		}
		
		Selection ExtendSelectionToComments(IDocument document, Selection selection, IList<AstNode> commentsBlankLines)
		{
			if (selection == null)
				throw new ArgumentNullException("selection");
			return ExtendSelectionToComments(document, selection.Start, selection.End, commentsBlankLines);
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, or behind selection on the same line, add it to selection.
		/// </summary>
		Selection ExtendSelectionToComments(IDocument document, TextLocation selectionStart, TextLocation selectionEnd, IList<AstNode> commentsBlankLines)
		{
			var comments = commentsBlankLines.Where(s => s is Comment).Cast<Comment>();
			// add "var i = 5; // comments" comments
			Selection extendedSelection = ExtendSelectionToEndOfLineComments(document, selectionStart, selectionEnd, comments);
			if (extendedSelection != null) {
				return extendedSelection;
			} else {
				// if end-line comments already included, add comments on separate lines preceding selection
				return ExtendSelectionToSeparateComments(document, selectionStart, selectionEnd, comments);
			}
		}
		
		/// <summary>
		/// If there is a comment block behind selection on the same line ("var i = 5; // comment"), add it to selection.
		/// </summary>
		Selection ExtendSelectionToEndOfLineComments(IDocument document, TextLocation selectionStart, TextLocation selectionEnd, IEnumerable<AstNode> commentsBlankLines)
		{
			var lineComment = commentsBlankLines.FirstOrDefault(c => c.StartLocation.Line == selectionEnd.Line && c.StartLocation >= selectionEnd);
			if (lineComment == null) {
				return null;
			}
//			bool isWholeLineSelected = IsWhitespaceBetween(document, new TextLocation(selectionStart.Line, 1), selectionStart);
//			if (!isWholeLineSelected) {
//				// whole line must be selected before we add the comment
//				return null;
//			}
			int endPos = document.GetOffset(lineComment.EndLocation);
			return new Selection { Start = selectionStart, End = document.GetLocation(endPos) };
		}
		
		/// <summary>
		/// If there is a comment block immediately before selection, add it to selection.
		/// </summary>
		Selection ExtendSelectionToSeparateComments(IDocument document, TextLocation selectionStart, TextLocation selectionEnd, IEnumerable<AstNode> commentsBlankLines)
		{
			var comments = commentsBlankLines.Where(c => (c is Comment) && ((Comment) c).StartsLine).ToList();
			int commentIndex = comments.FindIndex(c => c.EndLocation <= selectionStart && IsWhitespaceBetween(document, c.EndLocation, selectionStart));
			if (commentIndex < 0) {
				return null;
			}
			var extendedSelection = new Selection { Start = selectionStart, End = selectionEnd };
			// start at the selection and keep adding comments upwards as long as they are separated only by whitespace
			while (commentIndex >= 0 && IsWhitespaceBetween(document, comments[commentIndex].EndLocation, extendedSelection.Start)) {
				var comment = comments[commentIndex];
				// Include the "//, /*, ///" since they are not included by the parser
				extendedSelection.Start = ExtendLeft(comment.StartLocation, document, "///", "/*", "//") ;
				commentIndex--;
			}
			return extendedSelection;
		}
		
		/// <summary>
		/// Searches for parent of interesting type. Skips uninteresting parents.
		/// </summary>
		AstNode GetInterestingParent(AstNode node, Type[] interestingNodeTypes)
		{
			var parent = node.Parent;
			while(parent != null) {
				if (IsNodeTypeInteresting(parent, interestingNodeTypes))
					return parent;
				parent = parent.Parent;
			}
			return null;
		}
		
		bool IsNodeTypeInteresting(INode node, Type[] interestingNodeTypes)
		{
			Type nodeType = node.GetType();
			return interestingNodeTypes.Any(interestingType => interestingType.IsAssignableFrom(nodeType) || (nodeType == interestingType));
		}
		
		bool SelectionContainsBlankLines(TextLocation selectionStart, TextLocation selectionEnd, List<AstNode> blankLines)
		{
			return blankLines.Exists(b => b.StartLocation >= selectionStart && b.EndLocation <= selectionEnd);
		}
		
		bool IsWhitespaceBetween(IDocument document, TextLocation startPos, TextLocation endPos)
		{
			int startOffset = document.GetOffset(startPos);
			int endOffset = document.GetOffset(endPos);
			if (startOffset > endOffset) {
				throw new ArgumentException("Invalid range for (startPos, endPos)");
			}
			return string.IsNullOrWhiteSpace(document.GetText(startOffset, endOffset - startOffset));
		}
		
		/// <summary>
		/// If the text at startPos is preceded by any of the prefixes, moves the start backwards to include one prefix
		/// (the rightmost one).
		/// </summary>
		TextLocation ExtendLeft(TextLocation startPos, IDocument document, params String[] prefixes)
		{
			int startOffset = document.GetOffset(startPos);
			foreach (string prefix in prefixes) {
				if (startOffset < prefix.Length) continue;
				string realPrefix = document.GetText(startOffset - prefix.Length, prefix.Length);
				if (realPrefix == prefix) {
					return document.GetLocation(startOffset - prefix.Length);
				}
			}
			// no prefixes -> do not extend
			return startPos;
		}
		
		// could depend just on IDocument
		SyntaxTree ParseDocument(ITextEditor editor, out IList<AstNode> parsedSpecials)
		{
			parsedSpecials = null;
			CompilerSettings compilerSettings = new CompilerSettings();
			var parser = new CSharpParser();
			if (parser == null) return null;
			var syntaxTree = parser.Parse(editor.Document.CreateReader());
			if (syntaxTree == null) return null;
			parsedSpecials = new List<AstNode>(syntaxTree.Descendants.OfType<Comment>());
			return syntaxTree;
		}
		
		/// <summary>
		/// Swaps 2 ranges of text in a document.
		/// </summary>
		void SwapText(IDocument document, TextLocation start1, TextLocation end1, TextLocation start2, TextLocation end2)
		{
			if (start1 > start2) {
				TextLocation sw;
				sw = start1; start1 = start2; start2 = sw;
				sw = end1; end1 = end2; end2 = sw;
			}
			if (end1 >= start2)
				throw new InvalidOperationException("Cannot swap overlaping segments");
			int offset1 = document.GetOffset(start1);
			int len1 = document.GetOffset(end1) - offset1;
			int offset2 = document.GetOffset(start2);
			int len2 = document.GetOffset(end2) - offset2;
			
			string text1 = document.GetText(offset1, len1);
			string text2 = document.GetText(offset2, len2);
			
			using (var undoGroup = document.OpenUndoGroup()) {
				document.Replace(offset2, len2, text1);
				document.Replace(offset1, len1, text2);
			}
		}
		
		void SelectText(Selection selection, ITextEditor editor)
		{
			if (selection == null)
				return;
			int startOffset, endOffset;
			try {
				startOffset = editor.Document.GetOffset(selection.Start);
				endOffset = editor.Document.GetOffset(selection.End);
			} catch (ArgumentOutOfRangeException) {
				return;
			}
			editor.Select(startOffset, endOffset - startOffset);
		}
	}
}
