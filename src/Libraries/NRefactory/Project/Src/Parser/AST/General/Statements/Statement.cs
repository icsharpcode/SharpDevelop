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
	public abstract class Statement : AbstractNode, INullable
	{
		public static NullStatement Null {
			get {
				return NullStatement.Instance;
			}
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public static Statement CheckNull(Statement statement)
		{
			return statement == null ? NullStatement.Instance : statement;
		}
	}
	
	public class NullStatement : Statement
	{
		static NullStatement nullStatement = new NullStatement();
		
		public override bool IsNull {
			get {
				return true;
			}
		}
		
		public static NullStatement Instance {
			get {
				return nullStatement;
			}
		}
		
		NullStatement()
		{
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return data;
		}
		
		public override string ToString()
		{
			return String.Format("[NullStatement]");
		}
	}
}
