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
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
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
	/// <usage>Only in /SharpDevelop/Workbench/ProjectBindings</usage>
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
