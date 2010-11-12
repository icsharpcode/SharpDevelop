// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;

using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Content of text editor tooltip (used as <see cref="ToolTipRequestEventArgs.ContentToShow"/>), 
	/// specifying whether it should be displayed in a WPF Popup.
	/// </summary>
	public interface ITooltip
	{
		/// <summary>
		/// Gets or sets the logical location within the document.
		/// </summary>
		Location LogicalPosition { get; set; }
		
		/// <summary>
		/// If true, this ITooltip will be displayed in a WPF Popup.
		/// Otherwise it will be displayed in a WPF Tooltip.
		/// WPF Popups are (unlike WPF Tooltips) focusable.
		/// </summary>
		bool ShowAsPopup { get; }
		
		/// <summary>
		/// Closes this tooltip.
		/// </summary>
		/// <param name="mouseClick">True if close request is raised 
		/// because of mouse click on some SharpDevelop GUI element.</param>
		/// <returns>True if Close succeeded (that is, can close). False otherwise.</returns>
		bool Close(bool mouseClick);
		
		/// <summary>
		/// Occurs when this tooltip decides to close.
		/// </summary>
		event RoutedEventHandler Closed;
	}
}
