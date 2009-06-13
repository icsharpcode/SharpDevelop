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
    	private ObjectGraphBuilder objectGraphBuilder;
    	
    	private PositionedGraph oldPosGraph;
    	private PositionedGraph currentPosGraph;
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
        	this.objectGraph = rebuildGraph();
			layoutGraph(this.objectGraph);	
			//GraphDrawer drawer = new GraphDrawer(graph);
			//drawer.Draw(canvas);
		}
        
        ObjectGraph rebuildGraph()
        {
        	this.objectGraphBuilder = new ObjectGraphBuilder(debuggerService);
			try
			{	
				ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Building graph for expression: " + txtExpression.Text);
				return this.objectGraphBuilder.BuildGraphForExpression(txtExpression.Text, this.expandedNodes);
			}
			catch(DebuggerVisualizerException ex)
			{
				guiHandleException(ex);
				return null;
			}
			catch(Debugger.GetValueException ex)
			{
				guiHandleException(ex);
				return null;
			}
        }
        
        void layoutGraph(ObjectGraph graph)
        {
        	ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Calculating graph layout");
        	Layout.TreeLayouter layouter = new Layout.TreeLayouter();
        	
            this.oldPosGraph = this.currentPosGraph;
			this.currentPosGraph = layouter.CalculateLayout(graph, layoutViewModel.SelectedEnumValue, this.expandedNodes);
			registerExpandCollapseEvents(this.currentPosGraph);
			
			var graphDiff = new GraphMatcher().MatchGraphs(oldPosGraph, currentPosGraph);
			this.graphDrawer.StartAnimation(oldPosGraph, currentPosGraph, graphDiff);
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
			// remember this property is expanded (for later graph rebuilds)
			expandedNodes.SetExpanded(e.Property.Expression.Code);
			
			// add edge (+ possibly node) to underlying object graph (no need to rebuild)
			e.Property.ObjectProperty.TargetNode = this.objectGraphBuilder.ObtainNodeForExpression(e.Property.Expression);
			layoutGraph(this.objectGraph);
		}
		
		void node_Collapsed(object sender, PositionedPropertyEventArgs e)
		{
			// remember this property is collapsed (for later graph rebuilds)
			expandedNodes.SetCollapsed(e.Property.Expression.Code);
			
			// just remove edge from underlying object graph (no need to rebuild)
			e.Property.ObjectProperty.TargetNode = null;
			layoutGraph(this.objectGraph);
		}
    }
}