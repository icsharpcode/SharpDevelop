// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Windows forms control making up the object graph visualizer user interface/
	/// </summary>
	public partial class VisualizerWinFormsControl : UserControl
	{
		WindowsDebugger _debuggerService = null;
		
		public VisualizerWinFormsControl()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			_debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (_debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
				
			_debuggerService.IsProcessRunningChanged += new EventHandler(debuggerService_IsProcessRunningChanged);
		}
		
		public void debuggerService_IsProcessRunningChanged(object sender, EventArgs e)
		{
			// on step, breakpoint hit
			if (!_debuggerService.IsProcessRunning)
			{
				refreshGraph();
			}
		}
		
		void BtnInspectClick(object sender, EventArgs e)
		{
			refreshGraph();
		}
		
		void refreshGraph()
		{
			ObjectGraphBuilder graphBuilder = new ObjectGraphBuilder(_debuggerService);
			ObjectGraph graph = null;
			
			try
			{	
				graph = graphBuilder.BuildGraphForExpression(txtExpression.Text);
			}
			catch(DebuggerVisualizerException ex)
			{
				guiHandleException(ex);
				return;
			}
			catch(Debugger.GetValueException ex)
			{
				guiHandleException(ex);
				
				return;
			}
				
			// just a simple message for checking the graph is build ok, will be replaced by graph drawing of course
			lblInfo.Text = string.Format("Done. Number of graph nodes: {0}, number of edges: {1}", graph.Nodes.Count(), graph.Edges.Count());
		}
		
		void guiHandleException(System.Exception ex)
		{
			lblInfo.Text = "< Result >";
			MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
