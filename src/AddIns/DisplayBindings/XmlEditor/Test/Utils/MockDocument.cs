// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2498 $</version>
// </file>

using ICSharpCode.TextEditor.Document;
using System;
using System.Collections.Generic;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Helper class that implements the Text Editor library's IDocument interface.
	/// </summary>
	public class MockDocument
	{
		public static IDocument Create()
		{
			return new DocumentFactory().CreateDocument();
		}
	}
}
