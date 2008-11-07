// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Undo stack implementation.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	public sealed class UndoStack : INotifyPropertyChanged
	{
		Stack<IUndoableOperation> undostack = new Stack<IUndoableOperation>();
		Stack<IUndoableOperation> redostack = new Stack<IUndoableOperation>();
		
		bool acceptChanges = true;
		
		/// <summary>
		/// Gets if the undo stack currently accepts changes.
		/// Is false while an undo action is running.
		/// </summary>
		public bool AcceptChanges {
			get { return acceptChanges; }
		}
		
		/// <summary>
		/// Gets if there are actions on the undo stack.
		/// Use the PropertyChanged event to listen to changes of this property.
		/// </summary>
		public bool CanUndo {
			get { return undostack.Count > 0; }
		}
		
		/// <summary>
		/// Gets if there are actions on the redo stack.
		/// Use the PropertyChanged event to listen to changes of this property.
		/// </summary>
		public bool CanRedo {
			get { return redostack.Count > 0; }
		}
		
		int undoGroupDepth;
		int actionCountInUndoGroup;
		int optionalActionCount;
		
		/// <summary>
		/// Starts grouping changes.
		/// Maintains a counter so that nested calls are possible.
		/// </summary>
		public void StartUndoGroup()
		{
			if (undoGroupDepth == 0) {
				actionCountInUndoGroup = 0;
				optionalActionCount = 0;
			}
			undoGroupDepth++;
			//Util.LoggingService.Debug("Open undo group (new depth=" + undoGroupDepth + ")");
		}
		
		/// <summary>
		/// Stops grouping changes.
		/// </summary>
		public void EndUndoGroup()
		{
			if (undoGroupDepth == 0) throw new InvalidOperationException("There are no open undo groups");
			undoGroupDepth--;
			//Util.LoggingService.Debug("Close undo group (new depth=" + undoGroupDepth + ")");
			if (undoGroupDepth == 0) {
				if (actionCountInUndoGroup == optionalActionCount) {
					// only optional actions: don't store them
					for (int i = 0; i < optionalActionCount; i++) {
						undostack.Pop();
					}
				} else if (actionCountInUndoGroup > 1) {
					undostack.Push(new UndoOperationGroup(undostack, actionCountInUndoGroup));
				}
			}
		}
		
		/// <summary>
		/// Throws an InvalidOperationException if an undo group is current open.
		/// </summary>
		void VerifyNoUndoGroupOpen()
		{
			if (undoGroupDepth != 0) {
				undoGroupDepth = 0;
				throw new InvalidOperationException("No undo group should be open at this point");
			}
		}
		
		/// <summary>
		/// Call this method to undo the last operation on the stack
		/// </summary>
		public void Undo()
		{
			VerifyNoUndoGroupOpen();
			if (undostack.Count > 0) {
				acceptChanges = false;
				IUndoableOperation uedit = undostack.Pop();
				redostack.Push(uedit);
				uedit.Undo();
				acceptChanges = true;
				if (undostack.Count == 0)
					NotifyPropertyChanged("CanUndo");
				if (redostack.Count == 1)
					NotifyPropertyChanged("CanRedo");
			}
		}
		
		/// <summary>
		/// Call this method to redo the last undone operation
		/// </summary>
		public void Redo()
		{
			VerifyNoUndoGroupOpen();
			if (redostack.Count > 0) {
				acceptChanges = false;
				IUndoableOperation uedit = redostack.Pop();
				undostack.Push(uedit);
				uedit.Redo();
				acceptChanges = true;
				if (redostack.Count == 0)
					NotifyPropertyChanged("CanRedo");
				if (undostack.Count == 1)
					NotifyPropertyChanged("CanUndo");
			}
		}
		
		/// <summary>
		/// Call this method to push an UndoableOperation on the undostack.
		/// The redostack will be cleared if you use this method.
		/// </summary>
		public void Push(IUndoableOperation operation)
		{
			Push(operation, false);
		}
		
		/// <summary>
		/// Call this method to push an UndoableOperation on the undostack.
		/// However, the operation will be only stored if the undo group contains a
		/// non-optional operation.
		/// Use this method to store the caret position/selection on the undo stack to
		/// prevent having only actions that affect only the caret and not the document.
		/// </summary>
		public void PushOptional(IUndoableOperation operation)
		{
			if (undoGroupDepth == 0)
				throw new InvalidOperationException("Cannot use PushOptional outside of undo group");
			Push(operation, true);
		}
		
		void Push(IUndoableOperation operation, bool isOptional)
		{
			if (operation == null) {
				throw new ArgumentNullException("operation");
			}
			
			if (acceptChanges) {
				bool wasEmpty = undostack.Count == 0;
				
				StartUndoGroup();
				undostack.Push(operation);
				actionCountInUndoGroup++;
				if (isOptional)
					optionalActionCount++;
				EndUndoGroup();
				if (wasEmpty)
					NotifyPropertyChanged("CanUndo");
				ClearRedoStack();
			}
		}
		
		/// <summary>
		/// Call this method, if you want to clear the redo stack
		/// </summary>
		public void ClearRedoStack()
		{
			if (redostack.Count != 0) {
				redostack.Clear();
				NotifyPropertyChanged("CanRedo");
			}
		}
		
		/// <summary>
		/// Clears both the undo and redo stack.
		/// </summary>
		public void ClearAll()
		{
			VerifyNoUndoGroupOpen();
			if (undostack.Count != 0) {
				undostack.Clear();
				NotifyPropertyChanged("CanUndo");
			}
			ClearRedoStack();
			actionCountInUndoGroup = 0;
			optionalActionCount = 0;
		}
		
		internal void AttachToDocument(TextDocument document)
		{
			document.UpdateStarted += document_UpdateStarted;
			document.UpdateFinished += document_UpdateFinished;
			document.Changing += document_Changing;
		}
		
		void document_UpdateStarted(object sender, EventArgs e)
		{
			StartUndoGroup();
		}
		
		void document_UpdateFinished(object sender, EventArgs e)
		{
			EndUndoGroup();
		}
		
		void document_Changing(object sender, DocumentChangeEventArgs e)
		{
			TextDocument document = (TextDocument)sender;
			Push(new DocumentChangeOperation(
				document,
				e.Offset,
				document.GetText(e.Offset, e.RemovalLength),
				e.InsertedText));
		}
		
		/// <summary>
		/// Is raised when a property (CanUndo, CanRedo) changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		
		void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
