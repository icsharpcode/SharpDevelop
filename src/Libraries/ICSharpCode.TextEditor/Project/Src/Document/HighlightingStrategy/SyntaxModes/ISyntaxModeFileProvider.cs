// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Xml;

namespace ICSharpCode.TextEditor.Document
{
	public interface ISyntaxModeFileProvider
	{
		ArrayList SyntaxModes {
			get;
		}
		
		XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode);
		void UpdateSyntaxModeList();
	}
}
