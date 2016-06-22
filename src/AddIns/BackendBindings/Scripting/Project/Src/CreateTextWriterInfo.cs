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
using System.IO;
using System.Text;

namespace ICSharpCode.Scripting
{
	public class CreateTextWriterInfo
	{
		string fileName;
		Encoding encoding;
		bool append;
		
		public CreateTextWriterInfo(string fileName, Encoding encoding, bool append)
		{
			this.fileName = fileName;
			this.encoding = encoding;
			this.append = append;
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public Encoding Encoding {
			get { return encoding; }
		}
		
		public bool Append {
			get { return append; }
		}
		
		public override bool Equals(object obj)
		{
			CreateTextWriterInfo rhs = obj as CreateTextWriterInfo;
			if (rhs != null) {
				return Equals(rhs);
			}
			return false;
		}
		
		bool Equals(CreateTextWriterInfo rhs)
		{
			return (fileName == rhs.fileName) &&
				(encoding == rhs.encoding) &&
				(append == rhs.append);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public TextWriter CreateTextWriter()
		{
			return new StreamWriter(fileName, append, encoding);
		}
	}
}
