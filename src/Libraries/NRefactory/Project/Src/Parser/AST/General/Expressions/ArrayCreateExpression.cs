using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{
	// TODO: Overwork array create expression.
	// what is rank ?
	// is ArrayCreationParameter really needed ?
	public class ArrayCreateExpression : Expression
	{
		TypeReference              createType;
//		List<Expression>           parameters;
		ArrayList parameters;
		int[]                      rank             = new int[0];
		ArrayInitializerExpression arrayInitializer = null; // Array Initializer OR NULL
		
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
		
		public int[] Rank {
			get {
				return rank;
			}
			set {
				rank = value == null ? new int[0] : value;
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
		
		public ArrayCreateExpression(TypeReference createType, ArrayList parameters) : this (createType, parameters, null)
		{
		}
		
		public ArrayCreateExpression(TypeReference createType, ArrayInitializerExpression arrayInitializer) : this(createType, null, arrayInitializer)
		{
		}
		
		public ArrayCreateExpression(TypeReference createType, ArrayList parameters, ArrayInitializerExpression arrayInitializer)
		{
			this.CreateType       = createType;
			this.Parameters       = parameters;
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
			                     GetCollectionString(parameters),
			                     arrayInitializer);
		}
	}
}
