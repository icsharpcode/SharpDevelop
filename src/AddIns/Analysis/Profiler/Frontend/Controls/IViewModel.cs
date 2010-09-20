// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ICSharpCode.Profiler.Controls
{
	public interface IViewModel<T>
	{
		bool IsExpanded { get; set; }
		ReadOnlyCollection<T> Children { get; }
		int VisibleElementCount { get; }
		Thickness IndentationMargin { get; }
		event EventHandler<NodeEventArgs<T>> VisibleChildExpandedChanged;
	}
	
	public class NodeEventArgs<T> : EventArgs	{
		public T Node { get; protected set; }
		
		protected NodeEventArgs() {}
		
		public NodeEventArgs(T node)
		{
			this.Node = node;
		}
	}
}
