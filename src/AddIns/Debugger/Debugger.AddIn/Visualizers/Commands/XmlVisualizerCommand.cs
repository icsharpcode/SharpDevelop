// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
		
		public override bool CanExecute { 
			get { return true; }
		}
		
		public override string ToString()
		{
			return "Xml visualizer";
		}
		
		public override void Execute()
		{
			if (this.Expression != null)
			{
				var textVisualizerWindow = new TextVisualizerWindow(
					this.Expression.PrettyPrint(), this.Expression.Evaluate(WindowsDebugger.CurrentProcess).InvokeToString());
				textVisualizerWindow.Mode = TextVisualizerMode.Xml;
				textVisualizerWindow.ShowDialog();
			}
		}
	}
}
