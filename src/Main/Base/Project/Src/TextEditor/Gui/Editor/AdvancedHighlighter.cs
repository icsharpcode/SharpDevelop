// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Interface for advanced syntax highlighters.
	/// </summary>
	public interface IAdvancedHighlighter : IDisposable
	{
		/// <summary>
		/// Is called once after creating the highlighter. Gives your highlighter a chance
		/// to register events on the text editor.
		/// </summary>
		void Initialize(TextEditorControl textEditor);
		
		void BeginUpdate(IDocument document, IList<LineSegment> inputLines);
		void EndUpdate();
		
		/// <summary>
		/// Gives your highlighter the chance to change the highlighting of the words.
		/// </summary>
		void MarkLine(int lineNumber, LineSegment currentLine, List<TextWord> words);
	}
	
	/// <summary>
	/// Advanced syntax highlighter that stores a list of changed lines and can mark them
	/// later by calling <see cref="MarkOutstanding"/>.
	/// </summary>
	public abstract class AsynchronousAdvancedHighlighter : IAdvancedHighlighter
	{
		protected abstract void MarkWords(int lineNumber, LineSegment currentLine, List<TextWord> words);
		
		readonly object lockObject = new object();
		Dictionary<LineSegment, List<TextWord>> outstanding = new Dictionary<LineSegment, List<TextWord>>();
		TextEditorControl textEditor;
		IDocument document;
		
		#region Settings
		public TextEditorControl TextEditor {
			get {
				return textEditor;
			}
		}
		
		public IDocument Document {
			get {
				return document;
			}
		}
		
		int immediateMarkLimit = 3;
		
		/// <summary>
		/// Maximum number of changed lines to immediately mark the changes.
		/// </summary>
		protected int ImmediateMarkLimit {
			get {
				return immediateMarkLimit;
			}
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("value", value, "value must be >= 0");
				immediateMarkLimit = value;
			}
		}
		
		bool markVisibleOnly = true;
		/// <summary>
		/// Gets/Sets whether to only mark lines in the visible region of the text area.
		/// </summary>
		protected bool MarkVisibleOnly {
			get {
				return markVisibleOnly;
			}
			set {
				if (markVisibleOnly != value) {
					if (textEditor != null)
						throw new InvalidOperationException("Cannot change value after initialization");
					markVisibleOnly = value;
				}
			}
		}
		int markVisibleAdditional = 5;
		/// <summary>
		/// Number of additional lines around the visible region that should be marked.
		/// </summary>
		public int MarkVisibleAdditional {
			get {
				return markVisibleAdditional;
			}
			set {
				if (value < 0) throw new ArgumentOutOfRangeException("value", value, "value must be >= 0");
				markVisibleAdditional = value;
			}
		}
		#endregion
		
		public virtual void Initialize(TextEditorControl textEditor)
		{
			if (textEditor == null)
				throw new ArgumentNullException("textEditor");
			if (this.textEditor != null)
				throw new InvalidOperationException("Already initialized");
			this.textEditor = textEditor;
			this.document = textEditor.Document;
		}
		
		public virtual void Dispose()
		{
			textEditor = null;
			document = null;
		}
		
		int directMark;
		
		public virtual void BeginUpdate(IDocument document, IList<LineSegment> inputLines)
		{
			if (this.document == null)
				throw new InvalidOperationException("Not initialized");
			if (document != this.document)
				throw new InvalidOperationException("document != this.document");
			if (inputLines == null) {
				lock (lockObject) {
					outstanding.Clear();
				}
			} else {
				directMark = inputLines.Count > immediateMarkLimit ? 0 : inputLines.Count;
			}
		}
		
		public virtual void EndUpdate()
		{
		}
		
		void IAdvancedHighlighter.MarkLine(int lineNumber, LineSegment currentLine, List<TextWord> words)
		{
			if (directMark > 0) {
				directMark--;
				MarkWords(lineNumber, currentLine, words);
			} else {
				lock (lockObject) {
					outstanding[currentLine] = words;
				}
			}
		}
		
		protected virtual void MarkOutstanding()
		{
			if (WorkbenchSingleton.InvokeRequired) {
				// TODO: allow calling MarkOutstanding in separate threads
				throw new InvalidOperationException("Invoke required");
			}
			
			IEnumerable<KeyValuePair<LineSegment, List<TextWord>>> oldOutstanding;
			lock (lockObject) {
				oldOutstanding = outstanding;
				outstanding = new Dictionary<LineSegment, List<TextWord>>();
			}
			// We cannot call MarkLine inside lock(lockObject) because then the main
			// thread could deadlock with the highlighter thread.
			foreach (KeyValuePair<LineSegment, List<TextWord>> pair in oldOutstanding) {
				if (pair.Key.IsDeleted)
					continue;
				int offset = pair.Key.Offset;
				if (offset < 0 || offset >= document.TextLength)
					continue;
				int lineNumber = document.GetLineNumberForOffset(offset);
				if (markVisibleOnly && IsVisible(lineNumber) == false) {
					lock (lockObject) {
						outstanding[pair.Key] = pair.Value;
					}
				} else {
					MarkWords(lineNumber, pair.Key, pair.Value);
				}
			}
		}
		
		bool IsVisible(int lineNumber)
		{
			TextView textView = textEditor.ActiveTextAreaControl.TextArea.TextView;
			int firstLine = textView.FirstVisibleLine;
			if (lineNumber < firstLine - markVisibleAdditional) {
				return false;
			}
			int lastLine = document.GetFirstLogicalLine(textView.FirstPhysicalLine + textView.VisibleLineCount);
			if (lineNumber > lastLine + markVisibleAdditional) {
				return false;
			}
			// line is visible if it is not folded away
			return document.GetVisibleLine(lineNumber) != document.GetVisibleLine(lineNumber - 1);
		}
	}
}
