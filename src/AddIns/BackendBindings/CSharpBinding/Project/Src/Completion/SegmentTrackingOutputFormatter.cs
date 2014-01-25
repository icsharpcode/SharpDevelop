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
using System.IO;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Editor;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Output formatter that creates a dictionary from AST nodes to segments in the output text.
	/// </summary>
	public class SegmentTrackingOutputFormatter : TextWriterTokenWriter
	{
		Dictionary<AstNode, ISegment> segments = new Dictionary<AstNode, ISegment>();
		Stack<int> startOffsets = new Stack<int>();
		readonly StringWriter stringWriter;
		
		public IReadOnlyDictionary<AstNode, ISegment> Segments {
			get { return segments; }
		}
		
		public SegmentTrackingOutputFormatter (StringWriter stringWriter)
			: base(stringWriter)
		{
			this.stringWriter = stringWriter;
		}
		
		public static IReadOnlyDictionary<AstNode, ISegment> WriteNode(StringWriter writer, AstNode node, CSharpFormattingOptions policy, ITextEditorOptions editorOptions)
		{
			var formatter = new SegmentTrackingOutputFormatter(writer);
			formatter.IndentationString = editorOptions.IndentationString;
			var visitor = new CSharpOutputVisitor(formatter, policy);
			node.AcceptVisitor(visitor);
			return formatter.Segments;
		}
		
		public override void StartNode (AstNode node)
		{
			base.StartNode (node);
			startOffsets.Push(stringWriter.GetStringBuilder ().Length);
		}
		
		public override void EndNode (AstNode node)
		{
			int startOffset = startOffsets.Pop();
			StringBuilder b = stringWriter.GetStringBuilder();
			int endOffset = b.Length;
			while (endOffset > 0 && b[endOffset - 1] == '\r' || b[endOffset - 1] == '\n')
				endOffset--;
			segments.Add(node, new TextSegment { StartOffset = startOffset, EndOffset = endOffset });
			base.EndNode (node);
		}
	}
}
