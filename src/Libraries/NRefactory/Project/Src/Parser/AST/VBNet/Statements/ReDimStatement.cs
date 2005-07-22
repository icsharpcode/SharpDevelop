// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class ReDimStatement : Statement
	{
//		List<Expression> reDimClauses = new List<Expression>(1);
		ArrayList reDimClauses = new ArrayList(1);
		bool              isPreserve   = false;
		
		public bool IsPreserve {
			get {
				return isPreserve;
			}
			set {
				isPreserve = value;
			}
		}
		public ArrayList ReDimClauses {
			get {
				return reDimClauses;
			}
		}
		
		public ReDimStatement(bool isPreserve)
		{
			this.isPreserve = isPreserve;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ReDimStatement: ReDimClauses = {0}]",
			                     GetCollectionString(ReDimClauses));
		}
	}
	
//	public class ReDimClause : AbstractNode
//	{
//		string           name;
//		List<Expression> initializers = new List<Expression>(1);
//		
//		public string Name {
//			get {
//				return name;
//			}
//			set {
//				name = value == null ? String.Empty : value;
//			}
//		}
//		
//		public List<Expression> Initializers {
//			get {
//				return initializers;
//			}
//		}
//		
//		public ReDimClause(string name)
//		{
//			this.Name = name;
//		}
//		
//		public override object AcceptVisitor(IASTVisitor visitor, object data)
//		{
//			return visitor.Visit(this, data);
//		}
//		
//		public override string ToString()
//		{
//			return String.Format("[ReDimClause: Initializers = {0}, Name = {1}]",
//			                     GetCollectionString(Initializers),
//			                     Name);
//		}
//	}
}
