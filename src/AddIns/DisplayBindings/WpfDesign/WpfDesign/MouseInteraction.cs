// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2415 $</version>
// </file>

using System;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign
{
	// Interfaces for mouse interaction on the design surface.

	/// <summary>
	/// Behavior interface implemented by elements to handle the mouse down event
	/// on them.
	/// </summary>
	public interface IHandlePointerToolMouseDown
	{
		/// <summary>
		/// Called to handle the mouse down event.
		/// </summary>
		void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result);
	}
}
