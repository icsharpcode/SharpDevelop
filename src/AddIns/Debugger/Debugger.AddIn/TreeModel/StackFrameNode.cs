// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Collections.Generic;
using Debugger.MetaData;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	public class StackFrameNode: TreeNode
	{
		StackFrame stackFrame;
		
		public StackFrame StackFrame {
			get { return stackFrame; }
		}
		
		public StackFrameNode(StackFrame stackFrame)
			: base(null)
		{
			this.stackFrame = stackFrame;
			
			this.Name = stackFrame.MethodInfo.Name;
			this.childNodes = LazyGetChildNodes();
		}
		
		IEnumerable<TreeNode> LazyGetChildNodes()
		{
			foreach(DebugParameterInfo par in stackFrame.MethodInfo.GetParameters()) {
				string imageName;
				var image = ExpressionNode.GetImageForParameter(out imageName);
				var expression = new ExpressionNode(this, image, par.Name, par.GetExpression());
				expression.ImageName = imageName;
				yield return expression;
			}
			if (this.stackFrame.HasSymbols) {
				foreach(DebugLocalVariableInfo locVar in stackFrame.MethodInfo.GetLocalVariables(this.StackFrame.IP)) {
					string imageName;
					var image = ExpressionNode.GetImageForLocalVariable(out imageName);
					var expression = new ExpressionNode(this, image, locVar.Name, locVar.GetExpression());
					expression.ImageName = imageName;
					yield return expression;
				}
			} else {
				WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
				if (debugger.debuggerDecompilerService != null) {
					int typeToken = this.stackFrame.MethodInfo.DeclaringType.MetadataToken;
					int methodToken = this.stackFrame.MethodInfo.MetadataToken;
					foreach (var localVar in debugger.debuggerDecompilerService.GetLocalVariables(typeToken, methodToken)) {
						string imageName;
						var image = ExpressionNode.GetImageForLocalVariable(out imageName);
						var expression = new ExpressionNode(this, image, localVar, ExpressionEvaluator.ParseExpression(localVar, SupportedLanguage.CSharp));
						expression.ImageName = imageName;
						yield return expression;
					}
				}
			}
			if (stackFrame.Thread.CurrentException != null) {
				yield return new ExpressionNode(this, null, "__exception", new IdentifierExpression("__exception"));
			}
		}
	}
}
