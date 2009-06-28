// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.SharpDevelop.Services;
using System;

namespace Debugger.AddIn.Visualizers.Graph
{
	/// <summary>
	/// Node containing ObjectGraphProperty, with lazy evaluation.
	/// </summary>
	public class PropertyNode : AbstractNode, IEvaluate
	{
		public PropertyNode(ObjectGraphProperty objectGraphProperty)
		{
			if (objectGraphProperty == null)
				throw new ArgumentNullException("objectGraphProperty");
			
			this.property = objectGraphProperty;
		}
		
		private ObjectGraphProperty property;
		public ObjectGraphProperty Property
		{
			get { return this.property; }
		}
		
		#region IEvaluate members
		
		bool evaluateCalled = false;
		public bool IsEvaluated
		{
			get { return this.evaluateCalled; }
		}
		
		public void Evaluate()
		{
			if (this.Property.Expression == null)
			{
				throw new DebuggerVisualizerException("Cannot evaluate property with missing Expression");
			}
			this.Property.Value = 
				this.Property.Expression.Evaluate(WindowsDebugger.CurrentProcess).InvokeToString();
			
			this.evaluateCalled = true;
		}
		
		#endregion
	}
}
