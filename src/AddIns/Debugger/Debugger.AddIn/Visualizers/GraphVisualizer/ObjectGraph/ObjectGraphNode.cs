// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		public GraphExpression Expression { get; set; }

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
			get	{ 
				return this.Content.FlattenPropertyNodes().Select(node => node.Property);
			}
		}
		
		/// <summary>
		/// Same as <see cref="Properties" /> but sorted so that .NET properties come first, and .NET fields after them.
		/// </summary>
		public IEnumerable<ObjectGraphProperty> PropertiesFirstThenFields
		{
			get { 
				return this.Properties.OrderBy(prop => prop.MemberInfo, PropertiesFirstComparer.Instance); 
			}
		}
	}
}
