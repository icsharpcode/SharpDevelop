// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Debugger.MetaData;

namespace Debugger
{
	/// <summary>
	/// Description of IDStringProvider.
	/// </summary>
	public static class IDStringProvider
	{
		/// <summary>
		/// Gets the ID string (C# 4.0 spec, §A.3.1) for the specified entity.
		/// </summary>
		public static string GetIDString(MemberInfo member)
		{
			StringBuilder b = new StringBuilder();
			if (member is Type) {
				b.Append("T:");
				AppendTypeName(b, (Type)member);
			} else {
				if (member is FieldInfo)
					b.Append("F:");
				else if (member is PropertyInfo)
					b.Append("P:");
				else if (member is EventInfo)
					b.Append("E:");
				else if (member is MethodBase)
					b.Append("M:");
				AppendTypeName(b, member.DeclaringType);
				b.Append('.');
				b.Append(member.Name.Replace('.', '#'));
				IList<ParameterInfo> parameters;
				Type explicitReturnType = null;
				if (member is PropertyInfo) {
					parameters = ((PropertyInfo)member).GetIndexParameters();
				} else if (member is MethodInfo) {
					MethodInfo mr = (MethodInfo)member;
					if (mr.IsGenericMethod) {
						b.Append("``");
						// DebugMethodInfo does not implement GetGenericArguments
						if (mr is DebugMethodInfo)
							b.Append(((DebugMethodInfo)mr).GenericParameterCount);
						else
							b.Append(mr.GetGenericArguments().Length);
					}
					parameters = mr.GetParameters();
					if (mr.Name == "op_Implicit" || mr.Name == "op_Explicit") {
						explicitReturnType = mr.ReturnType;
					}
				} else {
					parameters = null;
				}
				if (parameters != null && parameters.Count > 0) {
					b.Append('(');
					for (int i = 0; i < parameters.Count; i++) {
						if (i > 0) b.Append(',');
						AppendTypeName(b, parameters[i].ParameterType);
					}
					b.Append(')');
				}
				if (explicitReturnType != null) {
					b.Append('~');
					AppendTypeName(b, explicitReturnType);
				}
			}
			return b.ToString();
		}
		
		static void AppendTypeName(StringBuilder b, Type type)
		{
			if (type == null) {
				return;
			}
			if (type.IsGenericType) {
				AppendTypeNameWithArguments(b, type, type.GetGenericArguments());
			} else if (type.HasElementType) {
				AppendTypeName(b, type.GetElementType());
				if (type.IsArray) {
					b.Append('[');
					if (type.GetArrayRank() > 1) {
						for (int i = 0; i < type.GetArrayRank(); i++) {
							if (i > 0)
								b.Append(',');
							b.Append("0:");
						}
					}
					b.Append(']');
				}
				if (type.IsByRef) {
					b.Append('@');
				}
				if (type.IsPointer) {
					b.Append('*');
				}
			} else {
				if (type.IsGenericParameter) {
					b.Append('`');
					if (type.DeclaringMethod != null) {
						b.Append('`');
					}
					b.Append(type.GenericParameterPosition);
				} else if (type.DeclaringType != null) {
					AppendTypeName(b, type.DeclaringType);
					b.Append('.');
					b.Append(type.Name);
				} else {
					b.Append(type.FullName);
				}
			}
		}
		
		static int AppendTypeNameWithArguments(StringBuilder b, Type type, IList<Type> genericArguments)
		{
			int outerTypeParameterCount = 0;
			if (type.DeclaringType != null) {
				Type declType = type.DeclaringType;
				outerTypeParameterCount = AppendTypeNameWithArguments(b, declType, genericArguments);
				b.Append('.');
			} else if (!string.IsNullOrEmpty(type.Namespace)) {
				b.Append(type.Namespace);
				b.Append('.');
			}
			int localTypeParameterCount = 0;
			b.Append(SplitTypeParameterCountFromReflectionName(type.Name, out localTypeParameterCount));
			
			if (localTypeParameterCount > 0) {
				int totalTypeParameterCount = outerTypeParameterCount + localTypeParameterCount;
				b.Append('{');
				for (int i = outerTypeParameterCount; i < totalTypeParameterCount && i < genericArguments.Count; i++) {
					if (i > outerTypeParameterCount) b.Append(',');
					AppendTypeName(b, genericArguments[i]);
				}
				b.Append('}');
			}
			return outerTypeParameterCount + localTypeParameterCount;
		}
		
		static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
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
	}
}
