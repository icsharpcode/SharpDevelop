// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
