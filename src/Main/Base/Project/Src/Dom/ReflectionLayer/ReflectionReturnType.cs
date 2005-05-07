// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	public static class ReflectionReturnType
	{
		public static IReturnType Create(IProjectContent content, Type type)
		{
			string name = type.FullName;
			if (name == null)
				return null;
			if (name.Length > 2) {
				if (name[name.Length - 2] == '`') {
					name = name.Substring(0, name.Length - 2);
				}
			}
			if (type.IsArray) {
				return MakeArray(type, Create(content, type.GetElementType()));
			} else {
				return new GetClassReturnType(content, name);
			}
		}
		
		public static IReturnType Create(IMember member, Type type)
		{
			if (type.IsArray) {
				return MakeArray(type, Create(member, type.GetElementType()));
			} else if (type.IsGenericType && !type.IsGenericTypeDefinition) {
				Type[] args = type.GetGenericArguments();
				List<IReturnType> para = new List<IReturnType>(args.Length);
				for (int i = 0; i < args.Length; ++i) {
					para.Add(Create(member, args[i]));
				}
				return new SpecificReturnType(Create(member, type.GetGenericTypeDefinition()), para);
			} else if (type.IsGenericParameter) {
				IClass c = member.DeclaringType;
				if (type.GenericParameterPosition < c.TypeParameters.Count) {
					if (c.TypeParameters[type.GenericParameterPosition].Name == type.Name) {
						return new GenericReturnType(c.TypeParameters[type.GenericParameterPosition]);
					}
				}
				if (type.DeclaringMethod != null) {
					IMethod method = member as IMethod;
					if (method != null) {
						// Create GenericReturnType for generic method
					}
				}
				return new GenericReturnType(new DefaultTypeParameter(c, type));
			}
			return Create(member.DeclaringType.ProjectContent, type);
		}
		
		static IReturnType MakeArray(Type type, IReturnType baseType)
		{
			return new ArrayReturnType(baseType, type.GetArrayRank());
		}
	}
	
	/*
	[Serializable]
	public class ReflectionReturnType : AbstractReturnType
	{
		public ReflectionReturnType(Type type)
		{
			string fullyQualifiedName = type.FullName == null ? type.Name : type.FullName.Replace("+", ".").Trim('&');
			
			// base.FullyQualifiedName = fullyQualifiedName.TrimEnd('[', ']', ',', '*');
			for (int i = fullyQualifiedName.Length; i > 0; i--) {
				char c = fullyQualifiedName[i - 1];
				if (c != '[' && c != ']' && c != ',' && c != '*') {
					if (i < fullyQualifiedName.Length)
						fullyQualifiedName = fullyQualifiedName.Substring(0, i);
					break;
				}
			}
			base.FullyQualifiedName = fullyQualifiedName;

			SetPointerNestingLevel(type);
			SetArrayDimensions(type);
			if (arrays == null)
				arrayDimensions = new int[0];
			else
				arrayDimensions = (int[])arrays.ToArray(typeof(int));
		}
		
		ArrayList arrays = null;
		void SetArrayDimensions(Type type)
		{
			if (type.IsArray && type != typeof(Array)) {
				if (arrays == null)
					arrays = new ArrayList();
				arrays.Add(type.GetArrayRank());
				SetArrayDimensions(type.GetElementType());
			}
		}
		
		void SetPointerNestingLevel(Type type)
		{
			if (type.IsPointer) {
				SetPointerNestingLevel(type.GetElementType());
				++pointerNestingLevel;
			}
		}
	}
	 */
}
