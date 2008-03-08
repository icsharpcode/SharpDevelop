// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace HexEditor.Util
{
	/// <summary>
	/// Description of UndoManager.
	/// </summary>
	public class UndoManager
	{
		Stack UndoStack;
		Stack RedoStack;
		
		public event EventHandler<UndoEventArgs> ActionUndone;
		public event EventHandler<UndoEventArgs> ActionRedone;
		
		public UndoManager()
		{
			UndoStack = new Stack();
			RedoStack = new Stack();
		}
		
		/// <summary>
		/// Determines if there's any further step to undo.
		/// </summary>
		public bool CanUndo {
			get { return (UndoStack.Count > 0); }
		}
		
		/// <summary>
		/// Determines if there's any further step to redo.
		/// </summary>
		public bool CanRedo {
			get { return (RedoStack.Count > 0); }
		}
		
		/// <summary>
		/// Adds a step to the stack.
		/// </summary>
		/// <param name="step">The step to add.</param>
		/// <remarks>Used internally, don't use!</remarks>
		internal void AddUndoStep(UndoStep step)
		{
			UndoStack.Push(step);
			RedoStack.Clear();
			
			EventHandler<UndoEventArgs> temp = ActionUndone;
			if (temp != null)
				temp(this, new UndoEventArgs(step, false));
		}
		
		/// <summary>
		/// Undoes the last step.
		/// </summary>
		/// <param name="buffer">Buffer to use</param>
		/// <remarks>Used internally, don't use!</remarks>
		internal UndoStep Undo(ref BufferManager buffer)
		{
			if (UndoStack.Count > 0) {
				UndoStep step = (UndoStep)UndoStack.Peek();
				RedoStack.Push(step);
				UndoStack.Pop();
				switch (step.Action) {
					case UndoAction.Add :
						buffer.SetBytes(step.Start, step.GetBytes(), false);
						break;
					case UndoAction.Remove :
						buffer.RemoveBytes(step.Start, step.GetBytes().Length);
						break;
					case UndoAction.Overwrite :
						buffer.SetBytes(step.Start, step.GetOldBytes(), true);
						break;
				}
				EventHandler<UndoEventArgs> temp = ActionUndone;
				if (temp != null)
					temp(this, new UndoEventArgs(step, true));
				
				return step;
			}
			return null;
		}
		
		/// <summary>
		/// Redoes the last step.
		/// </summary>
		/// <param name="buffer">Buffer to use</param>
		/// <remarks>Used internally, don't use!</remarks>
		internal UndoStep Redo(ref BufferManager buffer)
		{
			if (RedoStack.Count > 0) {
				UndoStep step = (UndoStep)RedoStack.Peek();
				UndoStack.Push(step);
				RedoStack.Pop();
				switch (step.Action) {
					case UndoAction.Add :
						buffer.RemoveBytes(step.Start, step.GetBytes().Length);
						break;
					case UndoAction.Remove :
						buffer.SetBytes(step.Start, step.GetBytes(), false);
						break;
					case UndoAction.Overwrite :
						buffer.SetBytes(step.Start, step.GetBytes(), true);
						break;
				}
				EventHandler<UndoEventArgs> temp = ActionRedone;
				if (temp != null)
					temp(this, new UndoEventArgs(step, false));
				
				return step;
			}
			return null;
		}
	}
}
