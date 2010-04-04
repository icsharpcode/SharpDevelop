// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Enables passing any type of graph (implementing IRect, IEdge) into EdgeRouter.
	/// </summary>
	public interface IRect
	{
		double Left { get; }
		double Top { get; }
		double Width { get; }
		double Height { get; }
		IRect Inflated(double padding);
	}
}
