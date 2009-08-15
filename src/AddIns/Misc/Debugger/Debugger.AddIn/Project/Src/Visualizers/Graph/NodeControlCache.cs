// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Graph.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Description of NodeControlCache.
	/// </summary>
	public class NodeControlCache
	{
		public static readonly NodeControlCache Instance = new NodeControlCache();
		
		private Stack<PositionedGraphNodeControl> controls;
		private int controlsReturned = 0;
		
		private NodeControlCache()
		{
			Clear();
		}
		
		public void ReturnForReuse(PositionedGraphNodeControl controlToReuse)
		{
			controls.Push(controlToReuse);
		}
		
		public PositionedGraphNodeControl GetNodeControl()
		{
			return new PositionedGraphNodeControl();
			var control = controls.Count == 0 ? new PositionedGraphNodeControl() : controls.Pop();
			control.Init();
			//control.InvalidateVisual();
			controlsReturned++;
			return control;
		}
		
		public void Clear()
		{
			controls = new Stack<PositionedGraphNodeControl>();
			controlsReturned = 0;
		}
	}
}
