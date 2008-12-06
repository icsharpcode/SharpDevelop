// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public class StackFrameNode: AbstractNode
	{
		StackFrame stackFrame;
		
		public StackFrame StackFrame {
			get { return stackFrame; }
		}
		
		public StackFrameNode(StackFrame stackFrame)
		{
			this.stackFrame = stackFrame;
			
			this.Name = stackFrame.MethodInfo.Name;
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression expr in stackFrame.MethodInfo.GetExpressionsForAllVariables()) {
				yield return ValueNode.Create(expr);
			}
			if (stackFrame.Thread.CurrentException != null) {
				yield return ValueNode.Create(new CurrentExceptionExpression());
			}
		}
	}
}
