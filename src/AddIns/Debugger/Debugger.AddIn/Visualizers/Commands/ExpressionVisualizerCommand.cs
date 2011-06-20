// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
		
		public abstract void Execute();
	}
}
