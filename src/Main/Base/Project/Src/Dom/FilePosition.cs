// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Description of SearchDefinition.
	/// </summary>
	public class FilePosition
	{
		string filename;
		Point position;
		
		public FilePosition(string filename, Point position)
		{
			this.filename = filename;
			this.position = position;
		}
		
		public string Filename {
			get {
				return filename;
			}
		}
		
		public Point Position {
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
				return position.X;
			}
		}
		
		public int Column {
			get {
				return position.X;
			}
		}
		
		public override bool Equals(object obj)
		{
			FilePosition b = obj as FilePosition;
			if (b == null) return false;
			return this.Filename == b.Filename && this.Position == b.Position;
		}
		
		public override int GetHashCode()
		{
			return filename.GetHashCode() ^ position.GetHashCode();
		}
	}
}
