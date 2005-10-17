// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates LanguageBindingDescriptor objects for the project service.
	/// </summary>
	/// <attribute name="guid">
	/// Project type GUID of the project used by MsBuild.
	/// </attribute>
	/// <attribute name="supportedextensions">
	/// Semicolon-separated list of file extensions that are compilable files in the project. (e.g. ".boo")
	/// </attribute>
	/// <attribute name="projectfileextension">
	/// File extension of project files. (e.g. ".booproj")
	/// </attribute>
	/// <attribute name="class">
	/// Name of the ILanguageBinding class.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/LanguageBindings</usage>
	/// <returns>
	/// An LanguageBindingDescriptor object that wraps the ILanguageBinding object.
	/// </returns>
	public class LanguageBindingDoozer : IDoozer
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
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new LanguageBindingDescriptor(codon);
		}
	}
}
