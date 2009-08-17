// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using Debugger.AddIn.Visualizers.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Shows object graph visualizer for a node.
	/// </summary>
	public class ObjectGraphVisualizerCommand : ExpressionNodeVisualizerCommand
	{
		public ObjectGraphVisualizerCommand(ExpressionNode expressionNode)
			:base(expressionNode)
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
			if (this.Node != null && this.Node.Expression != null)
			{
				var objectGraphWindow = VisualizerWPFWindow.EnsureShown();
				// is this ok with different languages than C#? 
				//  - Prettyprint an expression and then call WindowsDebugger.GetValueFromName, which is C# only
				objectGraphWindow.ShownExpression = this.Node.Expression.PrettyPrint();
			}
		}
	}
}
