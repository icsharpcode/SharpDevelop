//
// SharpDevelop
//
// Copyright (C) 2004 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

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
		int line = 0;
		
		/// <summary>
		/// The line column.
		/// </summary>
		int column = 0;
		
		
		/// <summary>
		/// Gets or sets the filename. 
		/// </summary>
		public string FileName {
			get	{
				return fileName;
			}
			set	{
				fileName = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the line number 
		/// </summary>
		public int Line {
			get	{
				return line;
			}
			set	{
				line = value;
			}
		}	
		
		/// <summary>
		/// Gets or sets the line column. 
		/// </summary>
		public int Column {
			get	{
				return column;
			}
			set	{
				column = value;
			}
		}			
		/// <summary>
		/// Creates a new instance of the <see cref="FileLineReference"/> class.
		/// </summary>
		/// <param name="filename">The filename that the reference refers to.</param>
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
		/// <param name="filename">The filename that the reference refers to.</param>
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
