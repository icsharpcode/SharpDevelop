// OutputFormatter.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
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
