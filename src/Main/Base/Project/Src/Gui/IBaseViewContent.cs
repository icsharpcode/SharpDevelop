// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The base functionalty all view contents must provide
	/// </summary>
	public interface IBaseViewContent : IDisposable
	{
		/// <summary>
		/// This is the Windows.Forms control for the view.
		/// </summary>
		Control Control {
			get;
		}
		
		/// <summary>
		/// The workbench window in which this view is displayed.
		/// </summary>
		IWorkbenchWindow  WorkbenchWindow {
			get;
			set;
		}
		
		/// <summary>
		/// The text on the tab page when more than one view content
		/// is attached to a single window.
		/// </summary>
		string TabPageText {
			get;
		}
		
		/// <summary>
		/// Is called when the window is switched to.
		/// -> Inside the tab (Called before Selected())
		/// -> Inside the workbench.
		/// </summary>
		void SwitchedTo();
		
		/// <summary>
		/// Is called when the view content is selected inside the window
		/// tab. NOT when the windows is selected.
		/// </summary>
		void Selected();
		
		/// <summary>
		/// Is called when the view content is deselected inside the window
		/// tab before the other window is selected. NOT when the windows is deselected.
		/// </summary>
		void Deselected();
		
		/// <summary>
		/// Reinitializes the content. (Re-initializes all add-in tree stuff)
		/// and redraws the content. Call this not directly unless you know
		/// what you do.
		/// </summary>
		void RedrawContent();
	}
}
