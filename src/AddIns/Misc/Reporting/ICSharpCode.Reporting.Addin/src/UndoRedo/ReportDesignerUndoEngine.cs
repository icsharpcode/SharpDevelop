/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.05.2014
 * Time: 19:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;

namespace ICSharpCode.Reporting.Addin.UndoRedo
{
	/// <summary>
	/// Description of ReportDesignerUndoEngine.
	/// </summary>
	public class ReportDesignerUndoEngine : UndoEngine, IUndoHandler
	{		
		Stack<UndoEngine.UndoUnit> undoStack = new Stack<UndoEngine.UndoUnit>();
		Stack<UndoEngine.UndoUnit> redoStack = new Stack<UndoEngine.UndoUnit>();
		
		public ReportDesignerUndoEngine(IServiceProvider provider) : base(provider)
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
