// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Dom
{
	public struct FilePosition
	{
		string filename;
		Location position;
		ICompilationUnit compilationUnit;
		
		public static readonly FilePosition Empty = new FilePosition(null, Location.Empty);
		
		public FilePosition(ICompilationUnit compilationUnit, int line, int column)
		{
			this.position = new Location(column, line);
			this.compilationUnit = compilationUnit;
			if (compilationUnit != null) {
				this.filename = compilationUnit.FileName;
			} else {
				this.filename = null;
			}
		}
		
		public FilePosition(string filename)
			: this(filename, Location.Empty)
		{
		}
		
		public FilePosition(string filename, int line, int column)
			: this(filename, new Location(column, line))
		{
		}
		
		public FilePosition(string filename, Location position)
		{
			this.compilationUnit = null;
			this.filename = filename;
			this.position = position;
		}
		
		public string FileName {
			get {
				return filename;
			}
		}
		
		public ICompilationUnit CompilationUnit {
			get {
				return compilationUnit;
			}
		}
		
		public Location Position {
			get {
				return position;
			}
		}
		
		public override string ToString()
		{
			return String.Format("{0} : (line {1}, col {2})",
			                     filename,
			                     Line,
			                     Column);
		}
		
		public int Line {
			get {
				return position.Y;
			}
		}
		
		public int Column {
			get {
				return position.X;
			}
		}
		
		public bool IsEmpty {
			get {
				return filename == null;
			}
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is FilePosition)) return false;
			FilePosition b = (FilePosition)obj;
			return this.FileName == b.FileName && this.Position == b.Position;
		}
		
		public override int GetHashCode()
		{
			return filename.GetHashCode() ^ position.GetHashCode();
		}
	}
}
