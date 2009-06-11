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
    public partial class VisualizerWPFWindow : Window
    {
    	private WindowsDebugger debuggerService;
    	private EnumViewModel<LayoutDirection> layoutViewModel;
    	private ObjectGraph objectGraph;
    	
    	private PositionedGraph oldGraph;
    	private PositionedGraph currentGraph;
    	private GraphDrawer graphDrawer;
    	
    	/// <summary>
    	/// Long-lived map telling which nodes the user expanded.
    	/// </summary>
    	private ExpandedNodes expandedNodes;

        public VisualizerWPFWindow()
        {
            InitializeComponent();
            
        	debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
				
			debuggerService.IsProcessRunningChanged += new EventHandler(debuggerService_IsProcessRunningChanged);
			debuggerService.DebugStopped += new EventHandler(_debuggerService_DebugStopped);
			
			this.layoutViewModel = new EnumViewModel<LayoutDirection>();
            this.layoutViewModel.PropertyChanged += new PropertyChangedEventHandler(layoutViewModel_PropertyChanged);
            this.DataContext = this.layoutViewModel;
            
            this.graphDrawer = new GraphDrawer(this.canvas);
            
            this.expandedNodes = new ExpandedNodes();
		}
		
		public void debuggerService_IsProcessRunningChanged(object sender, EventArgs e)
		{
			// on step or breakpoint hit
			if (!debuggerService.IsProcessRunning)
			{
				refreshGraph();
			}
		}
		
		public void _debuggerService_DebugStopped(object sender, EventArgs e)
		{
			debuggerService.IsProcessRunningChanged -= new EventHandler(debuggerService_IsProcessRunningChanged);
			this.Close();
		}
        
        private void Inspect_Button_Click(object sender, RoutedEventArgs e)
        {
			refreshGraph();
        }
        
        void layoutViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        	if (e.PropertyName == "SelectedEnumValue")	// TODO should be special event for enum value change
        	{
        		layoutGraph(this.objectGraph);
        	}
        }
        
        void refreshGraph()
		{
			ObjectGraphBuilder graphBuilder = new ObjectGraphBuilder(debuggerService);
			this.objectGraph = null;
			
			try
			{	
				ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Building graph for expression: " + txtExpression.Text);
				this.objectGraph = graphBuilder.BuildGraphForExpression(txtExpression.Text, expandedNodes);
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
			
			layoutGraph(this.objectGraph);
				
			//GraphDrawer drawer = new GraphDrawer(graph);
			//drawer.Draw(canvas);
		}
        
        void layoutGraph(ObjectGraph graph)
        {
        	ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Calculating graph layout");
        	Layout.TreeLayouter layouter = new Layout.TreeLayouter();
        	
            this.oldGraph = this.currentGraph;
			this.currentGraph = layouter.CalculateLayout(graph, layoutViewModel.SelectedEnumValue, this.expandedNodes);
			registerExpandCollapseEvents(this.currentGraph);
			
			var graphDiff = new GraphMatcher().MatchGraphs(oldGraph, currentGraph);
			this.graphDrawer.StartAnimation(oldGraph, currentGraph, graphDiff);
			//this.graphDrawer.Draw(currentGraph);
        }
		
		void guiHandleException(System.Exception ex)
		{
			MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		
		void registerExpandCollapseEvents(PositionedGraph posGraph)
		{
			foreach (var node in posGraph.Nodes)
			{
				node.Expanded += new EventHandler<PositionedPropertyEventArgs>(node_Expanded);
				node.Collapsed += new EventHandler<PositionedPropertyEventArgs>(node_Collapsed);
			}
		}

		void node_Expanded(object sender, PositionedPropertyEventArgs e)
		{
			expandedNodes.Expand(e.Property.Expression.Code);
			refreshGraph();
		}
		
		void node_Collapsed(object sender, PositionedPropertyEventArgs e)
		{
			expandedNodes.Collapse(e.Property.Expression.Code);
			refreshGraph();
		}
    }
}