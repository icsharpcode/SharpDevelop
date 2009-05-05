// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Text;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Node in the <see cref="ObjectGraph" />.
	/// </summary>
    public class ObjectNode
    {
    	/// <summary>
		/// Additional info useful for internal algorithms, not to be visible to the user.
		/// </summary>
    	internal Debugger.Value PermanentReference { get; set; }
    	
        private List<ObjectEdge> _edges = new List<ObjectEdge>();
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
        }

        private ObjectPropertyCollection _properties = new ObjectPropertyCollection();
        /// <summary>
        /// Properties representing atomic string values as part of the node.
        /// </summary>
        public ObjectPropertyCollection Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Adds string property.
        /// </summary>
        internal void AddProperty(string name, string value)
        {
            _properties.Add(new ObjectProperty { Name = name, Value = value });
        }
    }
}
