// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using ICSharpCode.SharpDevelop.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Creates visualizer commands and can decide whether visualizer command is available for given type.
	/// </summary>
	public interface IVisualizerDescriptor
	{
		bool IsVisualizerAvailable(DebugType type);
		IVisualizerCommand CreateVisualizerCommand(Expression expression);
	}
}
