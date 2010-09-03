// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
