// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
			panel.Children.Add(objectGraphControl);
		}
		
		
		protected override void RefreshPad()
		{
			// BUG: if pad window is undocked and floats standalone, IsVisible == false (so pad won't refresh)
			// REQUEST: need to refresh when pad becomes visible -> VisibleChanged event?
			if (!objectGraphControl.IsVisible)
			{
				return;
			}
			if (debuggedProcess == null || debuggedProcess.IsRunning || debuggedProcess.SelectedStackFrame == null) {
				this.objectGraphControl.Clear();
				return;
			}
			this.objectGraphControl.RefreshView();
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
			InvalidatePad();
		}
		
		void debuggedProcess_Paused(object sender, ProcessEventArgs e)
		{
			InvalidatePad();
		}
	}
}
