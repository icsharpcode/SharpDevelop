// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.TextVisualizer;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers
{
	public class TextVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(DebugType type)
		{
			return type.FullName == typeof(string).FullName;
		}
		
		public IVisualizerCommand CreateVisualizerCommand(Expression expression)
		{
			return new TextVisualizerCommand(expression);
		}
	}
	
	public class TextVisualizerCommand : ExpressionVisualizerCommand
	{
		public TextVisualizerCommand(Expression expression)
			:base(expression)
		{
		}
		
		public override string ToString()
		{
			return "Text visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression == null)
				return;
			string expressionValue = this.Expression.Evaluate(WindowsDebugger.CurrentProcess).AsString();
			var textVisualizerWindow = new TextVisualizerWindow(this.Expression.PrettyPrint(), expressionValue);
			textVisualizerWindow.ShowDialog();
		}
	}
}
