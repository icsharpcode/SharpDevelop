// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="none" email=""/>
//     <version>$Revision: 230 $</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class DefaultValueExpression : Expression
	{
		TypeReference typeReference;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
			}
		}
		
		public DefaultValueExpression(TypeReference typeReference)
		{
			this.TypeReference = typeReference;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[DefaultValueExpression: TypeReference={0}]",
			                     typeReference);
		}
	}
}
