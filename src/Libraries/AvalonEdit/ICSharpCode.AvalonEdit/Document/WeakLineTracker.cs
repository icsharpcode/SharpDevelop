// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.AvalonEdit.Document
{
	sealed class WeakLineTracker : ILineTracker
	{
		TextDocument textDocument;
		WeakReference targetObject;
		
		public WeakLineTracker(TextDocument textDocument, ILineTracker targetTracker)
		{
			this.textDocument = textDocument;
			this.targetObject = new WeakReference(targetTracker);
		}
		
		void Deregister()
		{
			textDocument.LineTracker.Remove(this);
		}
		
		public void BeforeRemoveLine(DocumentLine line)
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.BeforeRemoveLine(line);
			else
				Deregister();
		}
		
		public void SetLineLength(DocumentLine line, int newTotalLength)
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.SetLineLength(line, newTotalLength);
			else
				Deregister();
		}
		
		public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
		{
			ILineTracker targetTracker = targetObject.Target as ILineTracker;
			if (targetTracker != null)
				targetTracker.LineInserted(insertionPos, newLine);
			else
				Deregister();
		}
		
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
