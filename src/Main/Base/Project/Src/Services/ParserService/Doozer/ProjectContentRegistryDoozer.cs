// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates ProjectContentRegistryDescriptor objects for the parsing service.
	/// </summary>
	/// <attribute name="class">
	/// Name of the ProjectContentRegistry class.
	/// </attribute>
	/// <usage>Only in /Workspace/ProjectContentRegistry</usage>
	/// <returns>
	/// An RegistryDescriptor object that wraps a ProjectContentRegistry object.
	/// </returns>
	/// <conditions>Conditions are handled by the item, the condition "caller" will be the project to which
	/// the references requiring the registry belong to.
	/// You should always use the &lt;ProjectActive&gt; condition to restrict the project type
	/// your registry is used for.
	/// </conditions>
	public sealed class ProjectContentRegistryDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return true;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new ProjectContentRegistryDescriptor(codon);
		}
	}
}
