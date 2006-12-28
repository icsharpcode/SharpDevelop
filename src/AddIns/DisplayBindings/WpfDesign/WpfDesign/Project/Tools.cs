// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using System.Windows;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Describes the layer of input handling.
	/// When multiple actions are possible, that with the highest layer will be used.
	/// </summary>
	public enum InputHandlingLayer
	{
		/// <summary>
		/// No layer specified. This layer is lower than all other layers.
		/// </summary>
		None,
		/// <summary>
		/// Layer used for passing the input to the component.
		/// Normally never receives actions because there is always a <see cref="Tool"/>.
		/// </summary>
		Component,
		/// <summary>
		/// Layer used for tools.
		/// </summary>
		Tool,
		/// <summary>
		/// Layer used for certain components that should get input, for example scroll thumbs
		/// in user-defined ScrollViewers and the headers inside a TabControl.
		/// </summary>
		ComponentHigh,
		/// <summary>
		/// This layer is higher than all other layers.
		/// </summary>
		Highest
	}
	
	/// <summary>
	/// Describes a tool that can handle input on the design surface.
	/// Modelled after the description on http://urbanpotato.net/Default.aspx/document/2300
	/// </summary>
	public interface ITool
	{
		/// <summary>
		/// Gets the input handling layer of the tool.
		/// </summary>
		InputHandlingLayer InputLayer { get; }
		
		/// <summary>
		/// Gets the cursor used by the tool.
		/// </summary>
		Cursor Cursor { get; }
		
		/// <summary>
		/// Notifies the tool of the MouseDown event.
		/// </summary>
		void OnMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e);
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
	}
	
	/// <summary>
	/// Interface for the design panel. The design panel is the UIElement containing the
	/// designed elements and is responsible for handling mouse and keyboard events.
	/// </summary>
	public interface IDesignPanel : IInputElement
	{
		/// <summary>
		/// Gets the service provider used by the DesignPanel.
		/// </summary>
		DefaultServiceProvider Services { get; }
		
		/// <summary>
		/// Starts an input action. This prevents components and tools from getting input events,
		/// leaving input handling to event handlers attached to the design panel.
		/// </summary>
		void StartInputAction();
		
		/// <summary>
		/// Stops an input action. This reenables input handling of
		/// </summary>
		void StopInputAction();
		
		/// <summary>
		/// Finds the designed element for the specified original source.
		/// </summary>
		DesignSite FindDesignedElementForOriginalSource(object originalSource);
		
		
		// The following members were missing in <see cref="IInputElement"/>, but of course
		// are supported on the DesignPanel:
		
		/// <summary>
		/// Occurs when a mouse button is pressed.
		/// </summary>
		event MouseButtonEventHandler MouseDown;
		
		/// <summary>
		/// Occurs when a mouse button is released.
		/// </summary>
		event MouseButtonEventHandler MouseUp;
	}
}
