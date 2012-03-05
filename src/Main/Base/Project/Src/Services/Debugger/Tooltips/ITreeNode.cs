// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		string ImageName { get; }
		
		string Name { get; }
		
		string Text { get; }
		
		bool CanSetText { get; }
		
		string Type { get; }
		
		ImageSource ImageSource { get; }
		
		Func<IEnumerable<ITreeNode>> GetChildren { get; }
		
		IEnumerable<IVisualizerCommand> VisualizerCommands { get; }
		
		bool HasVisualizerCommands { get; }
		
		bool IsPinned { get; set; }
		
		bool SetText(string newValue);
	}
}
