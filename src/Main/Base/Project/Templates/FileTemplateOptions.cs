// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Templates
{
	/// <summary>
	/// Argument class for file templates.
	/// </summary>
	public class FileTemplateOptions
	{
		/// <summary>
		/// Gets/Sets whether the file being created will be untitled.
		/// </summary>
		public bool IsUntitled { get; set; }
		
		/// <summary>
		/// The parent project to which this file is added.
		/// Can be null when creating a file outside of a project.
		/// </summary>
		public IProject Project { get; set; }
		
		/// <summary>
		/// The name of the file
		/// </summary>
		public FileName FileName { get; set; }
		
		/// <summary>
		/// The default namespace to use for the newly created file.
		/// </summary>
		public string Namespace { get; set; }
		
		/// <summary>
		/// The class name (generated from the file name).
		/// </summary>
		public string ClassName { get; set; }
		
		/// <summary>
		/// The object that was created by <see cref="FileTemplate.CreateCustomizationObject"/>
		/// and subsequently customized by the user in the dialog's property grid.
		/// </summary>
		public object CustomizationObject { get; set; }
	}
}
