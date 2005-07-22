// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Used in many methods as parameter, to shorten the parameter lists
	/// </summary>
	public class Position
	{
		int line = -1;
		int column = -1;
		ICompilationUnit cu;
		
		public int Line {
			get {
				return line;
			}
		}
		public int Column {
			get {
				return column;
			}
		}
		public ICompilationUnit Cu {
			get {
				return cu;
			}
		}
		
		public Position(ICompilationUnit cu, int line, int column)
		{
			this.line = line;
			this.column = column;
			this.cu = cu;
		}
	}
}
