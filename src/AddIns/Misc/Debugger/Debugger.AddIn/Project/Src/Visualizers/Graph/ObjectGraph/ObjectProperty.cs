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
    /// Primitive property (int, string, etc.) of an object, in string form.
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
    }
}
