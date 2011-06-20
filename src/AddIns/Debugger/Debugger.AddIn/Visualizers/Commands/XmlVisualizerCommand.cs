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
	public class XmlVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(DebugType type)
		{
			return type.FullName == typeof(string).FullName;
		}
		
		public IVisualizerCommand CreateVisualizerCommand(Expression expression)
		{
			return new XmlVisualizerCommand(expression);
		}
	}
	
	/// <summary>
	/// Description of TextVisualizerCommand.
	/// </summary>
	public class XmlVisualizerCommand : ExpressionVisualizerCommand
	{
		public XmlVisualizerCommand(Expression expression)
			:base(expression)
		{
		}
		
		public override string ToString()
		{
			return "XML visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression == null)
				return;
			var textVisualizerWindow = new TextVisualizerWindow(
				this.Expression.PrettyPrint(), this.Expression.Evaluate(WindowsDebugger.CurrentProcess).AsString());
			textVisualizerWindow.Mode = TextVisualizerMode.Xml;
			textVisualizerWindow.ShowDialog();
		}
	}
}
