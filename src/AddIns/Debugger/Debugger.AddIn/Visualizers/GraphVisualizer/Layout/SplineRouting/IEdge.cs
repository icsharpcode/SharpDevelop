// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Edge in a graph, connecting two <see cref="IRect" />s.
	/// Enables passing generic graphs <see cref="EdgeRouter />.
	/// </summary>
	public interface IEdge
	{
		IRect From { get; }
		IRect To { get; }
	}
}
