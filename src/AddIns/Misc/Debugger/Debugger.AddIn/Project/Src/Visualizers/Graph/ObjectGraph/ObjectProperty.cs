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
    /// Primitive property  of an object, in string form.
    /// </summary>
    public class ObjectProperty
    {
        /// <summary>
        /// e.g. "Age"
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// e.g. "19"
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Expression used for obtaining this property
        /// </summary>
        public Debugger.Expressions.Expression Expression { get; set; }
        /// <summary>
        /// Node that this property points to. Can be null. Always null if <see cref="IsAtomic"/> is true.
        /// </summary>
        public ObjectNode TargetNode { get; set; }
        /// <summary>
        /// Is this property of atomic type? (int, string, etc.)
        /// </summary>
        public bool IsAtomic { get; set; }
    }
}
