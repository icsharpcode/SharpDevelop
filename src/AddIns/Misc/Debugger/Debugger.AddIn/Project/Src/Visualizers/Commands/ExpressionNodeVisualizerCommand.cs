// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.TreeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Visualizer command for <see cref="ExpressionNode"/>
	/// </summary>
	// probably we should not make visualizers available only to ExpressionNodes and descendants,
	// the visualizer command should be available for any TreeNode
	// and the VisualizerCommand itself should decide what to do with passed instance
	public abstract class ExpressionNodeVisualizerCommand : IVisualizerCommand
	{
		public ExpressionNode Node { get; private set; }
		
		public ExpressionNodeVisualizerCommand(ExpressionNode expressionNode)
		{
			this.Node = expressionNode;
		}
		
		public abstract bool CanExecute { get; }
		
		public abstract void Execute();
	}
}
