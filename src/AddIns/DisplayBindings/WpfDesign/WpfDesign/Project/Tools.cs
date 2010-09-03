// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.WpfDesign.Adorners;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Describes a tool that can handle input on the design surface.
	/// Modelled after the description on http://urbanpotato.net/Default.aspx/document/2300
	/// </summary>
	public interface ITool
	{
		/// <summary>
		/// Gets the cursor used by the tool.
		/// </summary>
		Cursor Cursor { get; }
		
		/// <summary>
		/// Activates the tool, attaching its event handlers to the design panel.
		/// </summary>
		void Activate(IDesignPanel designPanel);
		
		/// <summary>
		/// Deactivates the tool, detaching its event handlers from the design panel.
		/// </summary>
		void Deactivate(IDesignPanel designPanel);
	}
	
	/// <summary>
	/// Service that manages tool selection.
	/// </summary>
	public interface IToolService
	{
		/// <summary>
		/// Gets the 'pointer' tool.
		/// The pointer tool is the default tool for selecting and moving elements.
		/// </summary>
		ITool PointerTool { get; }
		
		/// <summary>
		/// Gets/Sets the currently selected tool.
		/// </summary>
		ITool CurrentTool { get; set; }
		
		/// <summary>
		/// Is raised when the current tool changes.
		/// </summary>
		event EventHandler CurrentToolChanged;
	}
	
	/// <summary>
	/// Interface for the design panel. The design panel is the UIElement containing the
	/// designed elements and is responsible for handling mouse and keyboard events.
	/// </summary>
	public interface IDesignPanel : IInputElement
	{
		/// <summary>
		/// Gets the design context used by the DesignPanel.
		/// </summary>
		DesignContext Context { get; }
		
		/// <summary>
		/// Gets/Sets if the design content is visible for hit-testing purposes.
		/// </summary>
		bool IsContentHitTestVisible { get; set; }
		
		/// <summary>
		/// Gets/Sets if the adorner layer is visible for hit-testing purposes.
		/// </summary>
		bool IsAdornerLayerHitTestVisible { get; set; }
		
		/// <summary>
		/// Gets the list of adorners displayed on the design panel.
		/// </summary>
		ICollection<AdornerPanel> Adorners { get; }
		
		/// <summary>
		/// Performs a hit test on the design surface.
		/// </summary>
		DesignPanelHitTestResult HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface);
		
		/// <summary>
		/// Performs a hit test on the design surface, raising <paramref name="callback"/> for each match.
		/// Hit testing continues while the callback returns true.
		/// </summary>
		void HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, Predicate<DesignPanelHitTestResult> callback);
		
		// The following members were missing in <see cref="IInputElement"/>, but
		// are supported on the DesignPanel:
		
		/// <summary>
		/// Occurs when a mouse button is pressed.
		/// </summary>
		event MouseButtonEventHandler MouseDown;
		
		/// <summary>
		/// Occurs when a mouse button is released.
		/// </summary>
		event MouseButtonEventHandler MouseUp;
		
		/// <summary>
		/// Occurs when a drag operation enters the design panel.
		/// </summary>
		event DragEventHandler DragEnter;
		
		/// <summary>
		/// Occurs when a drag operation is over the design panel.
		/// </summary>
		event DragEventHandler DragOver;
		
		/// <summary>
		/// Occurs when a drag operation leaves the design panel.
		/// </summary>
		event DragEventHandler DragLeave;
		
		/// <summary>
		/// Occurs when an element is dropped on the design panel.
		/// </summary>
		event DragEventHandler Drop;
	}
}
