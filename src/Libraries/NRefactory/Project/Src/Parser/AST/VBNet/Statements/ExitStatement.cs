// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public enum ExitType
	{
		None,
		Sub,
		Function,
		Property,
		Do,
		For,
		While,
		Select,
		Try
	}
	
	public class ExitStatement : Statement
	{
		ExitType exitType;
		
		public ExitType ExitType {
			get {
				return exitType;
			}
			set {
				exitType = value;
			}
		}
		
		public ExitStatement(ExitType exitType)
		{
			this.exitType = exitType;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ExitStatement]");
		}
	}
}
