// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers.Graph
{
    /// <summary>
    /// Interaction logic for VisualizerWPFWindow.xaml
    /// </summary>
    public partial class VisualizerWPFWindow : Window
    {
    	WindowsDebugger _debuggerService = null;

        public VisualizerWPFWindow()
        {
            InitializeComponent();
            
        	_debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (_debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
				
			_debuggerService.IsProcessRunningChanged += new EventHandler(debuggerService_IsProcessRunningChanged);
			_debuggerService.DebugStopped += new EventHandler(_debuggerService_DebugStopped);
		}
		
		public void debuggerService_IsProcessRunningChanged(object sender, EventArgs e)
		{
			// on step or breakpoint hit
			if (!_debuggerService.IsProcessRunning)
			{
				refreshGraph();
			}
		}
		
		public void _debuggerService_DebugStopped(object sender, EventArgs e)
		{
			_debuggerService.IsProcessRunningChanged -= new EventHandler(debuggerService_IsProcessRunningChanged);
			this.Close();
		}
        
        private void Inspect_Button_Click(object sender, RoutedEventArgs e)
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
			
			Layout.TreeLayouter layouter = new Layout.TreeLayouter();
			Layout.PositionedGraph posGraph = layouter.CalculateLayout(graph, Layout.LayoutDirection.LeftRight);
			
			GraphDrawer.Draw(posGraph, canvas);
				
			//GraphDrawer drawer = new GraphDrawer(graph);
			//drawer.Draw(canvas);
		}
		
		void guiHandleException(System.Exception ex)
		{
			MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}
    }
}