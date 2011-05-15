// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.SharpDevelop.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.GridVisualizer;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers
{
	public class GridVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(DebugType type)
		{
			if (type.IsAtomic()) {
				return false;
			}
			DebugType collectionType, itemType;
			// Visualizer available for IEnumerable<T> (that is, also IList<T>)
			return type.ResolveIEnumerableImplementation(out collectionType, out itemType);
		}
		
		public IVisualizerCommand CreateVisualizerCommand(Expression expression)
		{
			return new GridVisualizerCommand(expression);
		}
	}
	
	/// <summary>
	/// Shows grid visualizer for a node.
	/// </summary>
	public class GridVisualizerCommand : ExpressionVisualizerCommand
	{
		public GridVisualizerCommand(Expression expression)
			:base(expression)
		{
		}
		
		public override string ToString()
		{
			return "Collection visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression == null)
				return;
			var gridVisualizerWindow = GridVisualizerWindow.EnsureShown();
			gridVisualizerWindow.ShownExpression = this.Expression;
		}
	}
}
