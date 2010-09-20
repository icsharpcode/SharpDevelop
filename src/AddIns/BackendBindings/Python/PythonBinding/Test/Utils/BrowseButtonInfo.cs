// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Holds information about the property associated with a 
	/// browse button in an options panel.
	/// </summary>
	public struct BrowseButtonInfo 
	{
		public string Target;
		public string FileFilter; 
		public TextBoxEditMode TextBoxEditMode;
		
		public BrowseButtonInfo(string target, string fileFilter, TextBoxEditMode textBoxEditMode)
		{
			Target = target;
			FileFilter = fileFilter;
			TextBoxEditMode = textBoxEditMode;
		}
	}
}
