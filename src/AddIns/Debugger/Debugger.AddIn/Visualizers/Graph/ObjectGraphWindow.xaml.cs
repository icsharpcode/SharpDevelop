// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
			
			debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null) throw new DebuggerVisualizerException("Only windows debugger is currently supported");
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			UnregisterDebuggerEvents();
			ObjectGraphWindow.Instance = null;			// allow release
		}
		
		public ICSharpCode.NRefactory.Ast.Expression ShownExpression
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
