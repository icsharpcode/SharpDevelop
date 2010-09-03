// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Holds information about the property associated with a 
	/// browse folder button in an options panel.
	/// </summary>
	public struct BrowseFolderButtonInfo 
	{
		public string Target;
		public string Description; 
		public TextBoxEditMode TextBoxEditMode;
		
		public BrowseFolderButtonInfo(string target, string description, TextBoxEditMode textBoxEditMode)
		{
			Target = target;
			Description = description;
			TextBoxEditMode = textBoxEditMode;
		}
	}
}
