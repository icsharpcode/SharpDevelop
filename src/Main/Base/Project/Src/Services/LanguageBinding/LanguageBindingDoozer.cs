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
	/// Creates LanguageBindingDescriptor objects for the project service.
	/// </summary>
	/// <attribute name="guid" use="required">
	/// Project type GUID of the project used by MsBuild.
	/// </attribute>
	/// <attribute name="supportedextensions" use="required">
	/// Semicolon-separated list of file extensions that are compilable files in the project. (e.g. ".boo")
	/// </attribute>
	/// <attribute name="projectfileextension" use="required">
	/// File extension of project files. (e.g. ".booproj")
	/// </attribute>
	/// <attribute name="class" use="required">
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
