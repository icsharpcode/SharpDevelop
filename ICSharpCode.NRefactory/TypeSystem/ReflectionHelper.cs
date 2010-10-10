// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Static helper methods for reflection names.
	/// </summary>
	public static class ReflectionHelper
	{
		/// <summary>
		/// A reflection class used to represent <c>null</c>.
		/// </summary>
		public sealed class Null {}
		
		/// <summary>
		/// A reflection class used to represent <c>dynamic</c>.
		/// </summary>
		public sealed class Dynamic {}
		
		/// <summary>
		/// Retrieves a class.
		/// </summary>
		/// <returns>Returns the class; or null if it is not found.</returns>
		public static ITypeDefinition GetClass(this ITypeResolveContext context, Type type)
		{
			if (type == null)
				return null;
			while (type.IsArray || type.IsPointer || type.IsByRef)
				type = type.GetElementType();
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
				type = type.GetGenericTypeDefinition();
			if (type.IsGenericParameter)
				return null;
			if (type.DeclaringType != null) {
				ITypeDefinition declaringType = GetClass(context, type.DeclaringType);
				if (declaringType != null) {
					int typeParameterCount;
					string name = SplitTypeParameterCountFromReflectionName(type.Name, out typeParameterCount);
					typeParameterCount += declaringType.TypeParameterCount;
					foreach (ITypeDefinition innerClass in declaringType.InnerClasses) {
						if (innerClass.Name == name && innerClass.TypeParameterCount == typeParameterCount) {
							return innerClass;
						}
					}
				}
				return null;
			} else {
				int typeParameterCount;
				string name = SplitTypeParameterCountFromReflectionName(type.FullName, out typeParameterCount);
				return context.GetClass(name, typeParameterCount, StringComparer.Ordinal);
			}
		}
		
		/// <summary>
		/// Creates a reference to the specified type.
		/// </summary>
		/// <param name="type">The type to be converted.</param>
		/// <param name="entity">The parent entity, used to fetch the ITypeParameter for generic types.</param>
		/// <returns>Returns the type reference.</returns>
		public static ITypeReference ToTypeReference(this Type type, IEntity entity = null)
		{
			if (type == null)
				return SharedTypes.UnknownType;
			if (type.IsGenericType && !type.IsGenericTypeDefinition) {
				ITypeReference def = ToTypeReference(type.GetGenericTypeDefinition(), entity);
				Type[] arguments = type.GetGenericArguments();
				ITypeReference[] args = new ITypeReference[arguments.Length];
				for (int i = 0; i < arguments.Length; i++) {
					args[i] = ToTypeReference(arguments[i], entity);
				}
				return new ParameterizedTypeReference(def, args);
			} else if (type.IsArray) {
				return new ArrayTypeReference(ToTypeReference(type.GetElementType(), entity), type.GetArrayRank());
			} else if (type.IsPointer) {
				return new PointerTypeReference(ToTypeReference(type.GetElementType(), entity));
			} else if (type.IsByRef) {
				return new ByReferenceTypeReference(ToTypeReference(type.GetElementType(), entity));
			} else if (type.IsGenericParameter) {
				if (type.DeclaringMethod != null) {
					IMethod method = entity as IMethod;
					if (method != null) {
						if (type.GenericParameterPosition < method.TypeParameters.Count) {
							return method.TypeParameters[type.GenericParameterPosition];
						}
					}
					return SharedTypes.UnknownType;
				} else {
					ITypeDefinition c = (entity as ITypeDefinition) ?? (entity is IMember ? ((IMember)entity).DeclaringTypeDefinition : null);
					if (c != null && type.GenericParameterPosition < c.TypeParameters.Count) {
						if (c.TypeParameters[type.GenericParameterPosition].Name == type.Name) {
							return c.TypeParameters[type.GenericParameterPosition];
						}
					}
					return SharedTypes.UnknownType;
				}
			} else if (type.DeclaringType != null) {
				if (type == typeof(Dynamic))
					return SharedTypes.Dynamic;
				else if (type == typeof(Null))
					return SharedTypes.Null;
				ITypeReference baseTypeRef = ToTypeReference(type.DeclaringType, entity);
				int typeParameterCount;
				string name = SplitTypeParameterCountFromReflectionName(type.Name, out typeParameterCount);
				return new NestedTypeReference(baseTypeRef, name, typeParameterCount);
			} else {
				int typeParameterCount;
				string name = SplitTypeParameterCountFromReflectionName(type.FullName, out typeParameterCount);
				return new GetClassTypeReference(name, typeParameterCount);
			}
		}
		
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				return reflectionName;
			} else {
				return reflectionName.Substring(0, pos);
			}
		}
		
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0) {
				typeParameterCount = 0;
				return reflectionName;
			} else {
				string typeCount = reflectionName.Substring(pos + 1);
				if (int.TryParse(typeCount, out typeParameterCount))
					return reflectionName.Substring(0, pos);
				else
					return reflectionName;
			}
		}
		
		static readonly ITypeReference[] primitiveTypeReferences = {
			SharedTypes.UnknownType, // TypeCode.Empty
			new GetClassTypeReference("System.Object", 0),
			new GetClassTypeReference("System.DBNull", 0),
			new GetClassTypeReference("System.Boolean", 0),
			new GetClassTypeReference("System.Char", 0),
			new GetClassTypeReference("System.SByte", 0),
			new GetClassTypeReference("System.Byte", 0),
			new GetClassTypeReference("System.Int16", 0),
			new GetClassTypeReference("System.UInt16", 0),
			new GetClassTypeReference("System.Int32", 0),
			new GetClassTypeReference("System.UInt32", 0),
			new GetClassTypeReference("System.Int64", 0),
			new GetClassTypeReference("System.UInt64", 0),
			new GetClassTypeReference("System.Single", 0),
			new GetClassTypeReference("System.Double", 0),
			new GetClassTypeReference("System.Decimal", 0),
			new GetClassTypeReference("System.DateTime", 0),
			SharedTypes.UnknownType, // (TypeCode)17 has no enum value?
			new GetClassTypeReference("System.String", 0)
		};
		
		/// <summary>
		/// Creates a reference to the specified type.
		/// </summary>
		/// <param name="typeCode">The type to be converted.</param>
		/// <returns>Returns the type reference.</returns>
		public static ITypeReference ToTypeReference(this TypeCode typeCode)
		{
			return primitiveTypeReferences[(int)typeCode];
		}
		
		static readonly Dictionary<string, TypeCode> typeNameToCodeDict = new Dictionary<string, TypeCode> {
			{ "System.Object",   TypeCode.Object },
			{ "System.DBNull",   TypeCode.DBNull },
			{ "System.Boolean",  TypeCode.Boolean },
			{ "System.Char",     TypeCode.Char },
			{ "System.SByte",    TypeCode.SByte },
			{ "System.Byte",     TypeCode.Byte },
			{ "System.Int16",    TypeCode.Int16 },
			{ "System.UInt16",   TypeCode.UInt16 },
			{ "System.Int32",    TypeCode.Int32 },
			{ "System.UInt32",   TypeCode.UInt32 },
			{ "System.Int64",    TypeCode.Int64 },
			{ "System.UInt64",   TypeCode.UInt64 },
			{ "System.Single",   TypeCode.Single },
			{ "System.Double",   TypeCode.Double },
			{ "System.Decimal",  TypeCode.Decimal },
			{ "System.DateTime", TypeCode.DateTime },
			{ "System.String",   TypeCode.String }
		};
		
		/// <summary>
		/// Gets the type code for the specified type, or TypeCode.Empty if none of the other type codes matches.
		/// </summary>
		public static TypeCode GetTypeCode(IType type)
		{
			ITypeDefinition def = type as ITypeDefinition;
			TypeCode typeCode;
			if (def != null && def.TypeParameterCount == 0 && typeNameToCodeDict.TryGetValue(def.FullName, out typeCode))
				return typeCode;
			else
				return TypeCode.Empty;
		}
	}
}
