// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
		object Content {
			get;
		}
		
		/// <summary>
		/// Re-initializes all components of the pad. Don't call unless
		/// you know what you do.
		/// </summary>
		void RedrawContent();
	}
}
