// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Document
{
	public sealed class WeakLineTracker : ILineTracker
	{
		TextDocument textDocument;
		WeakReference targetObject;
		
		private WeakLineTracker(TextDocument textDocument, ILineTracker targetTracker)
		{
			this.textDocument = textDocument;
			this.targetObject = new WeakReference(targetTracker);
		}
		
		/// <summary>
		/// Registers the <paramref name="targetTracker"/> as line tracker for the <paramref name="textDocument"/>.
		/// A weak reference to the target tracker will be used, and the WeakLineTracker will deregister itself
		/// when the target tracker is garbage collected.
		/// </summary>
		public static WeakLineTracker Register(TextDocument textDocument, ILineTracker targetTracker)
		{
			if (textDocument == null)
				throw new ArgumentNullException("textDocument");
			if (targetTracker == null)
				throw new ArgumentNullException("targetTracker");
			WeakLineTracker wlt = new WeakLineTracker(textDocument, targetTracker);
			textDocument.LineTrackers.Add(wlt);
			return wlt;
		}
		
		/// <summary>
		/// Deregisters the weak line tracker.
		/// </summary>
		public void Deregister()
		{
			if (textDocument != null) {
				textDocument.LineTrackers.Remove(this);
				textDocument = null;
			}
		}
		
		/// <inheritdoc/>
		public void BeforeRemoveLine(DocumentLine line)
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.BeforeRemoveLine(line);
			else
				Deregister();
		}
		
		/// <inheritdoc/>
		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.SetLineLength(line, newTotalLength);
			else
				Deregister();
		}
		
		/// <inheritdoc/>
		public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.LineInserted(insertionPos, newLine);
			else
				Deregister();
		}
		
		/// <inheritdoc/>
		public void RebuildDocument()
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.RebuildDocument();
			else
				Deregister();
		}
	}
}
