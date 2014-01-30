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
using Debugger.AddIn.Visualizers.Graph;
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
        public GraphExpression Expression { get; set; }
        
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
