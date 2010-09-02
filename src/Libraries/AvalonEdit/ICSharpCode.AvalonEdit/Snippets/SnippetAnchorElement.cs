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
	/// <summary>
	/// Creates a named anchor that can be accessed by other SnippetElements.
	/// </summary>
	public sealed class SnippetAnchorElement : SnippetElement
	{
		/// <summary>
		/// Gets or sets the name of the anchor.
		/// </summary>
		public string Name { get; private set; }
		
		/// <summary>
		/// Creates a SnippetAnchorElement with the supplied name.
		/// </summary>
		public SnippetAnchorElement(string name)
		{
			this.Name = name;
		}
		
		/// <inheritdoc />
		public override void Insert(InsertionContext context)
		{
			int start = context.InsertionPosition;
			AnchorSegment segment = new AnchorSegment(context.Document, start, 0);
			context.RegisterActiveElement(this, new AnchorElement(segment, "", Name, context));
		}
	}
	
	/// <summary>
	/// AnchorElement created by SnippetAnchorElement.
	/// </summary>
	public sealed class AnchorElement : IActiveElement
	{
		/// <inheritdoc />
		public bool IsEditable {
			get { return false; }
		}
		
		AnchorSegment segment;
		InsertionContext context;
		
		/// <inheritdoc />
		public ISegment Segment {
			get { return segment; }
		}
		
		/// <summary>
		/// Creates a new AnchorElement.
		/// </summary>
		public AnchorElement(AnchorSegment segment, string text, string name, InsertionContext context)
		{
			this.segment = segment;
			this.context = context;
			this.Text = text;
			this.Name = name;
		}
		
		/// <summary>
		/// Gets or sets the text at the anchor.
		/// </summary>
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
		
		/// <summary>
		/// Gets or sets the name of the anchor.
		/// </summary>
		public string Name { get; private set; }
		
		/// <inheritdoc />
		public void OnInsertionCompleted()
		{
		}
		
		/// <inheritdoc />
		public void Deactivate(SnippetEventArgs e)
		{
		}
	}
}
