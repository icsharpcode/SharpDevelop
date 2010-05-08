// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
