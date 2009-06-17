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
    /// ObjectProperty used in ObjectGraph. Has TargetNode.
    /// </summary>
    public class ObjectGraphProperty : ObjectProperty
    {
        /// <summary>
        /// Node that this property points to. Can be null. Always null if <see cref="IsAtomic"/> is true.
        /// </summary>
        public ObjectNode TargetNode { get; set; }
    }
}
