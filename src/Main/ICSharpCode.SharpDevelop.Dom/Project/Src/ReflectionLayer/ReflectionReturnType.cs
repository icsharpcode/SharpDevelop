// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Dom.ReflectionLayer
{
	public static class ReflectionReturnType
	{
		public static bool IsDefaultType(Type type)
		{
			return !type.IsArray && !type.IsGenericType && !type.IsGenericParameter;
		}
		
		public static IReturnType Create(IClass @class, Type type, bool createLazyReturnType)
		{
			return Create(@class.ProjectContent, @class, type, createLazyReturnType);
		}
		
		public static IReturnType Create(IMember member, Type type, bool createLazyReturnType)
		{
			return Create(member.DeclaringType.ProjectContent, member, type, createLazyReturnType);
		}
		
		public static IReturnType Create(IProjectContent pc, IEntity member, Type type, bool createLazyReturnType)
		{
			return Create(pc, member, type, createLazyReturnType, true);
		}
		
		static IReturnType Create(IProjectContent pc, IEntity member, Type type, bool createLazyReturnType, bool forceGenericType)
		{
			if (type.IsByRef) {
				// TODO: Use ByRefRefReturnType
				return Create(pc, member, type.GetElementType(), createLazyReturnType);
			} else if (type.IsPointer) {
				return new PointerReturnType(Create(pc, member, type.GetElementType(), createLazyReturnType));
			} else if (type.IsArray) {
				return new ArrayReturnType(pc, Create(pc, member, type.GetElementType(), createLazyReturnType), type.GetArrayRank());
			} else if (type.IsGenericType && (forceGenericType || !type.IsGenericTypeDefinition)) {
				Type[] args = type.GetGenericArguments();
				List<IReturnType> para = new List<IReturnType>(args.Length);
				for (int i = 0; i < args.Length; ++i) {
					para.Add(Create(pc, member, args[i], createLazyReturnType));
				}
				return new ConstructedReturnType(Create(pc, member, type.GetGenericTypeDefinition(), createLazyReturnType, false), para);
			} else if (type.IsGenericParameter) {
				IClass c = (member is IClass) ? (IClass)member : (member is IMember) ? ((IMember)member).DeclaringType : null;
				if (c != null && type.GenericParameterPosition < c.TypeParameters.Count) {
					if (c.TypeParameters[type.GenericParameterPosition].Name == type.Name) {
						return new GenericReturnType(c.TypeParameters[type.GenericParameterPosition]);
					}
				}
				if (type.DeclaringMethod != null) {
					IMethod method = member as IMethod;
					if (method != null) {
						if (type.GenericParameterPosition < method.TypeParameters.Count) {
							return new GenericReturnType(method.TypeParameters[type.GenericParameterPosition]);
						}
						return new GenericReturnType(new DefaultTypeParameter(method, type));
					}
				}
				return new GenericReturnType(new DefaultTypeParameter(c, type));
			} else {
				string name = type.FullName;
				if (name == null)
					throw new ApplicationException("type.FullName returned null. Type: " + type.ToString());
				int typeParameterCount;
				name = ReflectionClass.ConvertReflectionNameToFullName(name, out typeParameterCount);
				if (!createLazyReturnType) {
					IClass c = pc.GetClass(name, typeParameterCount);
					if (c != null)
						return c.DefaultReturnType;
					// example where name is not found: pointers like System.Char*
					// or when the class is in a assembly that is not referenced
				}
				return new GetClassReturnType(pc, name, typeParameterCount);
			}
		}
	}
}
