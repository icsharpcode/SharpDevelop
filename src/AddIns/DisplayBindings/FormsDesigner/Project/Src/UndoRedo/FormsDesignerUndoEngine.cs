// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace ICSharpCode.FormsDesigner.UndoRedo
{
	public class FormsDesignerUndoEngine : UndoEngine, IFormsDesignerUndoEngine
	{
		Stack<UndoEngine.UndoUnit> undoStack = new Stack<UndoEngine.UndoUnit>();
		Stack<UndoEngine.UndoUnit> redoStack = new Stack<UndoEngine.UndoUnit>();

		public FormsDesignerUndoEngine(IServiceProvider provider) : base(provider)
		{
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain);
		}

		public bool EnableUndo {
			get { return undoStack.Count > 0; }
		}

		public bool EnableRedo {
			get { return redoStack.Count > 0; }
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

		protected override void AddUndoUnit(UndoEngine.UndoUnit unit)
		{
			undoStack.Push(unit);
		}
	}
	
	public class FormsDesignerUndoEngineProxy : MarshalByRefObject, IFormsDesignerUndoEngine
	{
		FormsDesignerUndoEngine engine;
		
		public FormsDesignerUndoEngineProxy(FormsDesignerUndoEngine engine)
		{
			this.engine = engine;
		}
		
		public bool EnableUndo {
			get {
				return engine.EnableUndo;
			}
		}
		
		public bool EnableRedo {
			get {
				return engine.EnableRedo;
			}
		}
		
		public void Undo()
		{
			engine.Undo();
		}
		
		public void Redo()
		{
			engine.Redo();
		}
	}
}
