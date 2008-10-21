using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	public interface ITextContainer
	{
		string Text { get; set; }
	}

	public class TextAction : IUndoAction
	{
		public TextAction(ITextContainer textContainer,
			int offset, string oldText, string newText)
		{
			this.textContainer = textContainer;
			this.offset = offset;
			this.oldText = oldText;
			this.newText = newText;
		}

		ITextContainer textContainer;
		int offset;
		string oldText;
		string newText;

		public IEnumerable<DesignItem> AffectedItems
		{
			get { yield break; }
		}

		public string Title
		{
			get { return "Text Editing"; }
		}

		public void Do()
		{
			var newContent = textContainer.Text.Remove(offset, oldText.Length).Insert(offset, newText);			
			textContainer.Text = newContent;
		}

		public void Undo()
		{
			var newContent = textContainer.Text.Remove(offset, newText.Length).Insert(offset, oldText);
			textContainer.Text = newContent;
		}
	}
}
