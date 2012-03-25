// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Windows.Controls;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Debugger;
using Debugger.AddIn.Visualizers.Graph;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Description of ObjectGraphPad.
	/// </summary>
	public class ObjectGraphPad : AbstractPadContent
	{
		DockPanel panel;
		ObjectGraphControl objectGraphControl;
		static ObjectGraphPad instance;
		
		public override object Control {
			get { return panel; }
		}
		
		public ObjectGraphPad()
		{
			this.panel = new DockPanel();
			instance = this;
			
			objectGraphControl = new ObjectGraphControl();
			panel.Children.Add(objectGraphControl);
			
			WindowsDebugger.RefreshingPads += RefreshPad;
			RefreshPad();
		}
		
		/// <remarks>Always check if Instance is null, might be null if pad is not opened!</remarks>
		public static ObjectGraphPad Instance {
			get { return instance; }
		}
		
		void RefreshPad()
		{
			// BUG: if pad window is undocked and floats standalone, IsVisible == false (so pad won't refresh)
			// REQUEST: need to refresh when pad becomes visible -> VisibleChanged event?
			if (!objectGraphControl.IsVisible) {
				return;
			}
			if (WindowsDebugger.CurrentStackFrame == null) {
				this.objectGraphControl.Clear();
			} else {
				this.objectGraphControl.RefreshView();
			}
		}
	}
}
