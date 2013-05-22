// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.Graph;
using Debugger.MetaData;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using Debugger.AddIn.Visualizers.Utils;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers
{
	public class ObjectGraphVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(IType type)
		{
			return !type.IsAtomic() && !type.IsSystemDotObject();
		}
		
		public IVisualizerCommand CreateVisualizerCommand(string valueName, Func<Value> getValue)
		{
			return new ObjectGraphVisualizerCommand(valueName, getValue);
		}
	}
	
	/// <summary>
	/// Shows object graph visualizer for a node.
	/// </summary>
	public class ObjectGraphVisualizerCommand : ExpressionVisualizerCommand
	{
		public ObjectGraphVisualizerCommand(string valueName, Func<Value> getValue) : base(valueName, getValue)
		{
		}
		
		public override string ToString()
		{
			return "Object graph visualizer";
		}
		
		public override void Execute()
		{
			var objectGraphWindow = ObjectGraphWindow.EnsureShown();
			objectGraphWindow.ShownExpression = new GraphExpression(this.ValueName, this.GetValue);
		}
	}
}
