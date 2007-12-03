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
