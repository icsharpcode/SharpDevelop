// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Remembers expressions and content nodes the user has expanded in <see cref="PositionedGraph"></see>.
	/// </summary>
	public class Expanded
	{
		private ExpandedExpressions expandedExpressions = new ExpandedExpressions();
		private ExpandedContentNodes expandedContentNodes = new ExpandedContentNodes();
		
		public ExpandedExpressions Expressions
		{
			get { return this.expandedExpressions; }
		}
		
		public ExpandedContentNodes ContentNodes
		{
			get { return this.expandedContentNodes; }
		}
	}
}
