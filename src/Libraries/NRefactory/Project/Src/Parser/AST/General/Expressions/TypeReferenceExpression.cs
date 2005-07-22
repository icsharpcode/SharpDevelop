// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST {
	
	public class TypeReferenceExpression : Expression
	{
		TypeReference  typeReference = TypeReference.Null;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		public TypeReferenceExpression(string type)
		{
			this.typeReference = new TypeReference(type == null ? String.Empty : type);
		}
		
		public TypeReferenceExpression(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[TypeReferenceExpression: TypeReference={0}]", 
			                     typeReference);
		}
	}
}
