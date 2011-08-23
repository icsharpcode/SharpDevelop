// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates ProjectBindingDescriptor objects for the project service.
	/// </summary>
	/// <attribute name="guid" use="required">
	/// Project type GUID of the project used by MSBuild.
	/// </attribute>
	/// <attribute name="supportedextensions" use="required">
	/// Semicolon-separated list of file extensions that are compilable files in the project. (e.g. ".boo")
	/// </attribute>
	/// <attribute name="projectfileextension" use="required">
	/// File extension of project files. (e.g. ".booproj")
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the IProjectBinding class.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/ProjectBinding</usage>
	/// <returns>
	/// A ProjectBindingDescriptor object that wraps the IProjectBinding object.
	/// </returns>
	public class ProjectBindingDoozer : IDoozer
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
		public object BuildItem(BuildItemArgs args)
		{
			return new ProjectBindingDescriptor(args.Codon);
		}
	}
}
