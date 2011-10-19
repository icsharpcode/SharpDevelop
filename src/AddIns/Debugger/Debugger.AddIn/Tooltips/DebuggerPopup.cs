// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;

namespace Debugger.AddIn.Tooltips
{
	/// <summary>
	/// Popup containing <see cref="DebuggerTooltipControl"></see>.
	/// </summary>
	public class DebuggerPopup : Popup
	{
		internal DebuggerTooltipControl innerControl;

		public DebuggerPopup(DebuggerTooltipControl parentControl, Location logicalPosition, bool showPins = true)
		{
			this.innerControl = new DebuggerTooltipControl(parentControl, logicalPosition) { ShowPins = showPins };
			this.innerControl.containingPopup = this;
			this.Child = this.innerControl;
			this.IsLeaf = false;
			
			//this.KeyDown += new KeyEventHandler(DebuggerPopup_KeyDown);

			//this.innerControl.Focusable = true;
			//Keyboard.Focus(this.innerControl);
			//this.AllowsTransparency = true;
			//this.PopupAnimation = PopupAnimation.Slide;
		}

		// attempt to propagate shortcuts to main windows when Popup is focusable (needed for keyboard scrolling + editing)
		/*void DebuggerPopup_KeyDown(object sender, KeyEventArgs e)
		{
			LoggingService.Debug("Unhandled popup key down: " + e.Key);
			RaiseEventPair(WorkbenchSingleton.MainWindow, PreviewKeyDownEvent, KeyDownEvent,
			                           new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key));
		}
		
		// copied from CompletionWindowBase
		static bool RaiseEventPair(UIElement target, RoutedEvent previewEvent, RoutedEvent @event, RoutedEventArgs args)
		{
			if (target == null)
				throw new ArgumentNullException("target");
			if (previewEvent == null)
				throw new ArgumentNullException("previewEvent");
			if (@event == null)
				throw new ArgumentNullException("event");
			if (args == null)
				throw new ArgumentNullException("args");
			args.RoutedEvent = previewEvent;
			target.RaiseEvent(args);
			args.RoutedEvent = @event;
			target.RaiseEvent(args);
			return args.Handled;
		}*/

		public IEnumerable<ITreeNode> ItemsSource
		{
			get { return this.innerControl.ItemsSource; }
			set { this.innerControl.SetItemsSource(value); }
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
			if (isLeaf) {
				this.innerControl.CloseOnLostFocus();
			}
		}

		public void Open()
		{
			this.IsOpen = true;
		}

		public void CloseSelfAndChildren()
		{
			this.innerControl.CloseChildPopups();
			this.IsOpen = false;
		}
	}
}