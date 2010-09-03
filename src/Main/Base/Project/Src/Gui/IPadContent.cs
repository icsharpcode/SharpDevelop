// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The IPadContent interface is the basic interface to all "tool" windows
	/// in SharpDevelop.
	/// </summary>
	public interface IPadContent : IDisposable
	{
		/// <summary>
		/// This is the UI element for the view.
		/// You can use both Windows.Forms and WPF controls.
		/// </summary>
		object Control {
			get;
		}
		
		/// <summary>
		/// Gets the control which has focus initially.
		/// </summary>
		object InitiallyFocusedControl {
			get;
		}
	}
}
