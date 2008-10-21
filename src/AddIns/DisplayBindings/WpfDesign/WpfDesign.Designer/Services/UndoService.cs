// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3285 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	interface IMergableTransaction : IChangeGroup
	{
		bool TryMergeWith(IMergableTransaction other);
	}

	class SaveTextAction : IUndoAction
	{
		public SaveTextAction(IUndoAction core, ITextContainer textContainer)
		{
			this.core = core;
			this.textContainer = textContainer;
			this.text = textContainer.Text;
		}

		IUndoAction core;
		ITextContainer textContainer;
		string text;

		public IEnumerable<DesignItem> AffectedItems
		{
			get { return core.AffectedItems; }
		}

		public string Title
		{
			get { return core.Title; }
		}

		public void Do()
		{
			core.Do();
		}

		public void Undo()
		{
			core.Undo();
			textContainer.Text = text;
		}
	}

	class UndoTransaction : IChangeGroup
	{
		public UndoTransaction(string title, IEnumerable<DesignItem> affectedItems)
		{
			this.title = title;
			this.affectedItems = affectedItems;
		}

		string title;

		public string Title
		{
			get { return title; }
		}

		IEnumerable<DesignItem> affectedItems;

		public IEnumerable<DesignItem> AffectedItems
		{
			get { return affectedItems; }
		}

		public enum TransactionState
		{
			Open,
			Completed,
			Undone,
			Failed
		}

		TransactionState _state;

		public TransactionState State
		{
			get { return _state; }
		}

		List<IUndoAction> items = new List<IUndoAction>();

		public void Execute(IUndoAction item, bool done)
		{
			AssertState(TransactionState.Open);
			if (!done) item.Do();

			var a = item as IMergableTransaction;
			if (a != null) {
				foreach (var b in items.OfType<IMergableTransaction>()) {
					if (b.TryMergeWith(a)) return;
				}
			}

			items.Add(item);
		}

		private void AssertState(TransactionState expectedState)
		{
			if (_state != expectedState)
				throw new InvalidOperationException("Expected state " + expectedState + ", but state is " + _state);
		}

		public event EventHandler Committed;
		public event EventHandler RolledBack;

		public void Commit()
		{
			AssertState(TransactionState.Open);
			_state = TransactionState.Completed;
			if (Committed != null)
				Committed(this, EventArgs.Empty);
		}

		public void Abort()
		{
			AssertState(TransactionState.Open);
			_state = TransactionState.Failed;
			InternalRollback();
			if (RolledBack != null)
				RolledBack(this, EventArgs.Empty);
		}

		public void Undo()
		{
			AssertState(TransactionState.Completed);
			_state = TransactionState.Undone;
			InternalRollback();
		}

		void InternalRollback()
		{
			try {
				for (int i = items.Count - 1; i >= 0; i--) {
					items[i].Undo();
				}
			}
			catch {
				_state = TransactionState.Failed;
				throw;
			}
		}

		public void Redo()
		{
			AssertState(TransactionState.Undone);
			try {
				for (int i = 0; i < items.Count; i++) {
					items[i].Do();
				}
				_state = TransactionState.Completed;
			}
			catch {
				_state = TransactionState.Failed;
				try {
					InternalRollback();
				}
				catch (Exception ex) {
					Debug.WriteLine("Exception rolling back after Redo error:\n" + ex.ToString());
				}
				throw;
			}
		}

		void IUndoAction.Do()
		{
			if (_state != TransactionState.Completed) {
				Redo();
			}
		}

		public void Dispose()
		{
			if (_state == TransactionState.Open) {
				try {
					Abort();
				}
				catch (Exception ex) {
					Debug.WriteLine("Exception rolling back after failure:\n" + ex.ToString());
				}
			}
		}
	}

	/// <summary>
	/// Service supporting Undo/Redo actions on the design surface.
	/// </summary>
	class UndoService : IUndoService
	{
		public UndoService(DesignContext context)
		{
			this.context = context;
			context.SubscribeToService<ITextContainer>(delegate(ITextContainer textContainer) {
				this.textContainer = textContainer;
			});
		}

		DesignContext context;
		ITextContainer textContainer;

		Stack<UndoTransaction> transactionStack = new Stack<UndoTransaction>();
		Stack<IUndoAction> undoStack = new Stack<IUndoAction>();
		Stack<IUndoAction> redoStack = new Stack<IUndoAction>();

		public IChangeGroup OpenGroup(string title, IEnumerable<DesignItem> affectedItems)
		{
			var t = new UndoTransaction(title, affectedItems);
			transactionStack.Push(t);
			t.Committed += TransactionFinished;
			t.RolledBack += TransactionFinished;
			t.Committed += delegate(object sender, EventArgs e) {
				Execute((IUndoAction)sender);
			};
			return t;
		}

		void TransactionFinished(object sender, EventArgs e)
		{
			if (sender != transactionStack.Pop()) {
				throw new Exception("Invalid transaction finish, nested transactions must finish first");
			}
		}

		public void Execute(IUndoAction action)
		{
			Push(action, false);
		}

		public void Done(IUndoAction action)
		{
			Push(action, true);
		}

		void Push(IUndoAction action, bool done)
		{
			var prevAction = undoStack.Count == 0 ? null : undoStack.Peek();

			if ((prevAction == null || prevAction is ITextAction) && !(action is ITextAction)) {
				if (textContainer != null) {
					action = new SaveTextAction(action, textContainer);
				}
			}

			if (transactionStack.Count == 0) {
				if (!done) action.Do();
				undoStack.Push(action);
				redoStack.Clear();
				OnUndoStackChanged(action, OperationKind.Execute);
			}
			else {
				transactionStack.Peek().Execute(action, done);
			}
		}

		/// <summary>
		/// Gets if undo actions are available.
		/// </summary>
		public bool CanUndo
		{
			get { return undoStack.Count > 0; }
		}

		/// <summary>
		/// Is raised when the undo stack has changed.
		/// </summary>
		public event EventHandler UndoStackChanged;

		void OnUndoStackChanged(IUndoAction lastAction, OperationKind kind)
		{
			if (lastAction is ITextAction) {
				context.ParseSuggested = true;
			}
			else if (lastAction != null) {				
				// ensure root
				var item = lastAction.AffectedItems.FirstOrDefault();
				if (item != null && item.Parent != null) {
					var itemRoot = ModelTools.GetRoot(item);
					if (itemRoot != context.ModelService.Root) {
						context.ModelService.Root = itemRoot;
					}
				}
				// print context 
				var textAlreadyUpdated = lastAction is SaveTextAction && kind == OperationKind.Undo;
				if (!textAlreadyUpdated && textContainer != null) {
					textContainer.Text = context.Save();
				}
			}

			if (UndoStackChanged != null) {
				UndoStackChanged(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Undoes the last action.
		/// </summary>
		public void Undo()
		{
			if (!CanUndo)
				throw new InvalidOperationException("Cannot Undo: undo stack is empty");
			if (transactionStack.Count != 0)
				throw new InvalidOperationException("Cannot Undo while transaction is running");
			IUndoAction action = undoStack.Pop();
			try {
				action.Undo();
				redoStack.Push(action);
				OnUndoStackChanged(action, OperationKind.Undo);
			}
			catch {
				// state might be invalid now, clear stacks to prevent getting more inconsistencies
				Clear();
				throw;
			}
		}

		/// <summary>
		/// Gets the list of names of the available actions on the undo stack.
		/// </summary>
		public IEnumerable<IUndoAction> UndoActions
		{
			get
			{
				return undoStack;
			}
		}

		/// <summary>
		/// Gets the list of names of the available actions on the undo stack.
		/// </summary>
		public IEnumerable<IUndoAction> RedoActions
		{
			get
			{
				return redoStack;
			}
		}

		/// <summary>
		/// Gets if there are redo actions available.
		/// </summary>
		public bool CanRedo { get { return redoStack.Count > 0; } }

		/// <summary>
		/// Redoes a previously undone action.
		/// </summary>
		public void Redo()
		{
			if (!CanRedo)
				throw new InvalidOperationException("Cannot Redo: redo stack is empty");
			if (transactionStack.Count != 0)
				throw new InvalidOperationException("Cannot Redo while transaction is running");
			IUndoAction action = redoStack.Pop();
			try {
				action.Do();
				undoStack.Push(action);
				OnUndoStackChanged(action, OperationKind.Redo);
			}
			catch {
				// state might be invalid now, clear stacks to prevent getting more inconsistencies
				Clear();
				throw;
			}
		}

		/// <summary>
		/// Clears saved actions (both undo and redo stack).
		/// </summary>
		public void Clear()
		{
			undoStack.Clear();
			redoStack.Clear();
			OnUndoStackChanged(null, OperationKind.Clear);
		}

		enum OperationKind
		{
			Execute, Undo, Redo, Clear
		}
	}
}
