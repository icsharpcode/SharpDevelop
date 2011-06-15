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

using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.Graph.Layout;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;
using Log = ICSharpCode.Core.LoggingService;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Interaction logic for ObjectGraphControl.xaml
	/// </summary>
	public partial class ObjectGraphControl : UserControl
	{
		WindowsDebugger debuggerService;
		EnumViewModel<LayoutDirection> layoutViewModel;
		ObjectGraph objectGraph;
		ObjectGraphBuilder objectGraphBuilder;
		
		PositionedGraph oldPosGraph;
		PositionedGraph currentPosGraph;
		GraphDrawer graphDrawer;
		
		static double mouseWheelZoomSpeed = 0.05;
		
		/// <summary> Long-lived map telling which graph nodes and content nodes the user expanded. </summary>
		static Expanded expanded = new Expanded();

		public ObjectGraphControl()
		{
			InitializeComponent();
			
			debuggerService = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (debuggerService == null) throw new ApplicationException("Only windows debugger is currently supported");
			
			this.layoutViewModel = new EnumViewModel<LayoutDirection>();
			this.layoutViewModel.PropertyChanged += new PropertyChangedEventHandler(layoutViewModel_PropertyChanged);
			this.cmbLayoutDirection.DataContext = this.layoutViewModel;
			
			this.graphDrawer = new GraphDrawer(this.canvas);
		}
		
		public void Clear()
		{
			txtExpression.Text = string.Empty;
		}
		
		public void Refresh()
		{
			// Almost all of the blocking is done ContentPropertyNode.Evaluate,
			// which is being called by WPF for all the properties.
			// It would be better if we were filling the node texts ourselves in a loop,
			// so that we could cancel any time. UI would redraw gradually thanks to INotifyPropertyChanged.
			try {
				Debugger.AddIn.TreeModel.Utils.DoEvents(debuggerService.DebuggedProcess);
			} catch(AbortedBecauseDebuggeeResumedException) { 
				Log.Warn("Object graph - debuggee resumed, cancelling refresh.");
				this.graphDrawer.ClearCanvas();
				return;
			}
			
			ClearErrorMessage();
			if (string.IsNullOrEmpty(txtExpression.Text)) {
				this.graphDrawer.ClearCanvas();
				return;
			}
			if (debuggerService.IsProcessRunning) {
				// "Process not paused" exception still occurs
				ErrorMessage("Cannot inspect when the process is running.");
				return;
			}
			bool isSuccess = true;
			try	{
				this.objectGraph = RebuildGraph(txtExpression.Text);
			} catch(DebuggerVisualizerException ex)	{
				isSuccess = false;
				ErrorMessage(ex.Message);
			} catch(Debugger.GetValueException ex)	{
				isSuccess = false;
				ErrorMessage("Expression cannot be evaluated - " + ex.Message);
			}
			
			if (isSuccess) {
				LayoutGraph(this.objectGraph);
			} else {
				this.graphDrawer.ClearCanvas();
			}
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
		
		private void Inspect_Button_Click(object sender, RoutedEventArgs e)
		{
			this.Refresh();
		}
		
		ObjectGraph RebuildGraph(string expression)
		{
			this.objectGraphBuilder = new ObjectGraphBuilder(debuggerService);
			Log.Debug("Debugger visualizer: Building graph for expression: " + txtExpression.Text);
			return this.objectGraphBuilder.BuildGraphForExpression(expression, expanded.Expressions);
		}
		
		void LayoutGraph(ObjectGraph graph)
		{
			this.oldPosGraph = this.currentPosGraph;
			Log.Debug("Debugger visualizer: Calculating graph layout");
			var layoutDirection = layoutViewModel.SelectedEnumValue;
			this.currentPosGraph = new TreeLayout(layoutDirection).CalculateLayout(graph, expanded);
			Log.Debug("Debugger visualizer: Graph layout done");
			RegisterExpandCollapseEvents(this.currentPosGraph);
			
			var graphDiff = new GraphMatcher().MatchGraphs(oldPosGraph, currentPosGraph);
			Log.Debug("Debugger visualizer: starting graph animation");
			this.graphDrawer.StartAnimation(oldPosGraph, currentPosGraph, graphDiff);
		}
		
		void layoutViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "SelectedEnumValue") {
				if (this.objectGraph != null) {
					LayoutGraph(this.objectGraph);
				}
			}
		}
		
		void ClearErrorMessage()
		{
			this.pnlError.Visibility = Visibility.Collapsed;
		}
		
		void ErrorMessage(string message)
		{
			this.txtError.Text = message;
			this.pnlError.Visibility = Visibility.Visible;
			//MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		
		void RegisterExpandCollapseEvents(PositionedGraph posGraph)
		{
			foreach (var node in posGraph.Nodes) {
				node.PropertyExpanded += new EventHandler<PositionedPropertyEventArgs>(node_PropertyExpanded);
				node.PropertyCollapsed += new EventHandler<PositionedPropertyEventArgs>(node_PropertyCollapsed);
				node.ContentNodeExpanded += new EventHandler<ContentNodeEventArgs>(node_ContentNodeExpanded);
				node.ContentNodeCollapsed += new EventHandler<ContentNodeEventArgs>(node_ContentNodeCollapsed);
			}
		}
		
		void node_ContentNodeExpanded(object sender, ContentNodeEventArgs e)
		{
			expanded.ContentNodes.SetExpanded(e.Node);
			LayoutGraph(this.objectGraph);
		}

		void node_ContentNodeCollapsed(object sender, ContentNodeEventArgs e)
		{
			expanded.ContentNodes.SetCollapsed(e.Node);
			LayoutGraph(this.objectGraph);
		}

		void node_PropertyExpanded(object sender, PositionedPropertyEventArgs e)
		{
			// remember this property is expanded (for later graph rebuilds)
			expanded.Expressions.SetExpanded(e.Property.Expression);
			
			// add edge (+ possibly nodes) to underlying object graph (no need to fully rebuild)
			// TODO can add more nodes if they are expanded - now this adds always one node
			e.Property.ObjectGraphProperty.TargetNode = this.objectGraphBuilder.ObtainNodeForExpression(e.Property.Expression);
			LayoutGraph(this.objectGraph);
		}
		
		void node_PropertyCollapsed(object sender, PositionedPropertyEventArgs e)
		{
			// remember this property is collapsed (for later graph rebuilds)
			expanded.Expressions.SetCollapsed(e.Property.Expression);
			
			// just remove edge from underlying object graph (no need to fully rebuild)
			e.Property.ObjectGraphProperty.TargetNode = null;
			LayoutGraph(this.objectGraph);
		}
		
		void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
				zoomSlider.Value += e.Delta > 0 ? mouseWheelZoomSpeed : -mouseWheelZoomSpeed;
			}
		}
	}
}
