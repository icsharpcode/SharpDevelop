// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using ICSharpCode.SharpDevelop.Editor.ContextActions;

namespace ICSharpCode.AvalonEdit.AddIn.ContextActions
{
	/// <summary>
	/// 'Paste from clipboard ring' command
	/// </summary>
	public class ClipboardRingPopup : ContextActionsPopup
	{
		ToolTip toolTip;
		
		public ClipboardRingPopup() : base()
		{
			this.toolTip = new ToolTip();
			this.toolTip.PlacementTarget = this.ActionsControl;
			this.toolTip.Placement = PlacementMode.Right;
			this.toolTip.Closed += new RoutedEventHandler(ClipboardRingPopup_Closed);
			
			this.ActionsControl.ActionExecuted += delegate
			{
				if(this.toolTip != null)
					this.toolTip.IsOpen = false;
			};
			this.ActionsControl.ActionSelected += new RoutedEventHandler(ClipboardRingPopup_ActionSelected);
			this.ActionsControl.ActionUnselected += new RoutedEventHandler(ClipboardRingPopup_ActionUnselected);
		}
		
		#region ToolTip handling
		
		string ExtractTextFromEvent(RoutedEventArgs args)
		{
			var button = args.Source as Button;
			if (button == null)
				return null;
			
			var command = button.Command as ContextActionCommand;
			if (command == null)
				return null;
			
			var clipboardRingAction = command.ContextAction as ClipboardRingAction;
			if (clipboardRingAction == null)
				return null;
			
			return clipboardRingAction.Text;
		}
		
		void ClipboardRingPopup_ActionSelected(object sender, RoutedEventArgs e)
		{
			if (this.toolTip != null) {
				var text = ExtractTextFromEvent(e);
				this.toolTip.Content = text;
				this.toolTip.IsOpen = (text != null);
			}
		}
		
		void ClipboardRingPopup_ActionUnselected(object sender, RoutedEventArgs e)
		{
		}
		
		void ClipboardRingPopup_Closed(object sender, RoutedEventArgs e)
		{
			if(toolTip != null)
				toolTip.Content = null;
		}
		
		#endregion
	}
}
