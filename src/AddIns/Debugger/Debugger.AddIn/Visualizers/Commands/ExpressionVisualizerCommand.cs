// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Visualizer command for <see cref="ExpressionNode"/>
	/// </summary>
	// should we make visualizer command available for Expression, or any TreeNode?
	public abstract class ExpressionVisualizerCommand : IVisualizerCommand
	{
		public Expression Expression { get; private set; }
		
		public ExpressionVisualizerCommand(Expression expression)
		{
			this.Expression = expression;
		}
		
		public abstract bool CanExecute { get; }
		
		public abstract void Execute();
	}
}
