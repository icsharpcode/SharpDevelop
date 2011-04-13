// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.VB.Ast
{
	public class OptionStatement : AstNode
	{
		public OptionType OptionType { get; set; }
		
		public bool OptionValue { get; set; }
		
		public OptionStatement(OptionType optionType, bool optionValue) {
			OptionType = optionType;
			OptionValue = optionValue;
		}
		
		protected internal override bool DoMatch(AstNode other, Match match)
		{
			var stmt = other as OptionStatement;
			return stmt != null && stmt.OptionType == this.OptionType
				&& stmt.OptionValue == this.OptionValue;
		}
		
		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitOptionStatement(this, data);
		}
		
		public override string ToString() {
			return string.Format("[OptionStatement OptionType={0} OptionValue={1}]", OptionType, OptionValue);
		}
	}
	
	public enum OptionType
	{
		Explicit,
		Strict,
		CompareBinary,
		CompareText,
		Infer
	}
}
