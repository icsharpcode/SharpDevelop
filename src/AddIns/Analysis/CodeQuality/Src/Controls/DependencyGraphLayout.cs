using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GraphSharp.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class DependencyGraphLayout : GraphLayout<DependencyVertex, DependencyEdge, DependencyGraph>
    {
        public event MouseButtonEventHandler VertexClick;

        public void ChangeGraph(DependencyGraph graph)
        {
            try
            {
                if (graph != null && graph.VertexCount > 0)
                {
                    this.Graph = graph;
                }
            }
            catch
            {
            } // ignore it if it fails

            foreach (UIElement element in this.Children)
            {
                var vertex = element as VertexControl;

                if (vertex != null)
                {
                    vertex.PreviewMouseLeftButtonDown += vertex_MouseLeftButtonDown;
                    vertex.MouseDoubleClick += vertex_MouseDoubleClick;
                }
            }
        }

        private void vertex_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vertexControl = sender as VertexControl;
            if (vertexControl != null)
            {
                var vertex = vertexControl.Vertex as DependencyVertex;
                if (vertex != null && vertex.Node.Dependency != null)
                {
                    this.ChangeGraph(vertex.Node.Dependency.BuildDependencyGraph());
                }
            }
        }

        private void vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VertexClick(sender, e);

            // TODO: Implement SelectedVertex and change its color
        }
    }
}
