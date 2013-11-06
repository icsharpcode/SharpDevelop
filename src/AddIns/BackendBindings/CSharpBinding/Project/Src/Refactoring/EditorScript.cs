// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Refactoring change script.
	/// </summary>
	sealed class EditorScript : DocumentScript
	{
		readonly ITextEditor editor;
		readonly TextSegmentCollection<TextSegment> textSegmentCollection;
		readonly SDRefactoringContext context;
		
		public EditorScript(ITextEditor editor, SDRefactoringContext context, CSharpFormattingOptions formattingOptions) 
			: base(editor.Document, formattingOptions, context.TextEditorOptions)
		{
			this.editor = editor;
			this.context = context;
			this.textSegmentCollection = new TextSegmentCollection<TextSegment>((TextDocument)editor.Document);
		}
		
		protected override ISegment CreateTrackedSegment(int offset, int length)
		{
			var segment = new TextSegment();
			segment.StartOffset = offset;
			segment.Length = length;
			textSegmentCollection.Add(segment);
			return segment;
		}
		
		/*public override void FormatText(AstNode node)
		{
			var parseInfo = SD.ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
			if (parseInfo != null) {
				//var startLocation = editor.Document.GetLocation(offset);
				//var endLocation = editor.Document.GetLocation(offset + length);
				//var node = parseInfo.SyntaxTree.GetNodeContaining(startLocation, endLocation);
				var formatter = new AstFormattingVisitor(FormattingOptionsFactory.CreateSharpDevelop(), editor.Document, context.TextEditorOptions);
				parseInfo.SyntaxTree.AcceptVisitor(formatter);
				var segment = GetSegment(node);
				formatter.ApplyChanges(segment.Offset, segment.Length);
			} else {
				base.FormatText(node);
			}
		}*/
		
		public override void Select(AstNode node)
		{
			var segment = GetSegment(node);
			int startOffset = segment.Offset;
			int endOffset = segment.EndOffset;
			// If the area to select includes a newline (e.g. node is a statement),
			// exclude that newline from the selection.
			if (endOffset > startOffset && editor.Document.GetLineByOffset(endOffset).Offset == endOffset) {
				endOffset = editor.Document.GetLineByOffset(endOffset).PreviousLine.EndOffset;
			}
			editor.Select(startOffset, endOffset - startOffset);
		}
		
		static readonly Task completedTask = Task.FromResult<object>(null);
		
		public override Task Link(params AstNode[] nodes)
		{
			// TODO
			return completedTask;
		}
		
		/*
		public override Task InsertWithCursor(string operation, InsertPosition defaultPosition, IEnumerable<AstNode> nodes)
		{
			AstNode contextNode = context.GetNode();
			if (contextNode == null)
				return completedTask;
			var resolver = context.GetResolverStateBefore(contextNode);
			return InsertWithCursor(operation, resolver.CurrentTypeDefinition, nodes);
		}
		
		public override Task InsertWithCursor(string operation, ITypeDefinition parentType, IEnumerable<AstNode> nodes)
		{
			if (parentType == null)
				return completedTask;
			var currentPart = parentType.Parts.FirstOrDefault(p => p.UnresolvedFile != null && string.Equals(p.UnresolvedFile.FileName, editor.FileName, StringComparison.OrdinalIgnoreCase));
			if (currentPart != null) {
				var insertionPoints = InsertionPoint.GetInsertionPoints(editor.Document, currentPart);
				if (insertionPoints.Count > 0) {
					int indentLevel = GetIndentLevelAt(editor.Document.GetOffset(insertionPoints[0].Location));
					foreach (var node in nodes.Reverse()) {
						var output = OutputNode(indentLevel, node);
						insertionPoints[0].Insert(editor.Document, output.Text);
					}
				}
			}
			return completedTask;
		}
		*/
		
		public override void Dispose()
		{
			base.Dispose();
			// refresh parse information so that the issue can disappear immediately
			SD.ParserService.ParseAsync(editor.FileName, editor.Document).FireAndForget();
		}
	}
}
