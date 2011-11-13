// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GraphSharp.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
	public class DependencyGraphLayout : GraphLayout<DependencyVertex, DependencyEdge, DependencyGraph>
	{
		public event MouseButtonEventHandler VertexClick;
		public event MouseButtonEventHandler VertexRightClick;
		public event EventHandler<SelectedVertexEventArgs> SelectedVertexChanged;
		
		public VertexControl SelectedVertexControl { get; set; }
		
		#region DependencyProperty for VertexControl.IsSelected
		
		public static readonly DependencyProperty IsSelectingEnabledProperty = 
			DependencyProperty.RegisterAttached("IsSelectingEnabled", 
			                                    typeof(bool), 
			                                    typeof(DependencyGraphLayout), 
			                                    new UIPropertyMetadata(false, OnIsSelectingEnabledPropertyChanged));
		
		public static readonly DependencyProperty IsSelectedProperty = 
			DependencyProperty.RegisterAttached("IsSelected", 
			                                    typeof(bool), 
			                                    typeof(DependencyGraphLayout), 
			                                    new UIPropertyMetadata(false));
		
		public static bool GetIsSelectingEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsSelectingEnabledProperty);
		}

		public static void SetIsSelectingEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(IsSelectingEnabledProperty, value);
		}
		
		public static bool GetIsSelected(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsSelectedProperty);
		}

		public static void SetIsSelected(DependencyObject obj, bool value)
		{
			if (obj != null)
				obj.SetValue(IsSelectedProperty, value);
		}
		
		private static void OnIsSelectingEnabledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is bool == false)
				return;
			
			var vertex = obj as VertexControl;
			
			if (obj == null)
				return;
			
			var graphLayout = vertex.RootCanvas as DependencyGraphLayout;
			
			if (graphLayout == null)
				return;
			
			if ((bool)e.NewValue)
				graphLayout.SelectedVertexChanged += OnSelectedVertexChanged;
			else
				graphLayout.SelectedVertexChanged -= OnSelectedVertexChanged;
		}
		
		public static void OnSelectedVertexChanged(object sender, SelectedVertexEventArgs e)
		{
			var vertex = sender as VertexControl;
			
			if (vertex == null)
				return;
			
			if (e.New == vertex) {
				SetIsSelected(e.New as DependencyObject, true);
				SetIsSelected(e.Old as DependencyObject, false);
			}
			else
				SetIsSelected(e.Old as DependencyObject, false);
		}
		
		#endregion
		
		public void ChangeGraph(DependencyGraph graph)
		{
			try {
				if (graph != null && graph.VertexCount > 0)
					Graph = graph;
			} catch {} // ignore it if it fails

			AttachEvents();
		}

		private void AttachEvents()
		{
			foreach (UIElement element in Children)
			{
				var vertex = element as VertexControl;

				if (vertex != null)
				{
					// Preview because otherwise is LeftButtonDown blocked
					vertex.PreviewMouseLeftButtonDown += vertex_MouseLeftButtonDown;
					vertex.MouseRightButtonDown += vertex_MouseRightButtonDown;
					vertex.MouseDoubleClick += vertex_MouseDoubleClick;
				}
			}
		}

		void vertex_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (VertexRightClick != null)
				VertexRightClick(sender, e);

			var vertexControl = sender as VertexControl;
			if (vertexControl != null)
				CreateContextMenu(vertexControl);
		}

		private void CreateContextMenu(VertexControl vertexControl)
		{
			var menu = new ContextMenu();

			// internal dependencies
			var iternalDependency = new MenuItem { Header = "View internal dependencies on graph" };
			iternalDependency.Click += (sender, e) =>
			{
				var vc =
					((ContextMenu) ((MenuItem) sender).Parent).PlacementTarget as
					VertexControl;
				if (vc != null)
				{
					var vertex = vertexControl.Vertex as DependencyVertex;
					if (vertex != null && vertex.Node.Dependency != null)
					{
						ChangeGraph(vertex.Node.Dependency.BuildDependencyGraph());
					}
				}
			};
			menu.Items.Add(iternalDependency);

			menu.Items.Add(new Separator());

			// remove
			var remove = new MenuItem { Header = "Remove" };
			remove.Click += (sender, e) =>
			{
				// no nested menus for now so this is ok
				var vc =
					((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as
					VertexControl;

				if (vc != null)
				{
					var vertex = vertexControl.Vertex as DependencyVertex;
					if (vertex != null)
					{
						RemoveVertexWithEdges(vertex);
					}
				}
			};
			menu.Items.Add(remove);

			// keep only nodes that use this
			var useThis = new MenuItem { Header = "Keep only nodes that use this" };
			useThis.Click += (sender, e) =>
			{
				// no nested menus for now so this is ok
				var vc =
					((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as
					VertexControl;

				if (vc != null)
				{
					var vertex = vertexControl.Vertex as DependencyVertex;
					if (vertex != null)
					{
						ISet<DependencyVertex> vertices = new HashSet<DependencyVertex>();
						vertices.Add(vertex); // original vertex shouldnt be removed

						foreach (KeyValuePair<DependencyEdge,EdgeControl> pair in _edgeControls)
						{
							if (pair.Key.Target.Equals(vertex))
							{
								vertices.Add(pair.Key.Source);
							}
						}

						foreach (var v in _vertexControls.Keys.Except(vertices).ToList())
						{
							RemoveVertexWithEdges(v);
						}
					}
				}
			};
			menu.Items.Add(useThis);

			var thisUse = new MenuItem { Header = "Keep only nodes that this use" };
			thisUse.Click += (sender, e) =>
			{
				// no nested menus for now so this is ok
				var vc =
					((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as
					VertexControl;

				if (vc != null)
				{
					var vertex = vertexControl.Vertex as DependencyVertex;
					if (vertex != null)
					{
						ISet<DependencyVertex> vertices = new HashSet<DependencyVertex>();
						vertices.Add(vertex); // original vertex shouldnt be removed

						foreach (KeyValuePair<DependencyEdge, EdgeControl> pair in _edgeControls)
						{
							if (pair.Key.Source.Equals(vertex))
							{
								vertices.Add(pair.Key.Target);
							}
						}

						foreach (var v in _vertexControls.Keys.Except(vertices).ToList())
						{
							RemoveVertexWithEdges(v);
						}
					}
				}
			};
			menu.Items.Add(thisUse);

			// keep only nodes users and used
			var usersUsed = new MenuItem { Header = "Keep only nodes users and used" };
			usersUsed.Click += (sender, e) =>
			{
				// no nested menus for now so this is ok
				var vc =
					((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as
					VertexControl;

				if (vc != null)
				{
					var vertex = vertexControl.Vertex as DependencyVertex;
					if (vertex != null)
					{
						ISet<DependencyVertex> vertices = new HashSet<DependencyVertex>();
						vertices.Add(vertex); // original vertex shouldnt be removed

						foreach (KeyValuePair<DependencyEdge, EdgeControl> pair in _edgeControls)
						{
							if (pair.Key.Source.Equals(vertex))
							{
								vertices.Add(pair.Key.Target);
							}

							if (pair.Key.Target.Equals(vertex))
							{
								vertices.Add(pair.Key.Source);
							}
						}

						foreach (var v in _vertexControls.Keys.Except(vertices).ToList())
						{
							RemoveVertexWithEdges(v);
						}
					}
				}
			};
			menu.Items.Add(usersUsed);

			vertexControl.ContextMenu = menu;
			vertexControl.ContextMenu.PlacementTarget = this;
		}

		private void RemoveVertexWithEdges(DependencyVertex vertex)
		{
			IList<DependencyEdge> edges = new List<DependencyEdge>();

			foreach (KeyValuePair<DependencyEdge,EdgeControl> pair in _edgeControls)
			{
				if (pair.Key.Source.Equals(vertex) || pair.Key.Target.Equals(vertex))
					edges.Add(pair.Key);
			}

			foreach (DependencyEdge edge in edges)
			{
				RemoveEdgeControl(edge);
			}

			RemoveVertexControl(vertex);
		}

		private void vertex_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var vertexControl = sender as VertexControl;
			if (vertexControl != null)
			{
				var vertex = vertexControl.Vertex as DependencyVertex;
				if (vertex != null && vertex.Node.Dependency != null)
				{
					ChangeGraph(vertex.Node.Dependency.BuildDependencyGraph());
				}
			}
		}

		private void vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (VertexClick != null)
				VertexClick(sender, e);
			
			// selection for nodes
			var vertex = sender as VertexControl;
			if (vertex != null) {
				if (SelectedVertexControl == vertex) {
					SelectedVertexControl = null;
					SelectedVertexChanged(sender, new SelectedVertexEventArgs(null, vertex));
					return;
				}
				
				if (SelectedVertexChanged != null)
					SelectedVertexChanged(sender, new SelectedVertexEventArgs(vertex, SelectedVertexControl));
				
				SelectedVertexControl = vertex;
			}
		}

		public void ResetGraph()
		{
			RecreateGraphElements(false);
			Relayout();
			AttachEvents();
		}

		public override void Relayout()
		{
			if (_vertexControls.Count > 0)
				base.Relayout();
		}

		public override void ContinueLayout()
		{
			if (_vertexControls.Count > 0)
				base.ContinueLayout();
		}
		
		public class SelectedVertexEventArgs : EventArgs
		{
			public VertexControl New { get; set; }
			public VertexControl Old { get; set; }
			
			public SelectedVertexEventArgs(VertexControl @new, VertexControl old)
			{
				New = @new;
				Old = old;
			}
		}
	}
}
