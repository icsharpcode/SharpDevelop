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
        private Style _defaultVertexControlStyle;

        public event MouseButtonEventHandler VertexClick;
        public event MouseButtonEventHandler VertexRightClick;

        public VertexControl SelectedVertexControl { get; set; }

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
            
            var vertex = sender as VertexControl;
            if (vertex != null)
            {
                if (SelectedVertexControl == vertex)
                {
                    SelectedVertexControl.Style = _defaultVertexControlStyle;
                    SelectedVertexControl = null;
                    return;
                }

                if (SelectedVertexControl != null)
                {
                    SelectedVertexControl.Style = vertex.Style;
                }

                SelectedVertexControl = vertex;

                _defaultVertexControlStyle = vertex.Style;

                // workaround which doesnt brake triggers of highlighting
                var style = new Style();

                foreach (Setter setter in vertex.Style.Setters)
                {
                    style.Setters.Add(setter);
                }

                foreach (var trigger in vertex.Style.Triggers)
                {
                    style.Triggers.Add(trigger);
                }

                style.Setters.Add(new Setter
                                      {
                                          Property = Control.BackgroundProperty,
                                          Value = new SolidColorBrush(Color.FromRgb(255, 165, 0)) // orange
                                      });


                SelectedVertexControl.Style = style;
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
    }
}
