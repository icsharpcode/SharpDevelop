// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.Core;
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
        public MemberInfo MemberInfo { get; set; }
        
        /// <summary>
        /// Has this property been evaluated? (Has Evaluate been called?)
        /// </summary>
		public bool IsEvaluated { get; private set; }
		
		public void Evaluate()
		{
			if (this.Expression == null) throw new DebuggerVisualizerException("Cannot evaluate property with missing Expression");
			Value debuggerVal;
			try {
				debuggerVal = this.Expression.Evaluate(WindowsDebugger.CurrentProcess);
			} catch (System.Exception ex) {
				this.Value = "Exception: " + ex.Message;
				this.IsEvaluated = true;
				return;
			}
			
			this.IsAtomic = debuggerVal.Type.IsAtomic();
			this.IsNull = debuggerVal.IsNull;
			// null and complex properties will show empty string
			this.Value = debuggerVal.IsNull || (!this.IsAtomic) ? string.Empty : debuggerVal.InvokeToString();
			this.IsEvaluated = true;
		}
	}
}
