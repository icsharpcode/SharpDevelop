// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
