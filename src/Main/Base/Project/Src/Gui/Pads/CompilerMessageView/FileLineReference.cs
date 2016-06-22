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

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Represents a file line and position reference.
	/// </summary>
	public class FileLineReference
	{
		/// <summary>
		/// The referenced filename.
		/// </summary>
		string fileName = String.Empty;
		
		/// <summary>
		/// The file line.
		/// </summary>
		int line;
		
		/// <summary>
		/// The line column.
		/// </summary>
		int column;
		
		
		/// <summary>
		/// Gets or sets the filename. 
		/// </summary>
		public string FileName {
			get	{ return fileName; }
			set	{ fileName = value; }
		}
		
		/// <summary>
		/// Gets or sets the line number. The first line has the number 1.
		/// The value '0' means that no line information is available.
		/// </summary>
		public int Line {
			get	{ return line; }
			set	{ line = value; }
		}	
		
		/// <summary>
		/// Gets or sets the line column. The first line has the number 1.
		/// The value '0' means that no column information is available.
		/// </summary>
		public int Column {
			get	{ return column; }
			set	{ column = value; }
		}			
		/// <summary>
		/// Creates a new instance of the <see cref="FileLineReference"/> class.
		/// </summary>
		/// <param name="fileName">The filename that the reference refers to.</param>
		/// <param name="line">The line number.</param>
		/// <param name="column">The line column.</param>
		public FileLineReference(string fileName, int line, int column)
		{
			this.fileName = fileName;
			this.line     = line;
			this.column   = column;
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="FileLineReference"/> class.
		/// </summary>
		/// <param name="fileName">The filename that the reference refers to.</param>
		/// <param name="line">The line number.</param>
		public FileLineReference(string fileName, int line) : this(fileName, line, 0)
		{
		}		
		
		/// <summary>
		/// Creates a new instance of the <see cref="FileLineReference"/> class.
		/// </summary>
		public FileLineReference()
		{
		}
		
	}
}
