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
		internal Debugger.Value PermanentReference { get; set; } // needed for graph building and matching, since hashCodes are not unique
		/// <summary>
		/// Hash code in the debuggee of the DebuggerValue this node represents.
		/// </summary>
		internal int HashCode { get; set; }
		/// <summary>
		/// Expression used to obtain this node.
		/// </summary>
		public Expressions.Expression Expression { get { return this.PermanentReference.Expression; } }
		
		class PropertyComparer : IComparer<ObjectGraphProperty>
		{
			public int Compare(ObjectGraphProperty prop1, ObjectGraphProperty prop2)
			{
				// order by IsAtomic, Name
				int comparedAtomic = prop2.IsAtomic.CompareTo(prop1.IsAtomic);
				if (comparedAtomic != 0)
				{
					return comparedAtomic;
				}
				else
				{
					return prop1.Name.CompareTo(prop2.Name);
				}
			}
		}
		
		private static PropertyComparer propertySortComparer = new PropertyComparer();
		
		private bool sorted = false;

		private List<ObjectGraphProperty> properties = new List<ObjectGraphProperty>();
		/// <summary>
		/// Properties (either atomic or complex) of the object this node represents.
		/// </summary>
		public List<ObjectGraphProperty> Properties
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
		public IEnumerable<ObjectGraphProperty> ComplexProperties
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
			properties.Add(new ObjectGraphProperty
			                { Name = name, Value = value, Expression = expression, IsAtomic = true, TargetNode = null });
		}
		
		/// <summary>
		/// Adds complex property.
		/// </summary>
		internal void AddComplexProperty(string name, string value, Expression expression, ObjectNode targetNode, bool isNull)
		{
			properties.Add(new ObjectGraphProperty
			                { Name = name, Value = value, Expression = expression, IsAtomic = false, TargetNode = targetNode, IsNull = isNull });
		}
	}
}
