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
using System.Text;

namespace ICSharpCode.Scripting.Tests.Utils
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
