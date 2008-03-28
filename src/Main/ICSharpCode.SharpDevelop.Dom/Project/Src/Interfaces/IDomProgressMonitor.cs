// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
		/// <summary>
	/// This is a basic interface to a "progress bar" type of
	/// control.
	/// </summary>
	public interface IDomProgressMonitor
	{
		/// <summary>
		/// Gets/sets if the task current shows a modal dialog. Set this property to true to make progress
		/// dialogs windows temporarily invisible while your modal dialog is showing.
		/// </summary>
		bool ShowingDialog {
			get;
			set;
		}
	}
}
