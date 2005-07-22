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
	public class ObjectCreateExpression : Expression
	{
		TypeReference    createType;
//		List<Expression> parameters;
		ArrayList parameters;
		
		public TypeReference CreateType {
			get {
				return createType;
			}
			set {
				createType = TypeReference.CheckNull(value);
			}
		}
		
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new ArrayList(1) : value;
			}
		}
		
		public ObjectCreateExpression(TypeReference createType, ArrayList parameters)
		{
			this.CreateType = createType;
			this.Parameters = parameters;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ObjectCreateExpression: CreateType={0}, Parameters={1}]",
			                     createType,
			                     GetCollectionString(parameters));
		}
	}
}
