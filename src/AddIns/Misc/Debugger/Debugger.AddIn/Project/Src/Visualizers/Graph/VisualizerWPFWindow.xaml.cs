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
		private Layout.TreeLayouter layouter;
		
		/// <summary>
		/// Long-lived map telling which graph nodes and content nodes the user expanded.
		/// </summary>
		private Expanded expanded = new Expanded();

		public VisualizerWPFWindow()
		{
			InitializeComponent();
			
			debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
			
			this.Closed += new EventHandler(VisualizerWPFWindow_Closed);
			
			registerEvents();
			
			this.layoutViewModel = new EnumViewModel<LayoutDirection>();
			this.layoutViewModel.PropertyChanged += new PropertyChangedEventHandler(layoutViewModel_PropertyChanged);
			this.DataContext = this.layoutViewModel;
			
			this.layouter = new TreeLayouter();
			this.graphDrawer = new GraphDrawer(this.canvas);
		}

		void VisualizerWPFWindow_Closed(object sender, EventArgs e)
		{
			unregisterEvents();
			NodeControlCache.Instance.Clear();
		}
		
		private void registerEvents()
		{
			debuggerService.IsProcessRunningChanged += new EventHandler(debuggerService_IsProcessRunningChanged);
			debuggerService.DebugStopped += new EventHandler(_debuggerService_DebugStopped);
		}
		
		private void unregisterEvents()
		{
			debuggerService.IsProcessRunningChanged -= new EventHandler(debuggerService_IsProcessRunningChanged);
			debuggerService.DebugStopped -= new EventHandler(_debuggerService_DebugStopped);
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
			this.Close();
		}
		
		private void Inspect_Button_Click(object sender, RoutedEventArgs e)
		{
			clearErrorMessage();
			if (debuggerService.IsProcessRunning)		// TODO "Process not paused" exception still occurs
			{
				showErrorMessage("Cannot inspect when the process is running.");
			}
			else
			{
				refreshGraph();
			}
		}
		
		void layoutViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SelectedEnumValue")	// TODO special event for enum value change
			{
				if (this.objectGraph != null)
				{
					layoutGraph(this.objectGraph);
				}
			}
		}
		
		void refreshGraph()
		{
			bool graphBuiltOk = true;
			clearErrorMessage();
			try
			{
				this.objectGraph = rebuildGraph();
			}
			catch(DebuggerVisualizerException ex)
			{
				graphBuiltOk = false;
				showErrorMessage(ex.Message);
			}
			catch(Debugger.GetValueException ex)
			{
				graphBuiltOk = false;
				showErrorMessage(ex.Message);
			}
			if (graphBuiltOk)
			{
				layoutGraph(this.objectGraph);
			}
		}
		
		ObjectGraph rebuildGraph()
		{
			this.objectGraphBuilder = new ObjectGraphBuilder(debuggerService);
			ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Building graph for expression: " + txtExpression.Text);
			return this.objectGraphBuilder.BuildGraphForExpression(txtExpression.Text, this.expanded.Expressions);
		}
		
		void layoutGraph(ObjectGraph graph)
		{
			if (this.oldPosGraph != null)
			{
				foreach (var oldNode in this.oldPosGraph.Nodes)
				{
					// controls from old graph would be garbage collected, reuse them
					NodeControlCache.Instance.ReturnForReuse(oldNode.NodeVisualControl);
				}
			}
			this.oldPosGraph = this.currentPosGraph;
			ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Calculating graph layout");
			this.currentPosGraph = this.layouter.CalculateLayout(graph, layoutViewModel.SelectedEnumValue, this.expanded);
			ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Graph layout done");
			registerExpandCollapseEvents(this.currentPosGraph);
			
			var graphDiff = new GraphMatcher().MatchGraphs(oldPosGraph, currentPosGraph);
			ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: starting graph animation");
			this.graphDrawer.StartAnimation(oldPosGraph, currentPosGraph, graphDiff);
			//this.graphDrawer.Draw(this.currentPosGraph);
		}
		
		void clearErrorMessage()
		{
			this.pnlError.Visibility = Visibility.Collapsed;
		}
		
		void showErrorMessage(string message)
		{
			this.txtError.Text = message;
			this.pnlError.Visibility = Visibility.Visible;
			//MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		
		void registerExpandCollapseEvents(PositionedGraph posGraph)
		{
			foreach (var node in posGraph.Nodes)
			{
				node.PropertyExpanded += new EventHandler<PositionedPropertyEventArgs>(node_PropertyExpanded);
				node.PropertyCollapsed += new EventHandler<PositionedPropertyEventArgs>(node_PropertyCollapsed);
				node.ContentNodeExpanded += new EventHandler<ContentNodeEventArgs>(node_ContentNodeExpanded);
				node.ContentNodeCollapsed += new EventHandler<ContentNodeEventArgs>(node_ContentNodeCollapsed);
			}
		}
		
		void node_ContentNodeExpanded(object sender, ContentNodeEventArgs e)
		{
			expanded.ContentNodes.SetExpanded(e.Node);
			layoutGraph(this.objectGraph);
		}

		void node_ContentNodeCollapsed(object sender, ContentNodeEventArgs e)
		{
			expanded.ContentNodes.SetCollapsed(e.Node);
			layoutGraph(this.objectGraph);
		}

		void node_PropertyExpanded(object sender, PositionedPropertyEventArgs e)
		{
			// remember this property is expanded (for later graph rebuilds)
			expanded.Expressions.SetExpanded(e.Property.Expression);
			
			// add edge (+ possibly nodes) to underlying object graph (no need to fully rebuild)
			// TODO can add more nodes if they are expanded - now this adds always one node
			e.Property.ObjectGraphProperty.TargetNode = this.objectGraphBuilder.ObtainNodeForExpression(e.Property.Expression);
			layoutGraph(this.objectGraph);
		}
		
		void node_PropertyCollapsed(object sender, PositionedPropertyEventArgs e)
		{
			// remember this property is collapsed (for later graph rebuilds)
			expanded.Expressions.SetCollapsed(e.Property.Expression);
			
			// just remove edge from underlying object graph (no need to fully rebuild)
			e.Property.ObjectGraphProperty.TargetNode = null;
			layoutGraph(this.objectGraph);
		}
	}
}