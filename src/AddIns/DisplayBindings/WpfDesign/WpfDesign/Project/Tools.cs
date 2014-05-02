// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		DesignPanelHitTestResult HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, HitTestType hitTestType);

		/// <summary>
		/// Performs a hit test on the design surface, raising <paramref name="callback"/> for each match.
		/// Hit testing continues while the callback returns true.
		/// </summary>
		void HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, Predicate<DesignPanelHitTestResult> callback, HitTestType hitTestType);

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
