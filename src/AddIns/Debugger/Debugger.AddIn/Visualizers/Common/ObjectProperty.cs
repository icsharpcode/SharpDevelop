// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;

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
        public Expression Expression { get; set; }
        
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
