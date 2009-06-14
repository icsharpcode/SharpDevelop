// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Text;
using Debugger.Expressions;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Node in the <see cref="ObjectGraph" />.
	/// </summary>
	public class ObjectNode
	{
		/// <summary>
		/// Permanent reference to the value in the the debugee this node represents.
		/// </summary>
		internal Debugger.Value DebuggerValue { get; set; }
		/// <summary>
		/// Hash code in the debuggee of the DebuggerValue this node represents.
		/// </summary>
		internal int HashCode { get; set; }
		/// <summary>
		/// Expression used to obtain this node.
		/// </summary>
		public Expressions.Expression Expression { get { return this.DebuggerValue.Expression; } }
		
		/*private List<ObjectEdge> _edges = new List<ObjectEdge>();
        /// <summary>
        /// Outgoing edges.
        /// </summary>
        public IEnumerable<ObjectEdge> Edges
        {
            get { return _edges; }
        }

        /// <summary>
        /// Adds outgoing edge.
        /// </summary>
        internal void AddNamedEdge(ObjectNode targetNode, string edgeName)
        {
            _edges.Add(new ObjectEdge { Name = edgeName, SourceNode = this, TargetNode = targetNode });
        }*/
		
		class PropertyComparer : IComparer<ObjectProperty>
		{
			public int Compare(ObjectProperty prop1, ObjectProperty prop2)
			{
				// order by IsAtomic, Name
				int atomic = prop2.IsAtomic.CompareTo(prop1.IsAtomic);
				if (atomic != 0)
				{
					return atomic;
				}
				else
				{
					return prop1.Name.CompareTo(prop2.Name);
				}
			}
		}
		
		private static PropertyComparer propertySortComparer = new PropertyComparer();
		
		private bool sorted = false;

		private List<ObjectProperty> properties = new List<ObjectProperty>();
		/// <summary>
		/// Properties (either atomic or complex) of the object this node represents.
		/// </summary>
		public List<ObjectProperty> Properties
		{
			get 
			{ 
				if (!sorted)
				{
					properties.Sort(propertySortComparer);
					sorted = true;
				}
				return properties; 
			}
		}
		/// <summary>
		/// Only complex properties filtered out of <see cref="Properties"/>
		/// </summary>
		public IEnumerable<ObjectProperty> ComplexProperties
		{
			get
			{
				foreach	(var property in Properties)
				{
					if (!property.IsAtomic)
						yield return property;
				}
			}
		}

		/// <summary>
		/// Adds primitive property.
		/// </summary>
		internal void AddAtomicProperty(string name, string value, Expression expression)
		{
			properties.Add(new ObjectProperty
			                { Name = name, Value = value, Expression = expression, IsAtomic = true, TargetNode = null });
		}
		
		/// <summary>
		/// Adds complex property.
		/// </summary>
		internal void AddComplexProperty(string name, string value, Expression expression, ObjectNode targetNode, bool isNull)
		{
			properties.Add(new ObjectProperty
			                { Name = name, Value = value, Expression = expression, IsAtomic = false, TargetNode = targetNode, IsNull = isNull });
		}
	}
}
