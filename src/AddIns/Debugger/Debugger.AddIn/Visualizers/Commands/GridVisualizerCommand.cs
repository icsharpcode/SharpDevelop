// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
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
		public bool IsVisualizerAvailable(IType type)
		{
			if (type.IsAtomic()) return false;
			ParameterizedType collectionType;
			IType itemType;
			// Visualizer available for IEnumerable<T> (that is, also IList<T>)
			return type.ResolveIEnumerableImplementation(out collectionType, out itemType);
		}
		
		public IVisualizerCommand CreateVisualizerCommand(string valueName, Func<Value> getValue)
		{
			return new GridVisualizerCommand(valueName, getValue);
		}
	}
	
	/// <summary>
	/// Shows grid visualizer for a node.
	/// </summary>
	public class GridVisualizerCommand : ExpressionVisualizerCommand
	{
		public GridVisualizerCommand(string valueName, Func<Value> getValue)
			: base(valueName, getValue)
		{
		}
		
		public override string ToString()
		{
			return "Collection visualizer";
		}
		
		public override void Execute()
		{
			GridVisualizerWindow window = new GridVisualizerWindow(this.ValueName, this.GetValue);
			window.ShowDialog();
		}
	}
}
