// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Editing
{
	/// <summary>
	/// Stores a list of foldings for a specific TextView and TextDocument.
	/// </summary>
	public class FoldingManager
	{
		internal readonly TextView textView;
		internal readonly TextDocument document;
		
		readonly TextSegmentCollection<FoldingSection> foldings;
		
		/// <summary>
		/// Creates a new FoldingManager instance.
		/// </summary>
		public FoldingManager(TextView textView, TextDocument document)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (document == null)
				throw new ArgumentNullException("document");
			this.textView = textView;
			this.document = document;
			this.foldings = new TextSegmentCollection<FoldingSection>(document);
		}
		
		// keep a reference to the helper as long as the folding manager exists
		ExpandFoldingsWhenCaretIsMovedIntoThemHelper helper;
		
		/// <summary>
		/// Will listen to Caret.PositionChanged events and automatically expand folding sections
		/// when the caret is moved into them.
		/// </summary>
		public void ExpandFoldingsWhenCaretIsMovedIntoThem(Caret caret)
		{
			if (helper == null)
				helper = new ExpandFoldingsWhenCaretIsMovedIntoThemHelper(this);
			CaretWeakEventManager.PositionChanged.AddListener(caret, helper);
		}
		
		sealed class ExpandFoldingsWhenCaretIsMovedIntoThemHelper : IWeakEventListener
		{
			FoldingManager manager;
			
			public ExpandFoldingsWhenCaretIsMovedIntoThemHelper(FoldingManager manager)
			{
				this.manager = manager;
			}
			
			bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
			{
				if (managerType == typeof(CaretWeakEventManager.PositionChanged)) {
					Caret caret = (Caret)sender;
					int caretOffset = caret.Offset;
					foreach (FoldingSection s in manager.GetFoldingsContaining(caretOffset)) {
						if (s.IsFolded && s.StartOffset < caretOffset && caretOffset < s.EndOffset) {
							s.IsFolded = false;
						}
					}
					return true;
				}
				return false;
			}
		}
		
		/// <summary>
		/// Creates a folding for the specified text section.
		/// </summary>
		public FoldingSection CreateFolding(int startOffset, int endOffset)
		{
			if (startOffset >= endOffset)
				throw new ArgumentException("startOffset must be less than endOffset");
			FoldingSection fs = new FoldingSection(this, startOffset, endOffset);
			foldings.Add(fs);
			textView.Redraw();
			return fs;
		}
		
		/// <summary>
		/// Removes a folding section from this manager.
		/// </summary>
		public void RemoveFolding(FoldingSection fs)
		{
			if (fs == null)
				throw new ArgumentNullException("fs");
			document.VerifyAccess();
			fs.IsFolded = false;
			foldings.Remove(fs);
			textView.Redraw(fs, DispatcherPriority.Normal);
		}
		
		/// <summary>
		/// Removes all folding sections.
		/// </summary>
		public void Clear()
		{
			document.VerifyAccess();
			foreach (FoldingSection s in foldings)
				s.IsFolded = false;
			foldings.Clear();
			textView.Redraw();
		}
		
		/// <summary>
		/// Gets all foldings in this manager.
		/// The foldings are returned sorted by start offset;
		/// for multiple foldings at the same offset the order is undefined.
		/// </summary>
		public IEnumerable<FoldingSection> AllFoldings {
			get { return foldings; }
		}
		
		/// <summary>
		/// Gets the first offset greater or equal to <paramref name="startOffset"/> where a folded folding starts.
		/// Returns -1 if there are no foldings after <paramref name="startOffset"/>.
		/// </summary>
		public int GetNextFoldedFoldingStart(int startOffset)
		{
			FoldingSection fs = foldings.FindFirstSegmentWithStartAfter(startOffset);
			while (fs != null && !fs.IsFolded)
				fs = foldings.GetNextSegment(fs);
			return fs != null ? fs.StartOffset : -1;
		}
		
		/// <summary>
		/// Gets the first folding with a <see cref="TextSegment.StartOffset"/> greater or equal to
		/// <paramref name="startOffset"/>.
		/// Returns null if there are no foldings after <paramref name="startOffset"/>.
		/// </summary>
		public FoldingSection GetNextFolding(int startOffset)
		{
			// TODO: returns the longest folding instead of any folding at the first position after startOffset
			return foldings.FindFirstSegmentWithStartAfter(startOffset);
		}
		
		/// <summary>
		/// Gets all foldings that start exactly at <paramref name="startOffset"/>.
		/// </summary>
		public ReadOnlyCollection<FoldingSection> GetFoldingsAt(int startOffset)
		{
			List<FoldingSection> result = new List<FoldingSection>();
			FoldingSection fs = foldings.FindFirstSegmentWithStartAfter(startOffset);
			while (fs != null && fs.StartOffset == startOffset) {
				result.Add(fs);
				fs = foldings.GetNextSegment(fs);
			}
			return result.AsReadOnly();
		}
		
		/// <summary>
		/// Gets all foldings that contain <param name="offset" />.
		/// </summary>
		public ReadOnlyCollection<FoldingSection> GetFoldingsContaining(int offset)
		{
			return foldings.FindSegmentsContaining(offset);
		}
	}
}
