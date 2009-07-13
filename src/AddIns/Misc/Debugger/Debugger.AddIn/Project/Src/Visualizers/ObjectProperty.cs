// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Property  of an object, in string form.
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
        /// Expression used for obtaining value of this property
        /// </summary>
        public Debugger.Expressions.Expression Expression { get; set; }
        
        /// <summary>
        /// Is this property of atomic type? (int, string, etc.)
        /// </summary>
        public bool IsAtomic { get; set; }
        
        /// <summary>
        /// Is this property value null? Only meaningful if <see cref="IsAtomic"/> is false.
        /// </summary>
        public bool IsNull { get; set; }
	}
}
