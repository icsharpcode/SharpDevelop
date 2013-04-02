// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Templates
{
	public abstract class FileTemplate
	{
		public abstract string Name { get; }
		public abstract string Category { get; }
		public abstract string Subcategory { get; }
		public abstract string Description { get; }
		public abstract IImage Icon { get; }
		
		public virtual bool NewFileDialogVisible { get { return true; } }
		
		/// <summary>
		/// Proposes a name for the new file.
		/// </summary>
		/// <param name="basePath">The target directory where the new file will be placed.
		/// <c>null</c> if an untitled file will be created.</param>
		public abstract string SuggestFileName(DirectoryName basePath);
		
		/// <summary>
		/// Creates an object with the customizable properties for this template.
		/// The NewFileDialog allows changing the properties of this object using a property grid.
		/// </summary>
		public virtual object CreateCustomizationObject()
		{
			return null;
		}
	}
}
