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
using System.Reflection;
using System.Text;
using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// ObjectProperty used in ObjectGraph. Has TargetNode.
	/// Holds an Expression which is evaluated on demand. 
	/// Evaluating fills properties like Value and IsAtomic, which are empty until evaluation.
	/// </summary>
	public class ObjectGraphProperty : ObjectProperty, IEvaluate
	{
		/// <summary>
		/// Node that this property points to. Can be null. Always null if <see cref="IsAtomic"/> is true.
		/// </summary>
		public ObjectGraphNode TargetNode { get; set; }
		
		/// <summary>
        /// MemberInfo used for obtaining value of this property
        /// </summary>
        public IMember MemberInfo { get; set; }
        
        /// <summary>
        /// Has this property been evaluated? (Has Evaluate been called?)
        /// </summary>
		public bool IsEvaluated { get; private set; }
		
		public void Evaluate()
		{
			if (this.Expression == null) throw new DebuggerVisualizerException("Cannot evaluate property with missing Expression");
			Value debuggerVal;
			try {
				debuggerVal = this.Expression.GetValue();
			} catch (System.Exception ex) {
				this.Value = "Exception: " + ex.Message;
				this.IsEvaluated = true;
				return;
			}
			
			this.IsAtomic = debuggerVal.Type.IsAtomic();
			this.IsNull = debuggerVal.IsNull;
			// null and complex properties will show empty string
			this.Value = debuggerVal.IsNull || (!this.IsAtomic) ? string.Empty : debuggerVal.InvokeToString(WindowsDebugger.EvalThread);
			this.IsEvaluated = true;
		}
	}
}
