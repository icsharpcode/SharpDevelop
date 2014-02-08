// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ICSharpCode.SharpDevelop;
using Debugger.AddIn.Visualizers.Graph.Layout;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Interaction logic for VisualizerWPFWindow.xaml
	/// </summary>
	public partial class ObjectGraphWindow : Window
	{
		private WindowsDebugger debuggerService;
		
		private ObjectGraphWindow()
		{
			InitializeComponent();
			
			debuggerService = SD.Debugger as WindowsDebugger;
			if (debuggerService == null) throw new DebuggerVisualizerException("Only windows debugger is currently supported");
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			UnregisterDebuggerEvents();
			ObjectGraphWindow.Instance = null;			// allow release
		}
		
		public GraphExpression ShownExpression
		{
			get { return this.objectGraphControl.ShownExpression; }
			set { this.objectGraphControl.ShownExpression = value; }
		}
		
		/// <summary> When Window is visible, returns reference to the Window. Otherwise returns null. </summary>
		public static ObjectGraphWindow Instance { get; private set; }
		
		/// <summary>
		/// Shows the singleton instance of ObjectGraphWindow and also returns it.
		/// </summary>
		/// <returns></returns>
		public static ObjectGraphWindow EnsureShown()
		{
			if (ObjectGraphWindow.Instance == null) {
				ObjectGraphWindow.Instance = new ObjectGraphWindow();
				ObjectGraphWindow.Instance.Topmost = true;
			}
			ObjectGraphWindow.Instance.RegisterDebuggerEvents();
			ObjectGraphWindow.Instance.Show();
			return ObjectGraphWindow.Instance;
		}
		
		static bool isDebuggerEventsRegistered = false;
		
		void RegisterDebuggerEvents()
		{
			if (!isDebuggerEventsRegistered) {
				debuggerService.IsProcessRunningChanged += DebuggerService_IsProcessRunningChanged;
				debuggerService.DebugStopped += DebuggerService_DebugStopped;
				// cannot use debuggerService.IsProcessRunningChanged.GetInvocationList() from outside
				isDebuggerEventsRegistered = true;
			}
		}
		
		void UnregisterDebuggerEvents()
		{
			debuggerService.IsProcessRunningChanged -= DebuggerService_IsProcessRunningChanged;
			debuggerService.DebugStopped -= DebuggerService_DebugStopped;
			isDebuggerEventsRegistered = false;
		}
		
		void DebuggerService_IsProcessRunningChanged(object sender, EventArgs e)
		{
			// on step or breakpoint hit
			if (!debuggerService.IsProcessRunning) {
				this.objectGraphControl.RefreshView();
			}
		}
		
		void DebuggerService_DebugStopped(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
