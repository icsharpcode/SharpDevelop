// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// Represents an error found in the NAnt build file.
	/// </summary>
	public class NAntBuildFileError
	{
		string message = String.Empty;
		int line;
		int column;
		
		public NAntBuildFileError(string message, int line, int column)
		{
			this.message = message;
			this.line = line;
			this.column = column;
		}
		
		public string Message {
			get {
				return message;
			}
		}
		
		public int Column {
			get {
				return column;
			}
			set {
				column = value;
			}
		}
		public int Line {
			get {
				return line;
			}
			set {
				line = value;
			}
		}
		
	}
}
