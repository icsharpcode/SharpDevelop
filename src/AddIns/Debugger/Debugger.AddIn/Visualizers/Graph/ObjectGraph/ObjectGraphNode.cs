// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
		public ThisNode Content { get; set; }
		
		/// <summary>
		/// Name of the Type in the debuggee.
		/// </summary>
		public string TypeName { get; set; }
		
		/// <summary>
		/// All ObjectGraphProperties in the property tree of this node.
		/// </summary>
		public IEnumerable<ObjectGraphProperty> Properties
		{
			get	{ return this.Content.FlattenPropertyNodes().Select(node => {return node.Property; });	}
		}
		
		/// <summary>
		/// Same as <see cref="Properties" /> but sorted so that .NET properties come first, and .NET fields after them.
		/// </summary>
		public IEnumerable<ObjectGraphProperty> PropertiesFirstThenFields
		{
			get { return this.Properties.OrderBy(prop => prop.MemberInfo, PropertiesFirstComparer.Instance); }
		}
	}
}
