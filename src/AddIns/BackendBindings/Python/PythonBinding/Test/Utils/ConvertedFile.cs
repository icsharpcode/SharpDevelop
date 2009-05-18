// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Stores the filename and the code for the converted file.
	/// </summary>
	public class ConvertedFile 
	{
		public string FileName;
		public string Text;
		public Encoding Encoding;
		
		public ConvertedFile(string fileName, string text, Encoding encoding)
		{
			this.FileName = fileName;
			this.Text = text;
			this.Encoding = encoding;
		}
		
		public override string ToString()
		{
			return "FileName: " + FileName + "\r\n" +
				"Encoding: " + Encoding + "\r\n" +
				"Text: " + Text;
		}
		
		public override bool Equals(object obj)
		{
			ConvertedFile convertedFile = obj as ConvertedFile;
			if (convertedFile != null) {
				return FileName == convertedFile.FileName && Text == convertedFile.Text && Encoding == convertedFile.Encoding;
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return FileName.GetHashCode();
		}
	}
}
