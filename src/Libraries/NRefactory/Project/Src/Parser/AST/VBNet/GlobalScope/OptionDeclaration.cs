// UsingAliasDeclaration.cs
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
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	///<summary>
	/// Compare type, used in the <c>Option Compare</c>
	/// pragma (VB only).
	///</summary>
	public enum OptionType
	{
		None,
		Explicit,
		Strict,
		CompareBinary,
		CompareText
	}
	
	public class OptionDeclaration : AbstractNode
	{
		OptionType optionType;
		bool       optionvalue;
		
		public OptionType OptionType {
			get {
				return optionType;
			}
			set {
				optionType = value;
			}
		}
		
		public bool OptionValue {
			get {
				return optionvalue;
			}
			set {
				optionvalue = value;
			}
		}
		
		public OptionDeclaration(OptionType optionType) : this(optionType, true)
		{
		}
		
		public OptionDeclaration(OptionType optionType, bool optionvalue)
		{
			this.optionType = optionType;
			this.optionvalue = optionvalue;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[OptionCompareDeclaration: OptionType = {0}, OptionValue = {1}]",
			                     OptionType,
			                     OptionValue);
		}
	}
}
