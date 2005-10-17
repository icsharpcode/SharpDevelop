// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates PadDescriptor objects for SharpDevelop pads.
	/// </summary>
	/// <attribute name="title">
	/// Title of the pad that is shown in the user interface.
	/// Should be a resource string, e.g. "${res:AddIns.HtmlHelp2.Contents}"
	/// </attribute>
	/// <attribute name="icon">
	/// Optional, specifies the name of the icon resource used for the pad.
	/// Pad icon resources must be registered with the ResourceService before the
	/// workbench is loaded!
	/// </attribute>
	/// <attribute name="category">
	/// Category of the pad. It is possible to create menu items that automatically
	/// contain show commands for all pads in a certain category.
	/// Pads in the category "Main" will show up in the "View" menu, the category
	/// "Tools" in the "View -&gt; Tools" menu, the category "Debugger" in the
	/// "View -&gt; Debugger" menu.
	/// </attribute>
	/// <attribute name="shortcut">
	/// Shortcut that activates the 'Show pad' command (e.g. "Control|Alt|T").
	/// </attribute>
	/// <attribute name="class">
	/// IPadContent class that is loaded when the pad content is shown for the first time.
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
