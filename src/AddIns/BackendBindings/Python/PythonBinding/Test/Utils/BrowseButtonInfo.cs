// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
