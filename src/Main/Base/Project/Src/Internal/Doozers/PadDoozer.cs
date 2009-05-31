// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates PadDescriptor objects for SharpDevelop pads.
	/// </summary>
	/// <attribute name="class" use="required">
	/// IPadContent class that is loaded when the pad content is shown for the first time.
	/// </attribute>
	/// <attribute name="title" use="required">
	/// Title of the pad that is shown in the user interface.
	/// Should be a resource string, e.g. "${res:AddIns.HtmlHelp2.Contents}"
	/// </attribute>
	/// <attribute name="icon" use="optional">
	/// Specifies the name of the icon resource used for the pad.
	/// Pad icon resources must be registered with the ResourceService before the
	/// workbench is loaded!
	/// </attribute>
	/// <attribute name="category" use="optional">
	/// Category of the pad. It is possible to create menu items that automatically
	/// contain show commands for all pads in a certain category.
	/// Pads in the category "Main" will show up in the "View" menu, the category
	/// "Tools" in the "View -&gt; Tools" menu, the category "Debugger" in the
	/// "View -&gt; Debugger" menu.
	/// </attribute>
	/// <attribute name="shortcut" use="optional">
	/// Shortcut that activates the 'Show pad' command (e.g. "Control|Alt|T").
	/// </attribute>
	/// <usage>Only in /Workspace/Parser</usage>
	/// <returns>
	/// An PadDescriptor object that wraps the IPadContent object.
	/// </returns>
	public class PadDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new PadDescriptor(codon);
		}
	}
}
