// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Interface defining the tab order mode.ning the tab order mode.
	/// </summary>
	public interface ITabOrder
	{
		/// <summary>
		/// Checks if tab order mode is active
		/// </summary>
		bool IsTabOrderMode { get; }

		/// <summary>
		/// Sets the next tab index if over a control.
		/// </summary>
		void SetNextTabIndex(Point p);
		
		/// <summary>
		/// Sets the previous tab index if over a control.
		/// </summary>
		void SetPrevTabIndex(Point p);
		
		/// <summary>
		/// Show tab order.
		/// </summary>
		void ShowTabOrder();
		
		/// <summary>
		/// Show tab order.
		/// </summary>
		void HideTabOrder();
		
	}
}
