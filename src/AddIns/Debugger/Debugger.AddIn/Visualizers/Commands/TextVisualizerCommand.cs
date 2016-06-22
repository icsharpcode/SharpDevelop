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
