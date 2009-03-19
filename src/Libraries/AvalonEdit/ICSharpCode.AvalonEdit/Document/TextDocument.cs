// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.Threading;

using ICSharpCode.AvalonEdit.Utils;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Runtimes:
	/// n = number of lines in the document
	/// </summary>
	public sealed class TextDocument : ITextSource
	{
		#region Thread ownership
		readonly object lockObject = new object();
		Thread owner = Thread.CurrentThread;
		
		/// <summary>
		/// Verifies that the current thread is the documents owner thread.
		/// Throws an <see cref="InvalidOperationException"/> if the wrong thread accesses the TextDocument.
		/// </summary>
		public void VerifyAccess()
		{
			if (Thread.CurrentThread != owner)
				throw new InvalidOperationException("TextDocument can be accessed only from the thread that owns it.");
		}
		
		/// <summary>
		/// Transfers ownership of the document to another thread. This method can be used to load
		/// a file into a TextDocument on a background thread and then transfer ownership to the UI thread
		/// for displaying the document.
		/// </summary>
		/// <remarks>
		/// The owner can be set to null, which means that no thread can access the document. But, if the document
		/// has no owner thread, any thread may take ownership by calling SetOwnerThread.
		/// </remarks>
		public void SetOwnerThread(Thread newOwner)
		{
			// We need to lock here to ensure that in the null owner case,
			// only one thread succeeds in taking ownership.
			lock (lockObject) {
				if (owner != null) {
					VerifyAccess();
				}
				owner = newOwner;
			}
		}
		#endregion
		
		#region Fields + Constructor
		readonly GapTextBuffer textBuffer = new GapTextBuffer();
		readonly DocumentLineTree lineTree;
		readonly LineManager lineManager;
		readonly TextAnchorTree anchorTree;
		
		/// <summary>
		/// Create an empty text document.
		/// </summary>
		public TextDocument()
		{
			lineTree = new DocumentLineTree(this);
			lineManager = new LineManager(textBuffer, lineTree, this);
			lineTrackers.CollectionChanged += delegate { 
				lineManager.lineTrackers = lineTrackers.ToArray();
			};
			
			anchorTree = new TextAnchorTree(this);
			undoStack = new UndoStack();
			undoStack.AttachToDocument(this);
			FireChangeEvents();
		}
		#endregion
		
		#region Text
		void VerifyRange(int offset, int length)
		{
			if (offset < 0 || offset > textBuffer.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + textBuffer.Length.ToString(CultureInfo.InvariantCulture));
			}
			if (length < 0 || offset + length > textBuffer.Length) {
				throw new ArgumentOutOfRangeException("length", length, "0 <= length, offset(" + offset + ")+length <= " + textBuffer.Length.ToString(CultureInfo.InvariantCulture));
			}
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			VerifyAccess();
			VerifyRange(offset, length);
			return textBuffer.GetText(offset, length);
		}
		
		/// <summary>
		/// Retrieves the text for a portion of the document.
		/// </summary>
		public string GetText(ISegment segment)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");
			return GetText(segment.Offset, segment.Length);
		}
		
		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
		public char GetCharAt(int offset)
		{
			VerifyAccess();
			if (offset < 0 || offset >= textBuffer.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset < " + textBuffer.Length.ToString());
			}
			return textBuffer.GetCharAt(offset);
		}
		
//		/// <summary>
//		/// Like GetCharAt, but without any safety checks.
//		/// </summary>
//		internal char FastGetCharAt(int offset)
//		{
//			return textBuffer.GetCharAt(offset);
//		}
		
		/// <summary>
		/// Gets/Sets the text of the whole document.
		/// Get: O(n)
		/// Set: O(n * log n)
		/// </summary>
		public string Text {
			get {
				VerifyAccess();
				return textBuffer.Text;
			}
			set {
				VerifyAccess();
				if (value == null)
					throw new ArgumentNullException("value");
				Replace(0, textBuffer.Length, value);
			}
		}
		
		/// <inheritdoc/>
		public event EventHandler TextChanged;
		
		/// <inheritdoc/>
		public int TextLength {
			get {
				VerifyAccess();
				return textBuffer.Length;
			}
		}
		
		/// <summary>
		/// Is raised when the TextLength property changes.
		/// </summary>
		public event EventHandler TextLengthChanged;
		
		/// <summary>
		/// Is raised before the document changes.
		/// </summary>
		public event EventHandler<DocumentChangeEventArgs> Changing;
		
		/// <summary>
		/// Is raised after the document has changed.
		/// </summary>
		public event EventHandler<DocumentChangeEventArgs> Changed;
		#endregion
		
		#region BeginUpdate / EndUpdate
		int beginUpdateCount;
		
		/// <summary>
		/// Gets if an update is running.
		/// </summary>
		public bool IsInUpdate {
			get {
				VerifyAccess();
				return beginUpdateCount > 0;
			}
		}
		
		/// <summary>
		/// Immediately calls <see cref="BeginUpdate()"/>,
		/// and returns an IDisposable that calls <see cref="EndUpdate()"/>.
		/// </summary>
		public IDisposable RunUpdate()
		{
			BeginUpdate();
			return new CallbackOnDispose(EndUpdate);
		}
		
		/// <summary>
		/// Begins a group of document changes.
		/// Some events are suspended until EndUpdate is called, and the <see cref="UndoStack"/> will
		/// group all changes into a single action.
		/// Calling BeginUpdate several times increments a counter, only after the appropriate number
		/// of EndUpdate calls the events resume their work.
		/// </summary>
		public void BeginUpdate()
		{
			VerifyAccess();
			beginUpdateCount++;
			if (beginUpdateCount == 1) {
				if (UpdateStarted != null)
					UpdateStarted(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Ends a group of document changes.
		/// </summary>
		public void EndUpdate()
		{
			VerifyAccess();
			if (inDocumentChanging)
				throw new InvalidOperationException("Cannot end update within document change.");
			if (beginUpdateCount == 0)
				throw new InvalidOperationException("No update is active.");
			beginUpdateCount -= 1;
			if (beginUpdateCount == 0) {
				FireChangeEvents();
				if (UpdateFinished != null)
					UpdateFinished(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Occurs when a document change starts.
		/// </summary>
		public event EventHandler UpdateStarted;
		
		/// <summary>
		/// Occurs when a document change is finished.
		/// </summary>
		public event EventHandler UpdateFinished;
		#endregion
		
		#region Fire events after update
		int oldTextLength;
		int oldLineCount;
		bool fireTextChanged;
		
		/// <summary>
		/// Fires TextChanged, TextLengthChanged, TotalHeightChanged, LineCountChanged if required.
		/// </summary>
		internal void FireChangeEvents()
		{
			if (beginUpdateCount > 0)
				return;
			
			if (fireTextChanged) {
				fireTextChanged = false;
				if (TextChanged != null)
					TextChanged(this, EventArgs.Empty);
			}
			
			int textLength = textBuffer.Length;
			if (oldTextLength != textBuffer.Length) {
				oldTextLength = textLength;
				if (TextLengthChanged != null)
					TextLengthChanged(this, EventArgs.Empty);
			}
			int lineCount = lineTree.LineCount;
			if (lineCount != oldLineCount) {
				oldLineCount = lineCount;
				if (LineCountChanged != null)
					LineCountChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		
		#region Insert / Remove  / Replace
		/// <summary>
		/// Inserts text.
		/// Runtime:
		///  for updating the text buffer: m=size of new text, d=distance to last change
		/// 	usual:	O(m+d)
		/// 	rare:	O(m+n)
		///  for updating the document lines: O(m*log n), m=number of changed lines
		/// </summary>
		public void Insert(int offset, string text)
		{
			Replace(offset, 0, text);
		}
		
		/// <summary>
		/// Removes text.
		/// Runtime:
		///  for updating the text buffer: d=distance to last change
		/// 	usual:	O(d)
		/// 	rare:	O(n)
		///  for updating the document lines: O(m*log n), m=number of changed lines
		/// </summary>
		public void Remove(int offset, int length)
		{
			Replace(offset, length, string.Empty);
		}
		
		internal bool inDocumentChanging;
		
		/// <summary>
		/// Replaces text.
		/// Runtime:
		///  for updating the text buffer: m=size of new text, d=distance to last change
		/// 	usual:	O(m+d)
		/// 	rare:	O(m+n)
		///  for updating the document lines: O(m*log n), m=number of changed lines
		/// </summary>
		public void Replace(int offset, int length, string text)
		{
			if (inDocumentChanging)
				throw new InvalidOperationException("Cannot change document within another document change.");
			BeginUpdate();
			// protect document change against corruption by other changes inside the event handlers
			inDocumentChanging = true;
			try {
				VerifyRange(offset, length);
				if (text == null)
					throw new ArgumentNullException("text");
				if (length == 0 && text.Length == 0)
					return;
				
				fireTextChanged = true;
				
				DocumentChangeEventArgs args = new DocumentChangeEventArgs(offset, length, text);
				
				// fire DocumentChanging event
				if (Changing != null)
					Changing(this, args);
				
				DelayedEvents delayedEvents = new DelayedEvents();
				
				// now do the real work
				anchorTree.RemoveText(offset, length, delayedEvents);
				ReplaceInternal(offset, length, text);
				anchorTree.InsertText(offset, text.Length);
				
				delayedEvents.RaiseEvents();
				
				// fire DocumentChanged event
				if (Changed != null)
					Changed(this, args);
			} finally {
				inDocumentChanging = false;
				EndUpdate();
			}
		}
		
		void ReplaceInternal(int offset, int length, string text)
		{
			if (offset == 0 && length == textBuffer.Length) {
				textBuffer.Text = text;
				lineManager.Rebuild(text);
			} else {
				textBuffer.Remove(offset, length, text.Length);
				lineManager.Remove(offset, length);
				#if DEBUG
				lineTree.CheckProperties();
				#endif
				textBuffer.Insert(offset, text);
				lineManager.Insert(offset, text);
				#if DEBUG
				lineTree.CheckProperties();
				#endif
			}
		}
		#endregion
		
		#region GetLineBy...
		/// <summary>
		/// Gets a read-only list of lines.
		/// </summary>
		public IList<DocumentLine> Lines {
			get { return lineTree; }
		}
		
		/// <summary>
		/// Gets a line by the line number: O(log n)
		/// </summary>
		public DocumentLine GetLineByNumber(int number)
		{
			VerifyAccess();
			if (number < 1 || number > lineTree.LineCount)
				throw new ArgumentOutOfRangeException("number", number, "Value must be between 1 and " + lineTree.LineCount);
			return lineTree.GetByNumber(number);
		}
		
		/// <summary>
		/// Gets a document lines by offset.
		/// Runtime: O(log n)
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
		public DocumentLine GetLineByOffset(int offset)
		{
			VerifyAccess();
			if (offset < 0 || offset > textBuffer.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + textBuffer.Length.ToString());
			}
			return lineTree.GetByOffset(offset);
		}
		#endregion
		
		/// <summary>
		/// Gets the offset from a text location.
		/// </summary>
		public int GetOffset(TextLocation location)
		{
			DocumentLine line = GetLineByNumber(location.Line);
			return line.Offset + location.Column - 1;
		}
		
		/// <summary>
		/// Gets the location from an offset.
		/// </summary>
		public TextLocation GetLocation(int offset)
		{
			DocumentLine line = GetLineByOffset(offset);
			return new TextLocation(line.LineNumber, offset - line.Offset + 1);
		}
		
		readonly ObservableCollection<ILineTracker> lineTrackers = new ObservableCollection<ILineTracker>();
		
		/// <summary>
		/// Gets the list of <see cref="ILineTracker"/>s attached to this document.
		/// </summary>
		public IList<ILineTracker> LineTrackers {
			get {
				VerifyAccess();
				return lineTrackers;
			}
		}
		
		readonly UndoStack undoStack;
		
		/// <summary>
		/// Gets the <see cref="UndoStack"/> of the document.
		/// </summary>
		public UndoStack UndoStack {
			get { return undoStack; }
		}
		
		/// <summary>
		/// Creates a new text anchor at the specified offset.
		/// </summary>
		public TextAnchor CreateAnchor(int offset)
		{
			VerifyAccess();
			if (offset < 0 || offset > textBuffer.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + textBuffer.Length.ToString(CultureInfo.InvariantCulture));
			}
			return anchorTree.CreateAnchor(offset);
		}
		
		#region LineCount
		/// <summary>
		/// Gets the total number of lines in the document.
		/// Runtime: O(1).
		/// </summary>
		public int LineCount {
			get {
				VerifyAccess();
				return lineTree.LineCount;
			}
		}
		
		/// <summary>
		/// Is raised when the LineCount property changes.
		/// </summary>
		public event EventHandler LineCountChanged;
		#endregion
		
		#region Debugging
		[Conditional("DEBUG")]
		internal void DebugVerifyAccess()
		{
			VerifyAccess();
		}
		
		/// <summary>
		/// Gets the document lines tree in string form.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		internal string GetLineTreeAsString()
		{
			#if DEBUG
			return lineTree.GetTreeAsString();
			#else
			return "Not available in release build.";
			#endif
		}
		
		/// <summary>
		/// Gets the text anchor tree in string form.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		internal string GetTextAnchorTreeAsString()
		{
			#if DEBUG
			return anchorTree.GetTreeAsString();
			#else
			return "Not available in release build.";
			#endif
		}
		#endregion
	}
}
