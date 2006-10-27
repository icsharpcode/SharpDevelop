// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace SharpServerTools.Forms
{
	/// <summary>
	/// An IRebuildable can be asked to rebuild its Node tree
	/// </summary>
	public interface IRebuildable
	{	
		void Rebuild();
	}
	
	/// <summary>
	/// An IRequiresRebuildSource can request the ServerToolTreeView to rebuild
	/// it by emitting the RebuildRequiredEvent
	/// </summary>
	public interface IRequiresRebuildSource
	{
		event RebuildRequiredEventHandler RebuildRequiredEvent;	
	}
	
	public delegate void RebuildRequiredEventHandler(object sender, RebuildRequiredEventArgs e);
	
	/// <summary>
	/// An IRequiresRebuildSource should add a reference to itself to
	/// this event if it wants to be rebuilt.
	/// The parent of an IRequiresRebuildSource may or may not add itself
	/// and resend the event to the ServerToolTreeView depending on the
	/// relationship between parent and child.
	/// </summary>
	public class RebuildRequiredEventArgs: EventArgs
	{
		List<IRebuildable> rebuildNodes = new List<IRebuildable>();
		
		public IEnumerable<IRebuildable> Nodes {
			get {
				return rebuildNodes;
			}
		}
		
		public void AddNode(IRebuildable node)
		{
			rebuildNodes.Add(node);
		}
	}
}
