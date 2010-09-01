// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Siegfried Pammer"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	public sealed class SnippetAnchorElement : SnippetElement
	{
		string textToInsert = "";
		
		public string Name { get; private set; }
		
		public SnippetAnchorElement(string name)
		{
			this.Name = name;
		}
		
		public override void Insert(InsertionContext context)
		{
			int start = context.InsertionPosition;
			context.InsertText("");
			int end = context.InsertionPosition;
			AnchorSegment segment = new AnchorSegment(context.Document, start, end - start);
			context.RegisterActiveElement(this, new AnchorSnippetElement(segment, "", Name, context));
		}
	}
	
	public sealed class AnchorSnippetElement : IActiveElement
	{
		public bool IsEditable {
			get { return false; }
		}
		
		AnchorSegment segment;
		InsertionContext context;
		
		public ISegment Segment {
			get { return segment; }
		}
		
		public AnchorSnippetElement(AnchorSegment segment, string text, string name, InsertionContext context)
		{
			this.segment = segment;
			this.context = context;
			this.Text = text;
			this.Name = name;
		}
		
		public string Text {
			get { return context.Document.GetText(segment); }
			set {
				int offset = segment.Offset;
				int length = segment.Length;
				context.Document.Replace(offset, length, value);
				if (length == 0) {
					// replacing an empty anchor segment with text won't enlarge it, so we have to recreate it
					segment = new AnchorSegment(context.Document, offset, value.Length);
				}
			}
		}
		
		public string Name { get; private set; }
		
		public void OnInsertionCompleted()
		{
		}
		
		public void Deactivate()
		{
		}
	}
}
