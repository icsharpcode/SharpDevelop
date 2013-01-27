// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.TextVisualizer;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Visualizers
{
	public class TextVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(IType type)
		{
			return type.FullName == typeof(string).FullName;
		}
		
		public IVisualizerCommand CreateVisualizerCommand(string valueName, Func<Value> getValue)
		{
			return new TextVisualizerCommand(valueName, getValue);
		}
	}
	
	public class TextVisualizerCommand : ExpressionVisualizerCommand
	{
		public TextVisualizerCommand(string valueName, Func<Value> getValue) : base(valueName, getValue)
		{
		}
		
		public override string ToString()
		{
			return "Text visualizer";
		}
		
		public override void Execute()
		{
			var textVisualizerWindow = new TextVisualizerWindow(this.ValueName, this.GetValue().AsString());
			textVisualizerWindow.ShowDialog();
		}
	}
}
