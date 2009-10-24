// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Expression = ICSharpCode.NRefactory.Ast.Expression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Node in the <see cref="ObjectGraph" />.
	/// </summary>
	public class ObjectGraphNode
	{
		/// <summary>
		/// Permanent reference to the value in the the debugee this node represents.
		/// Needed for graph building and matching, since hashCodes are not unique.
		/// </summary>
		internal Debugger.Value PermanentReference { get; set; }
		/// <summary>
		/// Hash code in the debuggee of the DebuggerValue this node represents.
		/// </summary>
		internal int HashCode { get; set; }
		/// <summary>
		/// Expression used to obtain this node.
		/// </summary>
		public Expression Expression { get; set; }

		/// <summary>
		/// Property tree of this node.
		/// </summary>
		public ThisNode Content
		{
			get; set;
		}
		
		public IEnumerable<ObjectGraphProperty> Properties
		{
			get
			{
				return this.Content.FlattenPropertyNodes().Select(node => {return node.Property; });
			}
		}
	}
}
