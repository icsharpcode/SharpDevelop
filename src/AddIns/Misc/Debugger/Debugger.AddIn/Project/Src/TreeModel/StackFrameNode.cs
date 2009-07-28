// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
			foreach(string arg in stackFrame.MethodInfo.ParameterNames) {
				yield return new ExpressionNode(ExpressionNode.GetImageForParameter(), arg, new IdentifierExpression(arg));
			}
			foreach(string loc in stackFrame.MethodInfo.LocalVariableNames) {
				yield return new ExpressionNode(ExpressionNode.GetImageForLocalVariable(), loc, new IdentifierExpression(loc));
			}
			if (stackFrame.Thread.CurrentException != null) {
				yield return new ExpressionNode(null, "__exception", new IdentifierExpression("__exception"));
			}
		}
	}
}
