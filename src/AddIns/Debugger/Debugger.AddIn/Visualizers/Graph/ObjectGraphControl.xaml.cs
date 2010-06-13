// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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

using Debugger.AddIn.Visualizers.Graph.Layout;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Interaction logic for ObjectGraphControl.xaml
	/// </summary>
	public partial class ObjectGraphControl : UserControl
	{
		private WindowsDebugger debuggerService;
		private EnumViewModel<LayoutDirection> layoutViewModel;
		private ObjectGraph objectGraph;
		private ObjectGraphBuilder objectGraphBuilder;
		
		private PositionedGraph oldPosGraph;
		private PositionedGraph currentPosGraph;
		private GraphDrawer graphDrawer;
		private Layout.TreeLayouter layouter;
		
		/// <summary> Long-lived map telling which graph nodes and content nodes the user expanded. </summary>
		private Expanded expanded = new Expanded();

		public ObjectGraphControl()
		{
			InitializeComponent();
			
			debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null)
				throw new ApplicationException("Only windows debugger is currently supported");
			
			this.layoutViewModel = new EnumViewModel<LayoutDirection>();
			this.layoutViewModel.PropertyChanged += new PropertyChangedEventHandler(layoutViewModel_PropertyChanged);
			this.cmbLayoutDirection.DataContext = this.layoutViewModel;
			
			this.layouter = new TreeLayouter();
			this.graphDrawer = new GraphDrawer(this.canvas);
		}
		
		public void Clear()
		{
			txtExpression.Text = string.Empty;
		}
		
		public void Refresh()
		{
			refreshGraph();
		}
		
		private ICSharpCode.NRefactory.Ast.Expression shownExpression;
		public ICSharpCode.NRefactory.Ast.Expression ShownExpression
		{
			get {
				return shownExpression;
			}
			set {
				if (value == null) {
					shownExpression = null;
					txtExpression.Text = null;
					Refresh();
					return;
				}
				if (shownExpression == null || value.PrettyPrint() != shownExpression.PrettyPrint()) {
					txtExpression.Text = value.PrettyPrint();
					Refresh();
				}
			}
		}
		
		/// <summary>
		/// ObjectGraph visualizer caches UI controls, this clears the cache.
		/// </summary>
		public void ClearUIControlCache()
		{
			NodeControlCache.Instance.Clear();
		}
		
		private void Inspect_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Refresh();
		}
		
		void refreshGraph()
		{
			clearErrorMessage();
			if (string.IsNullOrEmpty(txtExpression.Text))
			{
				this.graphDrawer.ClearCanvas();
				return;
			}
			if (debuggerService.IsProcessRunning)		// "Process not paused" exception still occurs
			{
				showErrorMessage("Cannot inspect when the process is running.");
				return;
			}
			bool graphBuiltOk = true;
			try
			{
				this.objectGraph = rebuildGraph(txtExpression.Text);
			}
			catch(DebuggerVisualizerException ex)
			{
				graphBuiltOk = false;
				showErrorMessage(ex.Message);
			}
			catch(Debugger.GetValueException ex)
			{
				graphBuiltOk = false;
				showErrorMessage("Expression cannot be evaluated - " + ex.Message);
			}
			if (graphBuiltOk)
			{
				layoutGraph(this.objectGraph);
			}
			else
			{
				this.graphDrawer.ClearCanvas();
			}
		}
		
		ObjectGraph rebuildGraph(string expression)
		{
			this.objectGraphBuilder = new ObjectGraphBuilder(debuggerService);
			ICSharpCode.Core.LoggingService.Debug("Debugger visualizer: Building graph for expression: " + txtExpression.Text);
			return this.objectGraphBuilder.BuildGraphForExpression(expression, this.expanded.Expressions);
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
			//this.graphDrawer.Draw(this.currentPosGraph);	// buggy layout with NodeControlCache
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
