// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Snippets
{
	/// <summary>
	/// Represents the context of a snippet insertion.
	/// </summary>
	public class InsertionContext
	{
		/// <summary>
		/// Creates a new InsertionContext instance.
		/// </summary>
		public InsertionContext(TextArea textArea, int insertionPosition)
		{
			if (textArea == null)
				throw new ArgumentNullException("textArea");
			this.TextArea = textArea;
			this.Document = textArea.Document;
			this.InsertionPosition = insertionPosition;
			
			DocumentLine startLine = this.Document.GetLineByOffset(insertionPosition);
			ISegment indentation = TextUtilities.GetWhitespaceAfter(this.Document, startLine.Offset);
			this.Indentation = Document.GetText(indentation.Offset, Math.Min(indentation.EndOffset, insertionPosition) - indentation.Offset);
			
			this.LineTerminator = NewLineFinder.GetNewLineFromDocument(this.Document, startLine.LineNumber);
		}
		
		/// <summary>
		/// Gets the text area.
		/// </summary>
		public TextArea TextArea { get; private set; }
		
		/// <summary>
		/// Gets the text document.
		/// </summary>
		public TextDocument Document { get; private set; }
		
		/// <summary>
		/// Gets the indentation at the insertion position.
		/// </summary>
		public string Indentation { get; private set; }
		
		/// <summary>
		/// Gets the line terminator at the insertion position.
		/// </summary>
		public string LineTerminator { get; private set; }
		
		/// <summary>
		/// Gets/Sets the insertion position.
		/// </summary>
		public int InsertionPosition { get; set; }
		
		/// <summary>
		/// Inserts text at the insertion position and advances the insertion position.
		/// </summary>
		public void InsertText(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			int textOffset = 0;
			SimpleSegment segment;
			while ((segment = NewLineFinder.NextNewLine(text, textOffset)) != SimpleSegment.Invalid) {
				string insertString = text.Substring(textOffset, segment.Offset - textOffset)
					+ this.LineTerminator + this.Indentation;
				this.Document.Insert(InsertionPosition, insertString);
				this.InsertionPosition += insertString.Length;
				textOffset = segment.EndOffset;
			}
			string remainingInsertString = text.Substring(textOffset);
			this.Document.Insert(InsertionPosition, remainingInsertString);
			this.InsertionPosition += remainingInsertString.Length;
		}
		
		Dictionary<SnippetElement, IActiveElement> elementMap = new Dictionary<SnippetElement, IActiveElement>();
		List<IActiveElement> registeredElements = new List<IActiveElement>();
		
		/// <summary>
		/// Registers an active element. Elements should be registered during insertion and will be called back
		/// when insertion has completed.
		/// </summary>
		/// <param name="owner">The snippet element that created the active element.</param>
		/// <param name="element">The active element.</param>
		public void RegisterActiveElement(SnippetElement owner, IActiveElement element)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			if (element == null)
				throw new ArgumentNullException("element");
			elementMap.Add(owner, element);
			registeredElements.Add(element);
		}
		
		/// <summary>
		/// Returns the active element belonging to the specified snippet element, or null if no such active element is found.
		/// </summary>
		public IActiveElement GetActiveElement(SnippetElement owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			IActiveElement element;
			if (elementMap.TryGetValue(owner, out element))
				return element;
			else
				return null;
		}
		
		bool insertionCompleted;
		
		/// <summary>
		/// Calls the <see cref="IActiveElement.OnInsertionCompleted"/> method on all registered active elements
		/// and raises the <see cref="InsertionCompleted"/> event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
		                                                 Justification="There is an event and this method is raising it.")]
		public void RaiseInsertionCompleted()
		{
			foreach (IActiveElement element in registeredElements) {
				element.OnInsertionCompleted();
			}
			if (InsertionCompleted != null)
				InsertionCompleted(this, EventArgs.Empty);
			insertionCompleted = true;
			if (registeredElements.Count == 0) {
				// deactivate immediately if there are no interactive elements
				Deactivate();
			} else {
				// register Escape key handler
				TextArea.KeyDown += TextArea_KeyDown;
			}
		}

		void TextArea_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				Deactivate();
				e.Handled = true;
			}
		}
		
		/// <summary>
		/// Occurs when the all snippet elements have been inserted.
		/// </summary>
		public event EventHandler InsertionCompleted;
		
		/// <summary>
		/// Calls the <see cref="IActiveElement.Deactivate"/> method on all registered active elements.
		/// </summary>
		public void Deactivate()
		{
			if (!insertionCompleted)
				throw new InvalidOperationException("Cannot call Deactivate() until RaiseInsertionCompleted() has finished.");
			TextArea.KeyDown -= TextArea_KeyDown;
			foreach (IActiveElement element in registeredElements) {
				element.Deactivate();
			}
			if (Deactivated != null)
				Deactivated(this, EventArgs.Empty);
		}
		
		/// <summary>
		/// Occurs when the interactive mode is deactivated.
		/// </summary>
		public event EventHandler Deactivated;
	}
}
