// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.Graph;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Debugging;
using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers
{
	public class ObjectGraphVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(DebugType type)
		{
			return !type.IsAtomic() && !type.IsSystemDotObject();
		}
		
		public IVisualizerCommand CreateVisualizerCommand(Expression expression)
		{
			return new ObjectGraphVisualizerCommand(expression);
		}
	}
	
	/// <summary>
	/// Shows object graph visualizer for a node.
	/// </summary>
	public class ObjectGraphVisualizerCommand : ExpressionVisualizerCommand
	{
		public ObjectGraphVisualizerCommand(Expression expression)
			:base(expression)
		{
		}
		
		public override string ToString()
		{
			return "Object graph visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression == null)
				return;
			var objectGraphWindow = ObjectGraphWindow.EnsureShown();
			objectGraphWindow.ShownExpression = this.Expression;
		}
	}
}
