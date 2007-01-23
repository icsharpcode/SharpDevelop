// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Used to bind a NameValueListEditor with an MSBuild property that is a 
	/// list of name/value pairs separated by a semicolon. 
	/// (e.g. "DATADIR=C:\projects\data; SRCDIR=C:\projects\src")
	/// </summary>
	public class SemicolonSeparatedNameValueListBinding : ConfigurationGuiBinding
	{
		NameValueListEditor editor;
		
		public SemicolonSeparatedNameValueListBinding(NameValueListEditor editor)
		{
			this.editor = editor;
			TreatPropertyValueAsLiteral = false;
		}
		
		public override void Load()
		{
			editor.LoadList(Get(String.Empty));
		}
		
		public override bool Save()
		{
			Set(editor.GetList());
			return true;
		}
	}
}
