// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
