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
	public class XmlVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(IType type)
		{
			return type.FullName == typeof(string).FullName;
		}
		
		public IVisualizerCommand CreateVisualizerCommand(string valueName, Func<Value> getValue)
		{
			return new XmlVisualizerCommand(valueName, getValue);
		}
	}
	
	/// <summary>
	/// Description of TextVisualizerCommand.
	/// </summary>
	public class XmlVisualizerCommand : ExpressionVisualizerCommand
	{
		public XmlVisualizerCommand(string valueName, Func<Value> getValue) : base(valueName, getValue)
		{
		}
		
		public override string ToString()
		{
			return "XML visualizer";
		}
		
		public override void Execute()
		{
			var textVisualizerWindow = new TextVisualizerWindow(this.ValueName, this.GetValue().AsString(), ".xml");
			textVisualizerWindow.ShowDialog();
		}
	}
}
