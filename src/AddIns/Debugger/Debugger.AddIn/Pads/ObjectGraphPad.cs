// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Debugger;
using Debugger.AddIn.Visualizers.Graph;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Description of ObjectGraphPad.
	/// </summary>
	public class ObjectGraphPad : DebuggerPad
	{
		Process debuggedProcess;
		ObjectGraphControl objectGraphControl;
		static ObjectGraphPad instance;
		
		public ObjectGraphPad()
		{
			instance = this;
		}
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static ObjectGraphPad Instance {
			get { return instance; }
		}
		
		protected override void InitializeComponents()
		{
			objectGraphControl = new ObjectGraphControl();
		}
		
		public override object Control {
			get {
				return objectGraphControl;
			}
		}
		
		public override void RefreshPad()
		{
			// BUG: if pad window is undocked and floats standalone, IsVisible == false (so pad won't refresh)
			// REQUEST: need to refresh when pad becomes visible -> VisibleChanged event?
			if (!this.IsVisible)
			{
				return;
			}
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null) {
				this.objectGraphControl.Clear();
				return;
			}
			this.objectGraphControl.Refresh();
		}
		
		protected override void SelectProcess(Process process)
		{
			if (debuggedProcess != null) {
				debuggedProcess.Paused -= debuggedProcess_Paused;
			}
			debuggedProcess = process;
			if (debuggedProcess != null) {
				debuggedProcess.Paused += debuggedProcess_Paused;
			}
			RefreshPad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			RefreshPad();
		}
	}
}
