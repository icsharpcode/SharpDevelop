// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.UndoRedo
{
	public class FormsDesignerUndoEngine : UndoEngine, IUndoHandler
	{		
		Stack<UndoEngine.UndoUnit> undoStack = new Stack<UndoEngine.UndoUnit>();
		Stack<UndoEngine.UndoUnit> redoStack = new Stack<UndoEngine.UndoUnit>();
		
		public FormsDesignerUndoEngine(IServiceProvider provider) : base(provider)
		{
		}
		
		#region IUndoHandler
		public bool EnableUndo {
			get {
				return undoStack.Count > 0;
			}
		}
		
		public bool EnableRedo {
			get {
				return redoStack.Count > 0;
			}
		}		

		public void Undo()
		{
			if (undoStack.Count > 0) {
				UndoEngine.UndoUnit unit = undoStack.Pop();
				unit.Undo();
				redoStack.Push(unit);
			}
		}
		
		public void Redo()
		{
			if (redoStack.Count > 0) {
				UndoEngine.UndoUnit unit = redoStack.Pop();
				unit.Undo();
				undoStack.Push(unit);
			}
		}
		#endregion
		
		protected override void AddUndoUnit(UndoEngine.UndoUnit unit)
		{
			undoStack.Push(unit);
		}
	}
}
