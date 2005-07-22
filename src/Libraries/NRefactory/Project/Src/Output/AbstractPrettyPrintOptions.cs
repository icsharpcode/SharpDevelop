// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.PrettyPrinter
{
	/// <summary>
	/// Description of PrettyPrintOptions.	
	/// </summary>
	public class AbstractPrettyPrintOptions
	{
		char indentationChar = '\t';
		int  tabSize         = 4;
		int  indentSize      = 4;
		
		public char IndentationChar {
			get {
				return indentationChar;
			}
			set {
				indentationChar = value;
			}
		}
		
		public int TabSize {
			get {
				return tabSize;
			}
			set {
				tabSize = value;
			}
		}
		
		public int IndentSize {
			get {
				return indentSize;
			}
			set {
				indentSize = value;
			}
		}
	}
}
