// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public enum ConversionType {
		None,
		Implicit,
		Explicit
	}
	
	public enum OverloadableOperatorType {
		None,
		
		Add,
		Subtract,
		Multiply,
		Divide,
		Modulus,
		Concat,
		
		Not,
		BitNot,
		
		BitwiseAnd,
		BitwiseOr,
		ExclusiveOr,
		
		ShiftLeft,
		ShiftRight,
		
		GreaterThan,
		GreaterThanOrEqual,
		Equality,
		InEquality,
		LessThan,
		LessThanOrEqual,
		
		Increment,
		Decrement,
		
		True,
		False,
		
		// VB specific
		IsTrue,
		IsFalse,
		Like,
		Power,
		CType,
		DivideInteger
	}
	
	public class OperatorDeclaration : MethodDeclaration
	{
		ConversionType conversionType = ConversionType.None;
		TypeReference  convertToType;
		List<AttributeSection> returnTypeAttributes = new List<AttributeSection>();
		OverloadableOperatorType overloadableOperator = OverloadableOperatorType.None;
		
		public ConversionType ConversionType {
			get {
				return conversionType;
			}
			set {
				conversionType = value;
			}
		}
		
		public List<AttributeSection> ReturnTypeAttributes
		{
			get {
				return returnTypeAttributes;
			}
			set {
				returnTypeAttributes = value;
			}
		}
		
		public TypeReference ConvertToType {
			get {
				return convertToType;
			}
			set {
				convertToType = TypeReference.CheckNull(value);
			}
		}
		
		public OverloadableOperatorType OverloadableOperator {
			get {
				return overloadableOperator;
			}
			set {
				overloadableOperator = value;
			}
		}
		
		public bool IsConversionOperator {
			get {
				return conversionType != ConversionType.None;
			}
		}
		
		/// <summary>
		/// Constructor for conversion type operators
		/// </summary>
		/// <param name="modifier"></param>
		/// <param name="List"></param>
		public OperatorDeclaration(Modifier modifier,
		                           List<AttributeSection> attributes,
		                           List<ParameterDeclarationExpression> parameters, 
		                           TypeReference convertToType,
		                           ConversionType conversionType
		                           ) : base(null, modifier, TypeReference.Null, parameters, attributes)
		{
			this.ConversionType = conversionType;
			this.ConvertToType  = convertToType;
		}
		
		/// <summary>
		/// Constructor for operator type operators
		/// </summary>
		/// <param name="modifier"></param>
		/// <param name="List"></param>
		public OperatorDeclaration(Modifier modifier,
		                           List<AttributeSection> attributes,
		                           List<ParameterDeclarationExpression> parameters, 
		                           TypeReference typeReference,
		                           OverloadableOperatorType overloadableOperator
		                           ) : base(null, modifier, typeReference, parameters, attributes)
		{
			this.overloadableOperator = overloadableOperator;
			convertToType = TypeReference.Null;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[OperatorDeclaration: conversionType = {0}, convertToType = {1}, operator ={2}, BASE={3}]",
			                     conversionType,
			                     convertToType,
			                     this.overloadableOperator,
			                     base.ToString()
			                    );
		}
	}
}
