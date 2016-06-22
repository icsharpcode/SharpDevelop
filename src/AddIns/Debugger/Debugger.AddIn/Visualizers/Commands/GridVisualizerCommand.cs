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

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.AddIn.TreeModel;
using Debugger.AddIn.Visualizers.GridVisualizer;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;
using Debugger.AddIn.Visualizers.Utils;

namespace Debugger.AddIn.Visualizers
{
	public class GridVisualizerDescriptor : IVisualizerDescriptor
	{
		public bool IsVisualizerAvailable(IType type)
		{
			if (type.IsAtomic()) return false;
			ParameterizedType collectionType;
			IType itemType;
			// Visualizer available for IEnumerable<T> (that is, also IList<T>)
			return type.ResolveIEnumerableImplementation(out collectionType, out itemType);
		}
		
		public IVisualizerCommand CreateVisualizerCommand(string valueName, Func<Value> getValue)
		{
			return new GridVisualizerCommand(valueName, getValue);
		}
	}
	
	/// <summary>
	/// Shows grid visualizer for a node.
	/// </summary>
	public class GridVisualizerCommand : ExpressionVisualizerCommand
	{
		public GridVisualizerCommand(string valueName, Func<Value> getValue)
			: base(valueName, getValue)
		{
		}
		
		public override string ToString()
		{
			return "Collection visualizer";
		}
		
		public override void Execute()
		{
			GridVisualizerWindow window = new GridVisualizerWindow(this.ValueName, this.GetValue);
			window.ShowDialog();
		}
	}
}
