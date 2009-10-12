// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Text element that is supposed to be replaced by the user.
	/// Will register an <see cref="IReplaceableActiveElement"/>.
	/// </summary>
	[Serializable]
	public class SnippetReplaceableTextElement : SnippetTextElement
	{
		/// <inheritdoc/>
		public override void Insert(InsertionContext context)
		{
			int start = context.InsertionPosition;
			base.Insert(context);
			int end = context.InsertionPosition;
			context.RegisterActiveElement(this, new ReplaceableActiveElement(context, start, end));
		}
	}
	
	/// <summary>
	/// Interface for active element registered by <see cref="SnippetReplaceableTextElement"/>.
	/// </summary>
	public interface IReplaceableActiveElement : IActiveElement
	{
		/// <summary>
		/// Gets the current text inside the element.
		/// </summary>
		string Text { get; }
		
		/// <summary>
		/// Occurs when the text inside the element changes.
		/// </summary>
		event EventHandler TextChanged;
	}
	
	sealed class ReplaceableActiveElement : IReplaceableActiveElement, IWeakEventListener
	{
		readonly InsertionContext context;
		readonly int startOffset, endOffset;
		TextAnchor start, end;
		
		public ReplaceableActiveElement(InsertionContext context, int startOffset, int endOffset)
		{
			this.context = context;
			this.startOffset = startOffset;
			this.endOffset = endOffset;
		}
		
		void AnchorDeleted(object sender, EventArgs e)
		{
			context.Deactivate();
		}
		
		public void OnInsertionCompleted()
		{
			// anchors must be created in OnInsertionCompleted because they should move only
			// due to user insertions, not due to insertions of other snippet parts
			start = context.Document.CreateAnchor(startOffset);
			start.MovementType = AnchorMovementType.BeforeInsertion;
			end = context.Document.CreateAnchor(endOffset);
			end.MovementType = AnchorMovementType.AfterInsertion;
			start.Deleted += AnchorDeleted;
			end.Deleted += AnchorDeleted;
			
			// Be careful with references from the document to the editing/snippet layer - use weak events
			// to prevent memory leaks when text areas get dropped from the UI while the snippet is active.
			// The InsertionContext will keep us alive as long as the snippet is in interactive mode.
			TextDocumentWeakEventManager.TextChanged.AddListener(context.Document, this);
			
			this.Text = GetText();
		}
		
		public void Deactivate()
		{
			TextDocumentWeakEventManager.TextChanged.RemoveListener(context.Document, this);
		}
		
		public string Text { get; private set; }
		
		string GetText()
		{
			if (start.IsDeleted || end.IsDeleted)
				return string.Empty;
			else
				return context.Document.GetText(start.Offset, Math.Max(0, end.Offset - start.Offset));
		}
		
		public event EventHandler TextChanged;
		
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			if (managerType == typeof(TextDocumentWeakEventManager.TextChanged)) {
				string newText = GetText();
				if (this.Text != newText) {
					this.Text = newText;
					if (TextChanged != null)
						TextChanged(this, e);
				}
				return true;
			}
			return false;
		}
	}
}
