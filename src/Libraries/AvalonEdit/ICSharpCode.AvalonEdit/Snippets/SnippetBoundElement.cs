// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// An element that binds to a <see cref="SnippetReplaceableTextElement"/> and displays the same text.
	/// </summary>
	[Serializable]
	public class SnippetBoundElement : SnippetElement
	{
		SnippetReplaceableTextElement targetElement;
		
		/// <summary>
		/// Gets/Sets the target element.
		/// </summary>
		public SnippetReplaceableTextElement TargetElement {
			get { return targetElement; }
			set {
				CheckBeforeMutation();
				targetElement = value;
			}
		}
		
		/// <summary>
		/// Converts the text before copying it.
		/// </summary>
		public virtual string ConvertText(string input)
		{
			return input;
		}
		
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			if (targetElement != null) {
				int start = context.InsertionPosition;
				string inputText = targetElement.Text;
				if (inputText != null) {
					context.InsertText(ConvertText(inputText));
				}
				int end = context.InsertionPosition;
				AnchorSegment segment = new AnchorSegment(context.Document, start, end - start);
				context.RegisterActiveElement(this, new BoundActiveElement(context, targetElement, this, segment));
			}
		}
	}
	
	sealed class BoundActiveElement : IActiveElement
	{
		InsertionContext context;
		SnippetReplaceableTextElement targetSnippetElement;
		SnippetBoundElement boundElement;
		IReplaceableActiveElement targetElement;
		AnchorSegment segment;
		
		public BoundActiveElement(InsertionContext context, SnippetReplaceableTextElement targetSnippetElement, SnippetBoundElement boundElement, AnchorSegment segment)
		{
			this.context = context;
			this.targetSnippetElement = targetSnippetElement;
			this.boundElement = boundElement;
			this.segment = segment;
		}
		
		public void OnInsertionCompleted()
		{
			targetElement = context.GetActiveElement(targetSnippetElement) as IReplaceableActiveElement;
			if (targetElement != null) {
				targetElement.TextChanged += targetElement_TextChanged;
			}
		}
		
		void targetElement_TextChanged(object sender, EventArgs e)
		{
			int offset = segment.Offset;
			int length = segment.Length;
			string text = boundElement.ConvertText(targetElement.Text);
			context.Document.Replace(offset, length, text);
			if (length == 0) {
				// replacing an empty anchor segment with text won't enlarge it, so we have to recreate it
				segment = new AnchorSegment(context.Document,  offset, text.Length);
			}
		}
		
		public void Deactivate()
		{
		}
	}
}
