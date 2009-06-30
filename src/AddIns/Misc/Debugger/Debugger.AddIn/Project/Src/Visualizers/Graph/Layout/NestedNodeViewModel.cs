// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// ViewModel base for node in tree of properties, to be bound to View (ie. PositionedGraphNodeControl).
	/// </summary>
	public class NestedNodeViewModel
	{
		public string Name { get; set; }
		public string Text { get; set; }
		
		private List<NestedNodeViewModel> children = new List<NestedNodeViewModel>();		
		public List<NestedNodeViewModel> Children { get { return this.children; } }
	
		/// <summary>
		/// Is this expandable node?
		/// </summary>
		public bool IsNested { get; set; }
		/// <summary>
		/// Does this node have any children?
		/// </summary>
		public bool HasChildren { get { return this.Children.Count > 0; } }
		
		/// <summary>
		/// Show a button to expand property associated with this node?
		/// </summary>
		public bool ShowExpandPropertyButton
		{
			get 
			{
				var thisAsPropertyNode = this as PropertyNodeViewModel;
				if (thisAsPropertyNode == null) 
				{
					// this is NestedNodeViewModel -> no property, don't show expand button
					return false;
				}
				else
				{
					// this is PositionedNodeViewModel -> show expand button when appropriate
					PositionedNodeProperty property = thisAsPropertyNode.Property;
					return (!property.IsAtomic && !property.IsNull);
				}
			}
		}
		
		// if we bound this ViewModel to a TreeView, this would not be needed,
		// it is added "artificially", to support PositionedGraphNodeControl
		public bool IsExpanded { get; set; }	
		
		public NestedNodeViewModel()
		{
		}
	}
}
