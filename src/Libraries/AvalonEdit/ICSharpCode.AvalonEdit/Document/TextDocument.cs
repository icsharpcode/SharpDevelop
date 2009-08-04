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
		readonly Rope<char> rope = new Rope<char>();
		readonly DocumentLineTree lineTree;
		readonly LineManager lineManager;
		readonly TextAnchorTree anchorTree;
		
		/// <summary>
		/// Create an empty text document.
		/// </summary>
		public TextDocument()
		{
			lineTree = new DocumentLineTree(this);
			lineManager = new LineManager(rope, lineTree, this);
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
			if (offset < 0 || offset > rope.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
			}
			if (length < 0 || offset + length > rope.Length) {
				throw new ArgumentOutOfRangeException("length", length, "0 <= length, offset(" + offset + ")+length <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
			}
		}
		
		/// <inheritdoc/>
		public string GetText(int offset, int length)
		{
			VerifyAccess();
			return rope.ToString(offset, length);
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
		public char GetCharAt(int offset)
		{
			VerifyAccess();
			return rope[offset];
		}
		
//		/// <summary>
//		/// Like GetCharAt, but without any safety checks.
//		/// </summary>
//		internal char FastGetCharAt(int offset)
//		{
//			return textBuffer.GetCharAt(offset);
//		}
		
		WeakReference cachedText;
		
		/// <summary>
		/// Gets/Sets the text of the whole document.
		/// Get: O(n)
		/// Set: O(n * log n)
		/// </summary>
		public string Text {
			get {
				VerifyAccess();
				string completeText = cachedText != null ? (cachedText.Target as string) : null;
				if (completeText == null) {
					completeText = rope.ToString();
					cachedText = new WeakReference(completeText);
				}
				return completeText;
			}
			set {
				VerifyAccess();
				if (value == null)
					throw new ArgumentNullException("value");
				Replace(0, rope.Length, value);
			}
		}
		
		/// <inheritdoc/>
		public event EventHandler TextChanged;
		
		/// <inheritdoc/>
		public int TextLength {
			get {
				VerifyAccess();
				return rope.Length;
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
		
		/// <summary>
		/// Creates a snapshot of the current text.
		/// </summary>
		/// <returns>Returns a copy of the document's text as a rope.</returns>
		/// <remarks>
		/// Unlike all other TextDocument methods, this method may be called from any thread; even when the owning thread
		/// is concurrently writing to the document.
		/// This special thread-safety guarantee is valid only for TextDocument.CreateSnapshot(), not necessarily for other
		/// classes implementing ITextSource.CreateSnapshot().
		/// </remarks>
		public ITextSource CreateSnapshot()
		{
			lock (rope) {
				return new RopeTextSource(rope.Clone());
			}
		}
		
		/// <summary>
		/// Creates a snapshot of a part of the current text.
		/// </summary>
		/// <remarks>
		/// Unlike all other TextDocument methods, this method may be called from any thread; even when the owning thread
		/// is concurrently writing to the document.
		/// This special thread-safety guarantee is valid only for TextDocument.CreateSnapshot(), not necessarily for other
		/// classes implementing ITextSource.CreateSnapshot().
		/// </remarks>
		public ITextSource CreateSnapshot(int offset, int length)
		{
			lock (rope) {
				return new RopeTextSource(rope.GetRange(offset, length));
			}
		}
		
		/// <inheritdoc/>
		public System.IO.TextReader CreateReader()
		{
			lock (rope) {
				return new RopeTextReader(rope);
			}
		}
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
			
			int textLength = rope.Length;
			if (textLength != oldTextLength) {
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
		/// </summary>
		public void Insert(int offset, string text)
		{
			Replace(offset, 0, text);
		}
		
		/// <summary>
		/// Removes text.
		/// </summary>
		public void Remove(ISegment segment)
		{
			Replace(segment, string.Empty);
		}
		
		/// <summary>
		/// Removes text.
		/// </summary>
		public void Remove(int offset, int length)
		{
			Replace(offset, length, string.Empty);
		}
		
		internal bool inDocumentChanging;
		
		/// <summary>
		/// Replaces text.
		/// </summary>
		public void Replace(ISegment segment, string text)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");
			Replace(segment.Offset, segment.Length, text, null);
		}
		
		/// <summary>
		/// Replaces text.
		/// </summary>
		public void Replace(int offset, int length, string text)
		{
			Replace(offset, length, text, null);
		}
		
		/// <summary>
		/// Replaces text.
		/// </summary>
		/// <param name="offset">The starting offset of the text to be replaced.</param>
		/// <param name="length">The length of the text to be replaced.</param>
		/// <param name="text">The new text.</param>
		/// <param name="offsetChangeMappingType">The offsetChangeMappingType determines how offsets inside the old text are mapped to the new text.
		/// This affects how the anchors and segments inside the replaced region behave.</param>
		public void Replace(int offset, int length, string text, OffsetChangeMappingType offsetChangeMappingType)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			// Please see OffsetChangeMappingType XML comments for details on how these modes work.
			switch (offsetChangeMappingType) {
				case OffsetChangeMappingType.Normal:
					Replace(offset, length, text, null);
					break;
				case OffsetChangeMappingType.RemoveAndInsert:
					if (length == 0 || text.Length == 0) {
						// only insertion or only removal?
						// OffsetChangeMappingType doesn't matter, just use Normal.
						Replace(offset, length, text, null);
					} else {
						OffsetChangeMap map = new OffsetChangeMap(2);
						map.Add(new OffsetChangeMapEntry(offset, length, 0));
						map.Add(new OffsetChangeMapEntry(offset, 0, text.Length));
						map.Freeze();
						Replace(offset, length, text, map);
					}
					break;
				case OffsetChangeMappingType.CharacterReplace:
					if (length == 0 || text.Length == 0) {
						// only insertion or only removal?
						// OffsetChangeMappingType doesn't matter, just use Normal.
						Replace(offset, length, text, null);
					} else if (text.Length > length) {
						// look at OffsetChangeMappingType.CharacterReplace XML comments on why we need to replace
						// the last
						OffsetChangeMapEntry entry = new OffsetChangeMapEntry(offset + length - 1, 1, 1 + text.Length - length);
						Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry));
					} else if (text.Length < length) {
						OffsetChangeMapEntry entry = new OffsetChangeMapEntry(offset + length - text.Length, length - text.Length, 0);
						Replace(offset, length, text, OffsetChangeMap.FromSingleElement(entry));
					} else {
						Replace(offset, length, text, OffsetChangeMap.Empty);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException("offsetChangeMappingType", offsetChangeMappingType, "Invalid enum value");
			}
		}
		
		/// <summary>
		/// Replaces text.
		/// </summary>
		/// <param name="offset">The starting offset of the text to be replaced.</param>
		/// <param name="length">The length of the text to be replaced.</param>
		/// <param name="text">The new text.</param>
		/// <param name="offsetChangeMap">The offsetChangeMap determines how offsets inside the old text are mapped to the new text.
		/// This affects how the anchors and segments inside the replaced region behave.
		/// If you pass null (the default when using one of the other overloads), the offsets are changed as
		/// in OffsetChangeMappingType.Normal mode.
		/// If you pass OffsetChangeMap.Empty, then everything will stay in its old place (OffsetChangeMappingType.CharacterReplace mode).
		/// The offsetChangeMap must be a valid 'explanation' for the document change. See <see cref="OffsetChangeMap.IsValidForDocumentChange"/>.
		/// Passing an OffsetChangeMap to the Replace method will automatically freeze it to ensure the thread safety of the resulting
		/// DocumentChangeEventArgs instance.
		/// </param>
		public void Replace(int offset, int length, string text, OffsetChangeMap offsetChangeMap)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			
			if (offsetChangeMap != null)
				offsetChangeMap.Freeze();
			
			BeginUpdate();
			try {
				if (inDocumentChanging)
					throw new InvalidOperationException("Cannot change document within another document change.");
				// protect document change against corruption by other changes inside the event handlers
				inDocumentChanging = true;
				try {
					// The range verification must wait until after the BeginUpdate() call because the document
					// might be modified inside the UpdateStarted event.
					VerifyRange(offset, length);
					
					DoReplace(offset, length, text, offsetChangeMap);
				} finally {
					inDocumentChanging = false;
				}
			} finally {
				EndUpdate();
			}
		}
		
		void DoReplace(int offset, int length, string text, OffsetChangeMap offsetChangeMap)
		{
			if (length == 0 && text.Length == 0)
				return;
			
			// trying to replace a single character in 'Normal' mode?
			// for single characters, 'CharacterReplace' mode is equivalent, but more performant
			// (we don't have to touch the anchorTree at all in 'CharacterReplace' mode)
			if (length == 1 && text.Length == 1 && offsetChangeMap == null)
				offsetChangeMap = OffsetChangeMap.Empty;
			
			DocumentChangeEventArgs args = new DocumentChangeEventArgs(offset, length, text, offsetChangeMap);
			
			// fire DocumentChanging event
			if (Changing != null)
				Changing(this, args);
			
			cachedText = null; // reset cache of complete document text
			fireTextChanged = true;
			DelayedEvents delayedEvents = new DelayedEvents();
			
			lock (rope) {
				// now update the textBuffer and lineTree
				if (offset == 0 && length == rope.Length) {
					// optimize replacing the whole document
					rope.Clear();
					rope.InsertText(0, text);
					lineManager.Rebuild(text);
				} else {
					rope.RemoveRange(offset, length);
					lineManager.Remove(offset, length);
					#if DEBUG
					lineTree.CheckProperties();
					#endif
					rope.InsertText(offset, text);
					lineManager.Insert(offset, text);
					#if DEBUG
					lineTree.CheckProperties();
					#endif
				}
			}
			
			// update text anchors
			if (offsetChangeMap == null) {
				anchorTree.HandleTextChange(args.CreateSingleChangeMapEntry(), delayedEvents);
			} else {
				foreach (OffsetChangeMapEntry entry in offsetChangeMap) {
					anchorTree.HandleTextChange(entry, delayedEvents);
				}
			}
			
			// raise delayed events after our data structures are consistent again
			delayedEvents.RaiseEvents();
			
			// fire DocumentChanged event
			if (Changed != null)
				Changed(this, args);
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
			if (offset < 0 || offset > rope.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + rope.Length.ToString());
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
			if (offset < 0 || offset > rope.Length) {
				throw new ArgumentOutOfRangeException("offset", offset, "0 <= offset <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
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
