// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// <attribute name="defaultPosition" use="optional">
	/// Default position of the pad, as a ICSharpCode.SharpDevelop.DefaultPadPositions enum value (e.g. "Bottom, Hidden").
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
		
		public object BuildItem(BuildItemArgs args)
		{
			return new PadDescriptor(args.Codon);
		}
	}
}
