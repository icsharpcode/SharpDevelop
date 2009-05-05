// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger.AddIn.Visualizers.Graph
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
        public TTarget TargetNode { get; set; }
        
        /// <summary>
        /// Source node of the edge.
        /// </summary>
        public TSource SourceNode { get; set; }
	}
}
