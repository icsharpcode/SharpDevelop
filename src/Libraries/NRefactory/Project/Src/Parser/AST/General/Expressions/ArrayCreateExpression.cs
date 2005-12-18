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
	public class ArrayCreateExpression : Expression
	{
		TypeReference              createType;
		List<Expression>           arguments;
		ArrayInitializerExpression arrayInitializer = null; // Array Initializer OR NULL
		
		public TypeReference CreateType {
			get {
				return createType;
			}
			set {
				createType = TypeReference.CheckNull(value);
			}
		}
		
		public List<Expression> Arguments {
			get {
				return arguments;
			}
			set {
				arguments = value ?? new List<Expression>(1);
			}
		}
		
		public ArrayInitializerExpression ArrayInitializer {
			get {
				return arrayInitializer;
			}
			set {
				arrayInitializer = ArrayInitializerExpression.CheckNull(value);
			}
		}
		
		public ArrayCreateExpression(TypeReference createType) : this(createType, null, null)
		{
		}
		
		public ArrayCreateExpression(TypeReference createType, List<Expression> parameters) : this (createType, parameters, null)
		{
		}
		
		public ArrayCreateExpression(TypeReference createType, ArrayInitializerExpression arrayInitializer) : this(createType, null, arrayInitializer)
		{
		}
		
		public ArrayCreateExpression(TypeReference createType, List<Expression> parameters, ArrayInitializerExpression arrayInitializer)
		{
			this.CreateType       = createType;
			this.Arguments       = parameters;
			this.ArrayInitializer = arrayInitializer;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ArrayCreateExpression: CreateType={0}, Parameters={1}, ArrayInitializer={2}]",
			                     createType,
			                     GetCollectionString(arguments),
			                     arrayInitializer);
		}
	}
}
