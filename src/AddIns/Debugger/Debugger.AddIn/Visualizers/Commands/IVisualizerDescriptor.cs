// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
