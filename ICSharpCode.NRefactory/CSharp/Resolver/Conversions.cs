// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	public class Conversions
	{
		readonly ITypeResolveContext context;
		readonly IType objectType;
		
		public Conversions(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
			this.objectType = context.GetClass(typeof(object)) ?? SharedTypes.UnknownType;
			this.dynamicErasure = new DynamicErasure(this);
		}
		
		#region ImplicitConversion
		public bool ImplicitConversion(IType fromType, IType toType)
		{
			if (fromType == null)
				throw new ArgumentNullException("fromType");
			if (toType == null)
				throw new ArgumentNullException("toType");
			// C# 4.0 spec: §6.1
			if (IdentityConversion(fromType, toType))
				return true;
			if (ImplicitNumericConversion(fromType, toType))
				return true;
			if (ImplicitReferenceConversion(fromType, toType))
				return true;
			if (ImplicitEnumerationConversion(fromType, toType))
				return true;
			if (ImplicitNullableConversion(fromType, toType))
				return true;
			if (NullLiteralConversion(fromType, toType))
				return true;
			if (BoxingConversion(fromType, toType))
				return true;
			if (ImplicitDynamicConversion(fromType, toType))
				return true;
			if (ImplicitConstantExpressionConversion(fromType, toType))
				return true;
			return false;
		}
		#endregion
		
		#region IdentityConversion
		/// <summary>
		/// Gets whether there is an identity conversion from <paramref name="fromType"/> to <paramref name="toType"/>
		/// </summary>
		bool IdentityConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.1
			return fromType.AcceptVisitor(dynamicErasure).Equals(toType.AcceptVisitor(dynamicErasure));
		}
		
		readonly DynamicErasure dynamicErasure;
		
		sealed class DynamicErasure : TypeVisitor
		{
			readonly IType objectType;
			
			public DynamicErasure(Conversions conversions)
			{
				this.objectType = conversions.objectType;
			}
			
			public override IType VisitOtherType(IType type)
			{
				if (type == SharedTypes.Dynamic)
					return objectType;
				else
					return base.VisitOtherType(type);
			}
		}
		#endregion
		
		#region ImplicitNumericConversion
		static readonly bool[,] implicitNumericConversionLookup = {
			//       to:   short  ushort  int   uint   long   ulong
			// from:
			/* char   */ { false, true , true , true , true , true  },
			/* sbyte  */ { true , false, true , false, true , false },
			/* byte   */ { true , true , true , true , true , true  },
			/* short  */ { false, false, true , false, true , false },
			/* ushort */ { false, false, true , true , true , true  },
			/* int    */ { false, false, false, false, true , false },
			/* uint   */ { false, false, false, false, true , true  },
		};
		
		bool ImplicitNumericConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.2
			
			TypeCode from = ReflectionHelper.GetTypeCode(fromType);
			TypeCode to = ReflectionHelper.GetTypeCode(toType);
			if (to >= TypeCode.Single && to <= TypeCode.Decimal) {
				// Conversions to float/double/decimal exist from all integral types,
				// and there's a conversion from float to double.
				return from >= TypeCode.Char && from <= TypeCode.UInt64
					|| from == TypeCode.Single && to == TypeCode.Double;
			} else {
				// Conversions to integral types: look at the table
				return from >= TypeCode.Char && from <= TypeCode.UInt32
					&& to >= TypeCode.Int16 && to <= TypeCode.UInt64
					&& implicitNumericConversionLookup[to - TypeCode.Int16, from - TypeCode.Char];
			}
		}
		#endregion
		
		#region ImplicitEnumerationConversion
		bool ImplicitEnumerationConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.3
			// TODO: implement ImplicitEnumerationConversion and ImplicitConstantExpressionConversion
			return false;
		}
		#endregion
		
		#region ImplicitNullableConversion
		bool ImplicitNullableConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.4
			IType t = UnpackNullable(toType);
			if (t != null) {
				IType s = UnpackNullable(fromType) ?? fromType;
				return IdentityConversion(s, t) || ImplicitNumericConversion(s, t);
			} else {
				return false;
			}
		}
		
		static IType UnpackNullable(IType type)
		{
			ParameterizedType pt = type as ParameterizedType;
			if (pt != null && pt.TypeArguments.Count == 1 && pt.FullName == "System.Nullable")
				return pt.TypeArguments[0];
			else
				return null;
		}
		#endregion
		
		#region NullLiteralConversion
		bool NullLiteralConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.5
			return fromType == SharedTypes.Null && (UnpackNullable(toType) != null);
			// This function only handles the conversion from the null literal to nullable value types,
			// reference types are handled by ImplicitReferenceConversion instead.
		}
		#endregion
		
		#region ImplicitReferenceConversion
		bool ImplicitReferenceConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.6
			
			// reference conversions are possible only if both types are known to be reference types
			if (fromType.IsReferenceType != true || toType.IsReferenceType != true)
				return false;
			
			// conversion from null literal is always possible
			if (fromType == SharedTypes.Null)
				return true;
			
			ArrayType fromArray = fromType as ArrayType;
			if (fromArray != null) {
				ArrayType toArray = toType as ArrayType;
				if (toArray != null) {
					// array covariance (the broken kind)
					return fromArray.Dimensions == toArray.Dimensions
						&& ImplicitReferenceConversion(fromArray.ElementType, toArray.ElementType);
				}
				// conversion from single-dimensional array S[] to IList<T>:
				ParameterizedType toPT = toType as ParameterizedType;
				if (fromArray.Dimensions == 1 && toPT != null && toPT.TypeArguments.Count == 1
				    && toPT.Namespace == "System.Collections.Generic"
				    && (toPT.Name == "IList" || toPT.Name == "ICollection" || toPT.Name == "IEnumerable"))
				{
					// array covariance plays a part here as well (string[] is IList<object>)
					return IdentityConversion(fromArray.ElementType, toPT.TypeArguments[0])
						|| ImplicitReferenceConversion(fromArray.ElementType, toPT.TypeArguments[0]);
				}
				// conversion from any array to System.Array and the interfaces it implements:
				ITypeDefinition systemArray = context.GetClass("System.Array", 0, StringComparer.Ordinal);
				return systemArray != null && (systemArray.Equals(toType) || ImplicitReferenceConversion(systemArray, toType));
			}
			
			// now comes the hard part: traverse the inheritance chain and figure out generics+variance
			return IsSubtypeOf(fromType, toType);
		}
		
		// Determines whether s is a subtype of t.
		// Helper method used for ImplicitReferenceConversion and BoxingConversion
		bool IsSubtypeOf(IType s, IType t)
		{
			// conversion to dynamic + object are always possible
			if (t == SharedTypes.Dynamic || t.Equals(objectType))
				return true;
			
			// let GetAllBaseTypes do the work for us
			foreach (IType baseType in s.GetAllBaseTypes(context)) {
				if (IdentityOrVarianceConversion(baseType, t))
					return true;
			}
			return false;
		}
		
		bool IdentityOrVarianceConversion(IType s, IType t)
		{
			ITypeDefinition def = s.GetDefinition();
			if (def != null && def.Equals(t.GetDefinition())) {
				ParameterizedType ps = s as ParameterizedType;
				ParameterizedType pt = t as ParameterizedType;
				if (ps != null && pt != null
				    && ps.TypeArguments.Count == pt.TypeArguments.Count
				    && ps.TypeArguments.Count == def.TypeParameters.Count)
				{
					// C# 4.0 spec: §13.1.3.2 Variance Conversion
					for (int i = 0; i < def.TypeParameters.Count; i++) {
						IType si = ps.TypeArguments[i];
						IType ti = pt.TypeArguments[i];
						if (IdentityConversion(si, ti))
							continue;
						ITypeParameter xi = def.TypeParameters[i];
						switch (xi.Variance) {
							case VarianceModifier.Covariant:
								if (!ImplicitReferenceConversion(si, ti))
									return false;
								break;
							case VarianceModifier.Contravariant:
								if (!ImplicitReferenceConversion(ti, si))
									return false;
								break;
							default:
								return false;
						}
					}
				} else if (ps != null || pt != null) {
					return false; // only of of them is parameterized, or counts don't match? -> not valid conversion
				}
				return true;
			}
			return false;
		}
		#endregion
		
		#region BoxingConversion
		bool BoxingConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.7
			fromType = UnpackNullable(fromType) ?? fromType;
			return fromType.IsReferenceType == false && toType.IsReferenceType == true && IsSubtypeOf(fromType, toType);
		}
		#endregion
		
		#region ImplicitDynamicConversion
		bool ImplicitDynamicConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.8
			return fromType == SharedTypes.Dynamic;
		}
		#endregion
		
		#region ImplicitConstantExpressionConversion
		bool ImplicitConstantExpressionConversion(IType fromType, IType toType)
		{
			// C# 4.0 spec: §6.1.9
			// TODO: implement ImplicitEnumerationConversion and ImplicitConstantExpressionConversion
			return false;
		}
		#endregion
		
		// TODO: add support for user-defined conversions
	}
}
