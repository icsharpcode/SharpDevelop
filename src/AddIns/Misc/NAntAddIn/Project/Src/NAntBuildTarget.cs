// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NAntAddIn
{
	/// <summary>
	/// Represents a NAnt build target.
	/// </summary>
	public class NAntBuildTarget
	{
		string name = String.Empty;
		bool isDefault;
		int line;
		int column;
		
		public NAntBuildTarget()
		{
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="NAntBuildTarget"/>
		/// with the specified name.
		/// </summary>
		/// <param name="name">The target name.</param>
		/// <param name="isDefault"><see langword="true"/> if the 
		/// target is the default target; otherwise 
		/// <see langword="false"/>.</param>
		/// <param name="line">The line number of the target element
		/// in the build file.</param>
		/// <param name="col">The column number of the target element
		/// in the build file.</param>
		public NAntBuildTarget(string name, bool isDefault, int line, int col)
		{
			this.name = name;
			this.isDefault = isDefault;
			this.line = line;
			this.column = col;
		}
		
		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// Gets whether this is the default target.
		/// </summary>
		public bool IsDefault {
			get {
				return isDefault;
			}
		}
		
		/// <summary>
		/// Gets the line in the build file where this
		/// target can be found.
		/// </summary>
		public int Line {
			get {
				return line;
			}
		}
		
		/// <summary>
		/// Gets the column in the build file where this
		/// target can be found.
		/// </summary>
		public int Column {
			get {
				return column;
			}
		}
	}
}
