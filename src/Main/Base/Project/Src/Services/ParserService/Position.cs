/*
 * Created by SharpDevelop.
 * User: Andrea
 * Date: 01.04.2004
 * Time: 14:56
 * 
 * To change this template use Tools | Options | File Templates.
 */

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
