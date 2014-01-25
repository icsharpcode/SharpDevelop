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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Used to bind a NameValueListEditor with an MSBuild property that is a 
	/// list of name/value pairs separated by a semicolon. 
	/// (e.g. "DATADIR=C:\projects\data; SRCDIR=C:\projects\src")
	/// </summary>
	[Obsolete("XML Forms are obsolete")]
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
