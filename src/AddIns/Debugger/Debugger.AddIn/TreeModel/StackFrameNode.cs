// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using Debugger.MetaData;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.TreeModel
{
	public class StackFrameNode: TreeNode
	{
		StackFrame stackFrame;
		
		public StackFrame StackFrame {
			get { return stackFrame; }
		}
		
		public StackFrameNode(StackFrame stackFrame)
		{
			this.stackFrame = stackFrame;
			
			this.Name = stackFrame.MethodInfo.Name;
			this.ChildNodes = LazyGetChildNodes();
		}
		
		IEnumerable<TreeNode> LazyGetChildNodes()
		{
			foreach(DebugParameterInfo par in stackFrame.MethodInfo.GetParameters()) {
				yield return new ExpressionNode(ExpressionNode.GetImageForParameter(), par.Name, par.GetExpression());
			}
			foreach(DebugLocalVariableInfo locVar in stackFrame.MethodInfo.GetLocalVariables(this.StackFrame.IP)) {
				yield return new ExpressionNode(ExpressionNode.GetImageForLocalVariable(), locVar.Name, locVar.GetExpression());
			}
			if (stackFrame.Thread.CurrentException != null) {
				yield return new ExpressionNode(null, "__exception", new IdentifierExpression("__exception"));
			}
		}
	}
}
