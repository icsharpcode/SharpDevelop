

using System;
using System.Diagnostics;
using System.Collections;

namespace ICSharpCode.NRefactory.Parser.AST
{	
	public class ParameterDeclarationExpression : Expression
	{
		TypeReference  typeReference = TypeReference.Null;
		string         parameterName = "";
		ParamModifier  paramModifier = ParamModifier.In;
		ArrayList attributes = new ArrayList(1);
		Expression     defaultValue  = Expression.Null;
		
		public Expression DefaultValue {
			get {
				return defaultValue;
			}
			set {
				defaultValue = Expression.CheckNull(defaultValue);
			}
		}
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				Debug.Assert(value != null);
				typeReference = value;
			}
		}
		public string ParameterName {
			get {
				return parameterName;
			}
			set {
				Debug.Assert(value != null);
				parameterName = value;
			}
		}
		
		public ParamModifier ParamModifier {
			get {
				return paramModifier;
			}
			set {
				paramModifier = value;
			}
		}
		
		public ArrayList Attributes {
			get {
				return attributes;
			}
			set {
				Debug.Assert(value != null);
				attributes = value;
			}
		}
		
		public ParameterDeclarationExpression(TypeReference typeReference, string parameterName)
		{
			this.typeReference  = typeReference;
			Debug.Assert(parameterName != null);
			this.parameterName  = parameterName;
			this.paramModifier = ParamModifier.In;
		}
		
		public ParameterDeclarationExpression(TypeReference typeReference, string parameterName, ParamModifier paramModifier)
		{
			this.typeReference  = typeReference;
			Debug.Assert(parameterName != null);
			this.parameterName  = parameterName;
			this.paramModifier = paramModifier;
		}
		
		public ParameterDeclarationExpression(TypeReference typeReference, string parameterName, ParamModifier paramModifier, Expression defaultValue)
		{
			this.typeReference  = typeReference;
			Debug.Assert(parameterName != null);
			this.parameterName  = parameterName;
			this.paramModifier = paramModifier;
			this.DefaultValue = defaultValue;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[ParameterDeclarationExpression: TypeReference={0}, ParameterName={1}, ParamModifier={2}, Attributes={3}]",
			                     TypeReference,
			                     ParameterName,
			                     ParamModifier,
			                     GetCollectionString(Attributes));
		}
		
	}
}
