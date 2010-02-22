// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Node that can be bound to <see cref="DebuggerTooltipControl" />.
	/// </summary>
	public interface ITreeNode
	{
		string Name { get; }
		
		string Text { get; }
		
		string Type { get; }
		
		ImageSource ImageSource { get; }
		
		IEnumerable<ITreeNode> ChildNodes { get; }
		
		bool HasChildNodes { get; }
		
		IEnumerable<IVisualizerCommand> VisualizerCommands { get; }
		
		bool HasVisualizerCommands { get; }
	}
}
