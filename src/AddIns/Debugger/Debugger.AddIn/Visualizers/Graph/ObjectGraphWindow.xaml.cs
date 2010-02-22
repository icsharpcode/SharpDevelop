// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		
		public ObjectGraphWindow()
		{
			InitializeComponent();
			
			debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
			
			registerEvents();
			instance = this;
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			unregisterEvents();
			this.objectGraphControl.ClearUIControlCache();
			instance = null;
		}
		
		public ICSharpCode.NRefactory.Ast.Expression ShownExpression
		{
			get {
				return this.objectGraphControl.ShownExpression;
			}
			set {
				this.objectGraphControl.ShownExpression = value;
			}
		}
		
		static ObjectGraphWindow instance;
		/// <summary> When Window is visible, returns reference to the Window. Otherwise returns null. </summary>
		public static ObjectGraphWindow Instance
		{
			get { return instance; }
		}
		
		public static ObjectGraphWindow EnsureShown()
		{
			var window = ObjectGraphWindow.Instance ?? new ObjectGraphWindow();
			window.Topmost = true;
			window.Show();
			return window;
		}
		
		private void registerEvents()
		{
			debuggerService.IsProcessRunningChanged += new EventHandler(debuggerService_IsProcessRunningChanged);
			debuggerService.DebugStopped += new EventHandler(debuggerService_DebugStopped);
		}
		
		private void unregisterEvents()
		{
			debuggerService.IsProcessRunningChanged -= new EventHandler(debuggerService_IsProcessRunningChanged);
			debuggerService.DebugStopped -= new EventHandler(debuggerService_DebugStopped);
		}
		
		public void debuggerService_IsProcessRunningChanged(object sender, EventArgs e)
		{
			// on step or breakpoint hit
			if (!debuggerService.IsProcessRunning)
			{
				this.objectGraphControl.Refresh();
			}
		}
		
		public void debuggerService_DebugStopped(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
