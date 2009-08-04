// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Node that can be bound to DebuggerTooltipControl.
	/// </summary>
	public interface ITreeNode
	{
		string Name { get; }
		
		string Text { get; }
		
		string Type { get; }
		
		IEnumerable<ITreeNode> ChildNodes { get; }
		
		bool HasChildren { get; }
	}
}
