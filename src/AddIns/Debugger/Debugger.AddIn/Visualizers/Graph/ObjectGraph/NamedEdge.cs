// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Named edge connecting 2 objects of same type.
	/// </summary>
	public class NamedEdge<TSourceTarget> : NamedEdge<TSourceTarget, TSourceTarget>
	{
	}
	
	/// <summary>
	/// Named edge connecting 2 objects of arbitrary types.
	/// </summary>
	public class NamedEdge<TSource, TTarget>
	{
		/// <summary>
    	/// Name of the edge.
    	/// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Target node of the edge.
        /// </summary>
        public TTarget Target { get; set; }
        
        /// <summary>
        /// Source node of the edge.
        /// </summary>
        public TSource Source { get; set; }
	}
}
