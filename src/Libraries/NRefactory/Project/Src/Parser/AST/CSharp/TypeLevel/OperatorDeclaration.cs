// OperatorDeclaratoin.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

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
		
		
	}
	public class OperatorDeclaration : MethodDeclaration
	{
		ConversionType conversionType = ConversionType.None;
		TypeReference  convertToType;
		
		OverloadableOperatorType overloadableOperator = OverloadableOperatorType.None;
		
		public ConversionType ConversionType {
			get {
				return conversionType;
			}
			set {
				conversionType = value;
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
			convertToType  = TypeReference.Null;
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
