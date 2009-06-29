// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.SharpDevelop.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debugger.AddIn.Visualizers.Graph
{
    /// <summary>
    /// ObjectProperty used in ObjectGraph. Has TargetNode.
    /// Support on-demand evaluation of Value property.
    /// </summary>
    public class ObjectGraphProperty : ObjectProperty, IEvaluate
    {
        /// <summary>
        /// Node that this property points to. Can be null. Always null if <see cref="IsAtomic"/> is true.
        /// </summary>
        public ObjectGraphNode TargetNode { get; set; }
        
       bool evaluateCalled = false;
		public bool IsEvaluated
		{
			get { return this.evaluateCalled; }
		}
		
		public void Evaluate()
		{
			if (this.Expression == null)
			{
				throw new DebuggerVisualizerException("Cannot evaluate property with missing Expression");
			}
			this.Value = this.Expression.Evaluate(WindowsDebugger.CurrentProcess).InvokeToString();
			this.evaluateCalled = true;
		}
    }
}
