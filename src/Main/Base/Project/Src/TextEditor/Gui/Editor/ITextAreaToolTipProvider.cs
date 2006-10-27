// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	/// <summary>
	/// Describes an object being able to provide tooltip information for
	/// the text editor.
	/// </summary>
	public interface ITextAreaToolTipProvider
	{
		/// <summary>
		/// Gets tooltip information for the specified position in the text area,
		/// if available.
		/// </summary>
		/// <returns><c>null</c>, if no tooltip information is available at this position, otherwise a ToolTipInfo object containing the tooltip information to be displayed.</returns>
		ToolTipInfo GetToolTipInfo(TextArea textArea, ToolTipRequestEventArgs e);
	}
	
	/// <summary>
	/// Contains information about a tooltip to be shown on the text area.
	/// </summary>
	public class ToolTipInfo
	{
		object toolTipObject;
		
		/// <summary>
		/// Gets the tool tip text to be displayed.
		/// May be <c>null</c>.
		/// </summary>
		public string ToolTipText {
			get {
				return this.toolTipObject as string;
			}
		}
		
		/// <summary>
		/// Gets the DebuggerGridControl to be shown as tooltip.
		/// May be <c>null</c>.
		/// </summary>
		public Control ToolTipControl {
			get {
				return this.toolTipObject as Control;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolTipInfo"/> class.
		/// </summary>
		/// <param name="toolTipText">The tooltip text to be displayed.</param>
		public ToolTipInfo(string toolTipText)
		{
			this.toolTipObject = toolTipText;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolTipInfo"/> class.
		/// </summary>
		/// <param name="toolTipControl">The DebuggerGridControl to be shown as tooltip.</param>
		public ToolTipInfo(Control toolTipControl)
		{
			this.toolTipObject = toolTipControl;
		}
	}
}
