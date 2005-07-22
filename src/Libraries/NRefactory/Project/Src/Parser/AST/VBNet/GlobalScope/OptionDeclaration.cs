// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

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
