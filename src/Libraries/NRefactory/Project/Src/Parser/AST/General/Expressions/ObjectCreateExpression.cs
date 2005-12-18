// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST 
{
	public class ObjectCreateExpression : Expression
	{
		TypeReference    createType;
		List<Expression> parameters;
		
		public TypeReference CreateType {
			get {
				return createType;
			}
			set {
				createType = TypeReference.CheckNull(value);
			}
		}
		
		public List<Expression> Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value ?? new List<Expression>(1);
			}
		}
		
		public ObjectCreateExpression(TypeReference createType, List<Expression> parameters)
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
