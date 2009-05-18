// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Windows;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Transforms positions to and from Neato.
	/// </summary>
	public class NeatoPositionTransform
	{
		private Rect graphBoundingRect;
		
		// So that Neato works with smaller numbers. Always > 1
		double ourInputScale = 40;
		// Neato itself scales input by this constant. Always > 1, 1 for plain output, 72 for .dot output
		double neatoOutputScale = 1;
		
		public NeatoPositionTransform(Rect graphBoundingRectangle)
		{
			graphBoundingRect = graphBoundingRectangle;
		}
		
		/// <summary>
		/// X shift, in Neato coords.
		/// </summary>
		public double NeatoShiftX { get; set; }
		/// <summary>
		/// Y shift, in Neato coords.
		/// </summary>
		public double NeatoShiftY { get; set; }
		
		public Point ToNeatoInput(Point ourPoint)
		{
			// invert Y axis, as Neato expects this
			return new Point(ourPoint.X / ourInputScale, (graphBoundingRect.Bottom - ourPoint.Y) / ourInputScale);
		}
		
		public System.Windows.Point FromNeatoOutput(Point neatoPoint)
		{
			// Neato multiplies coords by 72 and adds arbitrary shift (which has to be parsed)
			double ourX = (neatoPoint.X - NeatoShiftX) / neatoOutputScale * ourInputScale;
			double ourYInverted = (neatoPoint.Y - NeatoShiftY) / neatoOutputScale * ourInputScale;
			// invert back - our Y axis grows down
			return new Point(ourX, graphBoundingRect.Bottom - ourYInverted);
		}
		
		/// <summary>
		/// Transform points as Neato would transform it if Neato used no shift
		/// </summary>
		/// <param name="ourPoint"></param>
		/// <returns></returns>
		public System.Windows.Point AsNeato(Point ourPoint)
		{
			
			return new Point(ourPoint.X * neatoOutputScale / ourInputScale, (graphBoundingRect.Bottom - ourPoint.Y) * neatoOutputScale / ourInputScale);
		}
		
		public Rect NodeToNeatoInput(PositionedNode node)
		{
			// don't transform size
			return new Rect(ToNeatoInput(node.Center),
			                new Size(node.Width / ourInputScale, node.Height / ourInputScale));
		}
	}
}
