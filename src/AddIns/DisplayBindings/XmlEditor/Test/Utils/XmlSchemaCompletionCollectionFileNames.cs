// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
