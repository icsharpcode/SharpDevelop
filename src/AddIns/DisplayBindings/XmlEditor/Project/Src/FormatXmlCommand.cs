// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Pretty prints the xml.
	/// </summary>
	public class FormatXmlCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
			if (textEditor != null) {
				XmlView.FormatXml(textEditor);
			}
		}
	}
}
