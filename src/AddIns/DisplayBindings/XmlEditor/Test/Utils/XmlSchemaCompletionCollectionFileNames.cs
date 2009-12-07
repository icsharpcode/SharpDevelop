// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	public class XmlSchemaCompletionCollectionFileNames
	{
		XmlSchemaCompletionCollectionFileNames()
		{
		}
		
		public static string[] GetFileNames(XmlSchemaCompletionCollection schemas)
		{
			List<string> fileNames = new List<string>();
			foreach (XmlSchemaCompletion schema in schemas) {
				fileNames.Add(schema.FileName);
			}
			return fileNames.ToArray();
		}
	}
}
