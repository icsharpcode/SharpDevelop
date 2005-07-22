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
	public class FixedStatement : Statement
	{
		TypeReference             typeReference;
//		List<VariableDeclaration> pointerDeclarators;
		ArrayList pointerDeclarators;
		Statement                 embeddedStatement;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		public ArrayList PointerDeclarators {
			get {
				return pointerDeclarators;
			}
			set {
				pointerDeclarators = value == null ? new ArrayList(1) : value;
			}
		}
		
		public Statement EmbeddedStatement {
			get {
				return embeddedStatement;
			}
			set {
				embeddedStatement = Statement.CheckNull(value);
			}
		}
		
		public FixedStatement(TypeReference typeReference, ArrayList pointerDeclarators, Statement embeddedStatement)
		{
			this.TypeReference      = typeReference;
			this.PointerDeclarators = pointerDeclarators;
			this.EmbeddedStatement  = embeddedStatement;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[FixedStatement: TypeReference={0}, PointerDeclarators={1}, EmbeddedStatement={2}]", 
			                     typeReference,
			                     GetCollectionString(pointerDeclarators),
			                     embeddedStatement);
		}
	}
}
