// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
