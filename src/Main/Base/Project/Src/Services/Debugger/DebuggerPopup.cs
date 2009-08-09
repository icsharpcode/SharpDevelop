// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Popup containing <see cref="DebuggerTooltipControl"></see>.
	/// </summary>
	public class DebuggerPopup : Popup
	{
		private DebuggerTooltipControl contentControl;
		
		public DebuggerPopup(DebuggerTooltipControl parentControl)
		{
			this.contentControl = new DebuggerTooltipControl(parentControl);
			this.contentControl.containingPopup = this;
			this.Child = this.contentControl;
			this.IsLeaf = false;
			//this.AllowsTransparency = true;
			//this.PopupAnimation = PopupAnimation.Slide;
		}
		
		public IEnumerable<ITreeNode> ItemsSource
		{
			get { return this.contentControl.ItemsSource; }
			set { this.contentControl.ItemsSource = value;  }
		}
		
		private bool isLeaf;
		public bool IsLeaf
		{
			get { return isLeaf; }
			set
			{
				isLeaf = value;
				// leaf popup closes on lost focus
				this.StaysOpen = !isLeaf;
			}
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (isLeaf)
			{
				this.contentControl.CloseOnLostFocus();
			}
		}
		
		public void Open()
		{
			this.IsOpen = true;
		}
		
		public void CloseSelfAndChildren()
		{
			this.contentControl.CloseChildPopups();
			this.IsOpen = false;
		}
	}
}
