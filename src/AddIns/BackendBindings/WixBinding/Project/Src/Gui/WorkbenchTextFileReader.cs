// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Tries to create a text reader for the text currently open in a view
	/// in the workbench, otherwise it creates a normal text reader to read it
	/// from the file system.
	/// </summary>
	public class WorkbenchTextFileReader : ITextFileReader
	{
		public WorkbenchTextFileReader()
		{
		}
		
		/// <summary>
		/// Creates a TextReader for the specified file.
		/// </summary>
		public TextReader Create(string fileName)
		{
			return ParserService.GetParseableFileContent(fileName).CreateReader();
		}
	}
}
