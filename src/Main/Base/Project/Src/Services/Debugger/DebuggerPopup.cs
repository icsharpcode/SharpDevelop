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
		//private DebuggerPopup parentPopup;
		
		public DebuggerPopup()
		{
			this.contentControl = new DebuggerTooltipControl();
			this.contentControl.containingPopup = this;
			this.Child = this.contentControl;
			// to handle closed by lost focus
			this.Closed += DebuggerPopup_Closed;
		}

		void DebuggerPopup_Closed(object sender, EventArgs e)
		{
			LoggingService.Debug("DebuggerPopup_Closed");
		}
		
		public IEnumerable<ITreeNode> ItemsSource
		{
			get { return this.contentControl.ItemsSource; }
			set { this.contentControl.ItemsSource = value;  }
		}
		
		public void Open()
		{
			this.IsOpen = true;
		}
		
		public void Close()
		{
			this.contentControl.CloseChildPopup();
			this.IsOpen = false;
		}
	}
}
