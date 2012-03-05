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
	public partial class Utils
	{
		public static IEnumerable<TreeNode> GetLocalVariableNodes(StackFrame stackFrame)
		{
			foreach(DebugParameterInfo par in stackFrame.MethodInfo.GetParameters()) {
				var parCopy = par;
				yield return new ExpressionNode("Icons.16x16.Parameter", par.Name, () => parCopy.GetValue(stackFrame));
			}
			if (stackFrame.HasSymbols) {
				foreach(DebugLocalVariableInfo locVar in stackFrame.MethodInfo.GetLocalVariables(stackFrame.IP)) {
					var locVarCopy = locVar;
					yield return new ExpressionNode("Icons.16x16.Local", locVar.Name, () => locVarCopy.GetValue(stackFrame));
				}
			} else {
				WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
				if (debugger.debuggerDecompilerService != null) {
					int typeToken = stackFrame.MethodInfo.DeclaringType.MetadataToken;
					int methodToken = stackFrame.MethodInfo.MetadataToken;
					foreach (var localVar in debugger.debuggerDecompilerService.GetLocalVariables(typeToken, methodToken)) {
						int index = ((int[])debugger.debuggerDecompilerService.GetLocalVariableIndex(typeToken, methodToken, localVar))[0];
						yield return new ExpressionNode("Icons.16x16.Local", localVar, () => stackFrame.GetLocalVariableValue((uint)index));
					}
				}
			}
			if (stackFrame.Thread.CurrentException != null) {
				yield return new ExpressionNode(null, "$exception", () => stackFrame.Thread.CurrentException.Value);
			}
		}
	}
}
