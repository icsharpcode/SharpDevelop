using System;
// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Text;
using Debugger.AddIn.Visualizers.Graph;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Debugger.AddIn.Visualizers.Graph.Drawing;
using Debugger.AddIn.Visualizers.Graph.Layout;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Draws ObjectGraph on a Canvas.
	/// </summary>
    public class GraphDrawer
    {
        private ObjectGraph graph;

        public GraphDrawer(ObjectGraph graph)
        {
            this.graph = graph;
        }
        
        public static void Draw(PositionedGraph posGraph, Canvas canvas)
        {
        	canvas.Children.Clear();
        	
        	foreach	(PositionedNode node in posGraph.Nodes)
        	{
        		canvas.Children.Add(node.NodeVisualControl);
        		Canvas.SetLeft(node.NodeVisualControl, node.Left);
        		Canvas.SetTop(node.NodeVisualControl, node.Top);
        	}
        }
    }
}
