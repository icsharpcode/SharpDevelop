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

using ICSharpCode.SharpDevelop;
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
		
		/// <summary> Tells which graph nodes and content nodes the user expanded. </summary>
		static Expanded expanded = new Expanded();

		public ObjectGraphControl()
		{
			InitializeComponent();
			
			debuggerService = SD.Debugger as WindowsDebugger;
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
		
		public void RefreshView()
		{
			WindowsDebugger.CurrentProcess.EnqueueWork(Dispatcher, () => Refresh());
		}
		
		void Refresh()
		{
			ClearErrorMessage();
			if (string.IsNullOrEmpty(txtExpression.Text)) {
				this.graphDrawer.ClearCanvas();
				return;
			}
			if (debuggerService.IsProcessRunning) {
				ErrorMessage("Cannot inspect when the process is running.");
				return;
			}
			bool isSuccess = true;
			try	{
				this.objectGraph = RebuildGraph();
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
		
		private GraphExpression shownExpression;
		public GraphExpression ShownExpression
		{
			get {
				return shownExpression;
			}
			set {
				if (value == null) {
					shownExpression = null;
					txtExpression.Text = null;
					RefreshView();
					return;
				}
				if (shownExpression == null || shownExpression.Expr != value.Expr) {
					shownExpression = value;
					txtExpression.Text = value.Expr;
					RefreshView();
				}
			}
		}
		
		ObjectGraph RebuildGraph()
		{
			this.objectGraphBuilder = new ObjectGraphBuilder(debuggerService);
			return this.objectGraphBuilder.BuildGraphForExpression(this.ShownExpression, expanded.Expressions);
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
			expanded.Expressions.SetExpanded(e.Property.Expression.Expr);
			
			// add edge (+ possibly nodes) to underlying object graph (no need to fully rebuild)
			e.Property.ObjectGraphProperty.TargetNode = this.objectGraphBuilder.ObtainNodeForExpression(e.Property.Expression);
			LayoutGraph(this.objectGraph);
		}
		
		void node_PropertyCollapsed(object sender, PositionedPropertyEventArgs e)
		{
			// remember this property is collapsed (for later graph rebuilds)
			expanded.Expressions.SetCollapsed(e.Property.Expression.Expr);
			
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
