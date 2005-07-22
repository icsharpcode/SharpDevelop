// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
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
	}
}
