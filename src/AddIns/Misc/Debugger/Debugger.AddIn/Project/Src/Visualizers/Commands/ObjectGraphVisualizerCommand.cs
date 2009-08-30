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
using Debugger.AddIn.Visualizers.Graph;
using Debugger.MetaData;
using ICSharpCode.SharpDevelop.Debugging;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers
{
	public class ObjectGraphVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(DebugType type)
		{
			bool typeIsAtomic = type.IsPrimitive || type.IsSystemDotObject() || type.IsEnum();
			return !typeIsAtomic;
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
		
		public override bool CanExecute { 
			get { return true; }
		}
		
		public override string ToString()
		{
			return "Object graph visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression != null)
			{
				var objectGraphWindow = ObjectGraphWindow.EnsureShown();
				objectGraphWindow.ShownExpression = this.Expression;
			}
		}
	}
}
