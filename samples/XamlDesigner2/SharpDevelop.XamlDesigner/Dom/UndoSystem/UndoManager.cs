using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SharpDevelop.XamlDesigner.Dom.UndoSystem
{
	public class UndoManager
	{
		Stack<UndoAction> undoStack = new Stack<UndoAction>();
		Stack<UndoAction> redoStack = new Stack<UndoAction>();
		UndoTransaction trans;

		public EventHandler Changed;

		public bool CanUndo
		{
			get { return undoStack.Count > 0; }
		}

		public bool CanRedo
		{
			get { return redoStack.Count > 0; }
		}

		public void Undo()
		{
			if (CanUndo) {
				var action = undoStack.Pop();
				action.Undo();
				redoStack.Push(action);
			}
			else {
				throw new Exception();
			}
		}

		public void Redo()
		{
			if (CanRedo) {
				var action = redoStack.Pop();
				action.Redo();
				undoStack.Push(action);
			}
			else {
				throw new Exception();
			}
		}

		public void Execute(UndoAction action)
		{
			action.Redo();
			Done(action);
		}

		public void Done(UndoAction action)
		{
			if (trans != null) {
				var propertyAction = action as PropertyAction;
				if (propertyAction != null) {
					foreach (var a in trans.Actions.OfType<PropertyAction>()) {
						if (a.TryMergeWith(propertyAction)) {
							return;
						}
					}
				}
				trans.Actions.Add(action);
			}
			else {
				undoStack.Push(action);
				redoStack.Clear();
			}
		}

		public bool OpenTransaction()
		{
			if (trans == null) {
				trans = new UndoTransaction();
				return true;
			}
			return false;
		}

		public void CommitTransaction()
		{
			var commited = trans;
			trans = null;
			if (commited.Actions.Count > 0) {
				Done(commited);
			}
		}

		public void AbortTransaction()
		{
			trans.Undo();
			trans = null;
		}

		static bool opened;

		static UndoManager FindUndoManager(FrameworkElement element)
		{
			var contextHolder = element.AncestorsAndSelf().OfType<IHasContext>().FirstOrDefault();
			if (contextHolder != null) {
				return contextHolder.Context.UndoManager;
			}
			return null;
		}

		public static void OnDragStarted(FrameworkElement element)
		{
			var manager = FindUndoManager(element);
			if (manager != null) {
				opened = manager.OpenTransaction();
			}
		}

		public static void OnDragCompleted(FrameworkElement element)
		{
			if (opened) {
				var manager = FindUndoManager(element);
				if (manager != null) {
					manager.CommitTransaction();
				}
			}
		}

		public static void OnDragCanceled(FrameworkElement element)
		{
			if (opened) {
				var manager = FindUndoManager(element);
				if (manager != null) {
					manager.AbortTransaction();
				}
			}
		}
	}
}
