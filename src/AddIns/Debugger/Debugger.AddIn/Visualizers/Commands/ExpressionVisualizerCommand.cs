// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.NRefactory.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using ICSharpCode.SharpDevelop.Debugging;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Visualizer command for <see cref="ExpressionNode"/>
	/// </summary>
	// should we make visualizer command available for Expression, or any TreeNode?
	public abstract class ExpressionVisualizerCommand : IVisualizerCommand
	{
		public string ValueName { get; private set; }
		public Func<Value> GetValue { get; private set; }
		
		public ExpressionVisualizerCommand(string valueName, Func<Value> getValue)
		{
			if (getValue == null)
				throw new ArgumentNullException("getValue");
			this.ValueName = valueName;
			this.GetValue = getValue;
		}
		
		public abstract void Execute();
	}
}
